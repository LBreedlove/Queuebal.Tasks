using Queuebal.Tasks.Core;
using Queuebal.Tasks.Models;

namespace Queuebal.Tasks.Processor;


/// <summary>
/// An abstract base class that can be used as a base for processing tasks.
/// </summary>
public abstract class ProcessorBase
{
    private readonly IWorkerTaskConsumer _consumer;
    private readonly IIdempotencyChecker _idempotencyChecker;
    private readonly WorkerTaskProcessorConfiguration _configuration;
    private readonly string _processorId;

    /// <summary>
    /// Initializes a new instance of the ProcessorBase class.
    /// </summary>
    /// <param name="consumer">The consumer used to pull from the queue.</param>
    /// <param name="idempotencyChecker">The idempotency checker used to check for duplicate tasks.</param>
    /// <param name="configuration">The configuration for the processor.</param>
    /// <param name="processorId">The ID of the processor.</param>
    protected ProcessorBase(IWorkerTaskConsumer consumer, IIdempotencyChecker idempotencyChecker, WorkerTaskProcessorConfiguration configuration, string processorId)
    {
        _consumer = consumer;
        _idempotencyChecker = idempotencyChecker;
        _configuration = configuration;
        _processorId = processorId;
    }

    /// <summary>
    /// Runs the processor loop until the cancellationToken is set.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to end the run loop.</param>
    public async Task Run(CancellationToken cancellationToken)
    {
        for (; ; )
        {
            try
            {
                await RunLoop(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception)
            {
                await Task.Delay(_configuration.SleepTimeAfterConsumerError, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Runs the processor loop until the cancellationToken is set.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to end the run loop.</param>
    private async Task RunLoop(CancellationToken cancellationToken)
    {
        for (; ; )
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            WorkerTaskBatch? batch = await _consumer.GetTasks(_configuration.MaxTasksPerBatch, _configuration.WaitTimeout, cancellationToken);
            if (batch == null)
            {
                continue;
            }

            if (!batch.Tasks.Any())
            {
                // we don't need to sleep because GetTasks will block until tasks are available,
                // or the timeout has been met.
                continue;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                // TODO: Should we call OnFailure for this task batch, or let the
                // lease timeout handle releasing the tasks back to the queue?
                throw new TaskCanceledException();
            }

            bool failed = true;
            try
            {
                await ProcessTaskBatch(batch, cancellationToken);
                failed = false;
            }
            catch (WorkerTaskProcessorException ex)
            {
                var failure = new WorkerTaskFailure
                {
                    ErrorMessage = ex.ErrorMessage,
                    ErrorCode = ex.ErrorCode,
                    FailedTaskId = ex.FailedTaskId,
                    RetryAfter = ex.RetryAfter,
                    RetryFailedTask = ex.RetryFailedTask,
                };
                await OnFailure(ex.WorkerTaskBatch, failure, cancellationToken);
            }
            catch (Exception ex)
            {
                var failure = new WorkerTaskFailure
                {
                    ErrorMessage = ex.Message,
                    FailedTaskId = batch.Tasks.First().WorkerTaskId,
                };
                await OnFailure(batch, failure, cancellationToken);
            }

            if (!failed)
            {
                await OnSuccess(batch, batch.Tasks.Count(), cancellationToken);
            }
        }
    }

    /// <summary>
    /// Checks if a task with the given idempotency key has already been processed.
    /// </summary>
    /// <param name="idempotencyKey">The idempotency key of the task to check.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the asynchronous operation.</param>
    /// <returns></returns>
    protected async Task<bool> IsDuplicateTaskAsync(string idempotencyKey, CancellationToken cancellationToken) =>
        await _idempotencyChecker.IsDuplicateAsync(idempotencyKey, cancellationToken);

    /// <summary>
    /// Method to be implemented by derived classes to process a batch of tasks.
    /// </summary>
    /// <param name="batch">The batch to process.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await the async operation.</returns>
    protected abstract Task ProcessTaskBatch(WorkerTaskBatch batch, CancellationToken cancellationToken);

    /// <summary>
    /// Failure handler called when a batch fails to process completely.
    /// </summary>
    /// <param name="batch">The batch that failed to completely process.</param>
    /// <param name="failure">Information about the failure.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await the async operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the failing taskId is not found in the worker task batch.</exception>
    private async Task OnFailure(WorkerTaskBatch batch, WorkerTaskFailure failure, CancellationToken cancellationToken)
    {
        int indexOfFailedTask = batch.Tasks
            .ToList()
            .FindIndex((t) => t.WorkerTaskId == failure.FailedTaskId);

        if (indexOfFailedTask == -1)
        {
            throw new ArgumentException("Failed taskId was not found in the task batch");
        }

        if (indexOfFailedTask > 0)
        {
            // we need to create a success result for the records that were
            // processed before the failure.
            await OnSuccess(batch, indexOfFailedTask, cancellationToken);
        }

        var workerTaskBatchFailure = new WorkerTaskBatchFailure
        {
            GroupKey = batch.GroupKey,
            ErrorMessage = failure.ErrorMessage,
            ErrorCode = failure.ErrorCode,
            ProcessorId = _processorId,
            FailedWorkerTaskId = failure.FailedTaskId,
            RemainingWorkerTaskIds = batch.Tasks
                .Skip(indexOfFailedTask)
                .Select(task => task.WorkerTaskId)
                .ToList()
        };

        await _consumer.Fail(workerTaskBatchFailure, failure.RetryAfter, cancellationToken);
    }

    /// <summary>
    /// Handler to be called when a batch completes processing successfully.
    /// This method is also called by OnFailure, to account for tasks that completed successfully from the batch.
    /// </summary>
    /// <param name="batch">The batch that was processed.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await the async operation.</returns>
    private async Task OnSuccess(WorkerTaskBatch batch, int successfulTaskCount, CancellationToken cancellationToken)
    {
        if (successfulTaskCount == 0)
        {
            return;
        }

        await _consumer.Commit
        (
            new WorkerTaskBatchCompleted
            {
                GroupKey = batch.GroupKey,
                ProcessorId = _processorId,
                CompletedWorkerTaskIds = batch.Tasks
                    .Take(successfulTaskCount)
                    .Select(t => t.WorkerTaskId)
                    .ToList()
            },
            cancellationToken
        );
    }
}

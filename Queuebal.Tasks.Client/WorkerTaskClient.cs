using Queuebal.Tasks.Core;
using Queuebal.Tasks.Models;

namespace Queuebal.Tasks.Client;

public class WorkerTaskClient : IWorkerTaskClient
{
    /// <summary>
    /// Indicates whether or not the class has been disposed yet.
    /// </summary>
    private int _disposed = 0;

    ~WorkerTaskClient()
    {
        Dispose(false);
    }

    /// <summary>
    /// Disposes of the unmanaged resources owned by the client.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Gets the next set of available tasks.
    /// </summary>
    /// <param name="count">The maximum number of tasks to retrieve.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskBatch containing the retrieved tasks.</returns>
    public async Task<WorkerTaskBatch?> GetTasks(int count, TimeSpan timeout, CancellationToken cancellationToken) =>
        await Task.FromException<WorkerTaskBatch?>(new NotImplementedException());

    /// <summary>
    /// Gets the next set of available tasks for the specified groupKey.
    /// </summary>
    /// <param name="count">The maximum number of tasks to retrieve.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskBatch containing the retrieved tasks.</returns>
    public async Task<WorkerTaskBatch?> GetTasksForGroup(int count, string groupKey, TimeSpan timeout, CancellationToken cancellationToken) =>
        await Task.FromException<WorkerTaskBatch?>(new NotImplementedException());

    /// <summary>
    /// Marks the tasks in the batch as 'committed' so they will not be processed again.
    /// </summary>
    /// <param name="taskBatch">The batch of tasks to mark as committed.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await completion of the operation.</returns>
    public async Task Commit(WorkerTaskBatchCompleted taskBatch, CancellationToken cancellationToken) =>
        await Task.FromException(new NotImplementedException());

    /// <summary>
    /// Marks the tasks in the batch as 'failed' so they can be processed again.
    /// </summary>
    /// <param name="taskBatchFailure">Data about the failure that occurred, including which tasks to mark as failed.</param>
    /// <param name="retryAfter">
    /// A timespan indicating how long to wait before processing tasks for the
    /// batch's GroupKey again. If not provided, the failure counter for the GroupKey
    /// will be used to calculate an exponential backoff to use for the group. The
    /// retryAfter parameter is for use in case of a rate limit error returned by the processor.
    /// </param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await completion of the operation.</returns>
    public async Task Fail(WorkerTaskBatchFailure taskBatchFailure, TimeSpan? retryAfter, CancellationToken cancellationToken) =>
        await Task.FromException(new NotImplementedException());

    /// <summary>
    /// Writes a single worker task to the task queue.
    /// </summary>
    /// <param name="request">The request containing the worker task to publish.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskPublishInfo object containing data about the successful publish operation.</returns>
    /// <remarks>This method should throw an exception if the publish operation fails.</remarks>
    public async Task<WorkerTaskPublishInfo> Publish(WorkerTaskPublishRequest request, CancellationToken cancellationToken) =>
        await Task.FromException<WorkerTaskPublishInfo>(new NotImplementedException());

    /// <summary>
    /// Writes a batch of worker tasks to the task queue.
    /// </summary>
    /// <param name="request">The request containing the batch of worker tasks to publish.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskPublishInfo object containing data about the successful publish operation.</returns>
    /// <remarks>This method should throw an exception if the publish operation fails.</remarks>
    public async Task<WorkerTaskBatchPublishInfo> Publish(WorkerTaskBatchPublishRequest request, CancellationToken cancellationToken) =>
        await Task.FromException<WorkerTaskBatchPublishInfo>(new NotImplementedException());

    private void Dispose(bool disposing)
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
        {
            return;
        }

        // TODO: close our connection
    }
}

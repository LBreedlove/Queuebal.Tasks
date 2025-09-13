using Queuebal.Tasks.Models;

namespace Queuebal.Tasks.Core;


/// <summary>
/// An interface to be implemented by a worker task client that consumes
/// from the task queue.
/// </summary>
public interface IWorkerTaskConsumer
{
    /// <summary>
    /// Gets the next set of available tasks.
    /// </summary>
    /// <param name="count">The maximum number of tasks to retrieve.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskBatch containing the retrieved tasks.</returns>
    Task<WorkerTaskBatch?> GetTasks(int count, TimeSpan timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the next set of available tasks for the specified groupKey.
    /// </summary>
    /// <param name="count">The maximum number of tasks to retrieve.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskBatch containing the retrieved tasks.</returns>
    Task<WorkerTaskBatch?> GetTasksForGroup(int count, string groupKey, TimeSpan timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Marks the tasks in the batch as 'committed' so they will not be processed again.
    /// </summary>
    /// <param name="taskBatch">The batch of tasks to mark as committed.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A Task used to await completion of the operation.</returns>
    Task Commit(WorkerTaskBatchCompleted taskBatch, CancellationToken cancellationToken);

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
    Task Fail(WorkerTaskBatchFailure taskBatchFailure, TimeSpan? retryAfter, CancellationToken cancellationToken);
}

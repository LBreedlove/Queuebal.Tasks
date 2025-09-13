using Queuebal.Tasks.Models;


namespace Queuebal.Tasks.Core;

public class WorkerTaskPublishInfo { }
public class WorkerTaskBatchPublishInfo { }


/// <summary>
/// An interface to be implemented by a worker task client that publishes
/// to the task queue.
/// </summary>
public interface IWorkerTaskPublisher
{
    /// <summary>
    /// Writes a single worker task to the task queue.
    /// </summary>
    /// <param name="request">The request containing the worker task to publish.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskPublishInfo object containing data about the successful publish operation.</returns>
    /// <remarks>This method should throw an exception if the publish operation fails.</remarks>
    Task<WorkerTaskPublishInfo> Publish(WorkerTaskPublishRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a batch of worker tasks to the task queue.
    /// </summary>
    /// <param name="request">The request containing the batch of worker tasks to publish.</param>
    /// <param name="cancellationToken">A CancellationToken used to cancel the async operation.</param>
    /// <returns>A WorkerTaskPublishInfo object containing data about the successful publish operation.</returns>
    /// <remarks>This method should throw an exception if the publish operation fails.</remarks>
    Task<WorkerTaskBatchPublishInfo> Publish(WorkerTaskBatchPublishRequest request, CancellationToken cancellationToken);
}

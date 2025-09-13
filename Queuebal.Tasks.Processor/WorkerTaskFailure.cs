namespace Queuebal.Tasks.Processor;


/// <summary>
/// Represents a failed task within a worker task batch.
/// </summary>
public class WorkerTaskFailure
{
    /// <summary>
    /// The ID of the failed worker task.
    /// </summary>
    public required long FailedTaskId { get; set; }

    /// <summary>
    /// An error message that describes the failure.
    /// </summary>
    public required string ErrorMessage { get; set; }

    /// <summary>
    /// An optional error code that represents the error that occurred.
    /// </summary>
    public int? ErrorCode { get; set; }

    /// <summary>
    /// An optional duration after which the failed group key should be retried.
    /// If null, the task should be retried immediately.
    /// </summary>
    public TimeSpan? RetryAfter { get; set; }

    /// <summary>
    /// Indicates if the failed task should be retried.
    /// If false, the task will be marked as failed and won't be retried.
    /// </summary>
    public bool RetryFailedTask { get; set; } = true;
}
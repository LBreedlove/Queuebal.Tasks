using MessagePack;

namespace Queuebal.Tasks.Models;


/// <summary>
/// Represents a batch of tasks for a GroupKey.
/// </summary>
[MessagePackObject]
public class WorkerTaskBatchFailure
{
    /// <summary>
    /// The key of the group to which the tasks belong.
    /// </summary>
    [Key(0)]
    public required string GroupKey { get; set; }

    /// <summary>
    /// The error that occurred when processing the task.
    /// </summary>
    [Key(1)]
    public required string ErrorMessage { get; set; }

    /// <summary>
    /// An optional error code that represents the error that occurred
    /// when processing the task.
    /// </summary>
    [Key(2)]
    public int? ErrorCode { get; set; }

    /// <summary>
    /// A string identifier for the processor that attempted to process the task.
    /// The ProcessorId is opaque to the task framework, and is intended to be
    /// useful for diagnostics, in the context of the processor implementation.
    /// </summary>
    [Key(3)]
    public required string ProcessorId { get; set; }

    /// <summary>
    /// The ID of the worker task that failed.
    /// </summary>
    [Key(4)]
    public required long FailedWorkerTaskId { get; set; }

    /// <summary>
    /// The ID's of the worker tasks included in the batch that weren't processed
    /// due to the failed worker task.
    /// </summary>
    [Key(5)]
    public required List<long> RemainingWorkerTaskIds { get; set; }
}

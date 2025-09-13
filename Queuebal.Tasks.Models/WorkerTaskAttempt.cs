using MessagePack;

namespace Queuebal.Tasks.Models;

/// <summary>
/// Represents an attempt to process a worker task.
/// </summary>
[MessagePackObject]
public class WorkerTaskAttempt
{
    /// <summary>
    /// The ID of the worker task that was attempted.
    /// </summary>
    [Key(0)]
    public required long WorkerTaskId { get; set; }

    /// <summary>
    /// The DateTime the task was dequeued for processing.
    /// </summary>
    [Key(1)]
    public required DateTime DequeuedAt { get; set; }

    /// <summary>
    /// The DateTime the task processing completed, whether
    /// successful or not.
    /// </summary>
    [Key(2)]
    public required DateTime CompletedAt { get; set; }

    /// <summary>
    /// Indicates if the processing of the task was successful or not.
    /// </summary>
    [Key(3)]
    public required bool Success { get; set; }

    /// <summary>
    /// The error that occurred when processing the task, if any.
    /// </summary>
    [Key(4)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// An optional error code that represents the error that occurred
    /// when processing the task, if any.
    /// </summary>
    [Key(5)]
    public int? ErrorCode { get; set; }

    /// <summary>
    /// A string identifier for the processor that attempted to process the task.
    /// The ProcessorId is opaque to the task framework, and is intended to be
    /// useful for diagnostics, in the context of the processor implementation.
    /// </summary>
    [Key(6)]
    public required string ProcessorId { get; set; }
}

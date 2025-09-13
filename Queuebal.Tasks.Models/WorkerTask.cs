using MessagePack;

namespace Queuebal.Tasks.Models;

/// <summary>
/// Represents a task to be processed in the Queuebal task processing system.
/// </summary>
[MessagePackObject]
public class WorkerTask
{
    /// <summary>
    /// The unique ID of the task.
    /// </summary>
    [Key(0)]
    public required long WorkerTaskId { get; set; }

    /// <summary>
    /// An idempotency key for the task.
    /// </summary>
    [Key(1)]
    public required string IdempotencyKey { get; set; }

    /// <summary>
    /// The DateTime the task was added to the queue.
    /// </summary>
    [Key(2)]
    public required DateTime EnqueuedUTC { get; set; }

    /// <summary>
    /// Indicates how many times the task has been attempted before
    /// the current attempt. If this is the first attempt to process the task,
    /// Attempts will be 0.
    /// </summary>
    [Key(3)]
    public required int Attempts { get; set; }

    /// <summary>
    /// A value that can be used, in conjunction with 'TaskType' to determine how the 'data'
    /// field should be deserialized.
    /// </summary>
    [Key(4)]
    public string? TaskDomain { get; set; }

    /// <summary>
    /// A value that can be used to determine how the 'data' field should be deserialized.
    /// </summary>
    [Key(5)]
    public required string TaskType { get; set; }

    /// <summary>
    /// The serialized task data.
    /// </summary>
    [Key(6)]
    public required byte[] Data { get; set; }
}

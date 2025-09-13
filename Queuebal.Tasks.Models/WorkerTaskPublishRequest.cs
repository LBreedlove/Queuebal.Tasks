using MessagePack;

namespace Queuebal.Tasks.Models;


/// <summary>
/// Represents a task to be processed in the Queuebal task processing system.
/// </summary>
[MessagePackObject]
public class WorkerTaskPublishRequest
{
    /// <summary>
    /// The key of the group to which the task belongs.
    /// </summary>
    [Key(0)]
    public required string GroupKey { get; set; }

    /// <summary>
    /// An idempotency key for the task.
    /// </summary>
    [Key(1)]
    public required string IdempotencyKey { get; set; }

    /// <summary>
    /// A value that can be used, in conjunction with 'TaskType' to determine how the 'data'
    /// field should be deserialized.
    /// </summary>
    [Key(2)]
    public string? TaskDomain { get; set; }

    /// <summary>
    /// A value that can be used to determine how the 'data' field should be deserialized.
    /// </summary>
    [Key(3)]
    public required string TaskType { get; set; }

    /// <summary>
    /// The serialized task data.
    /// </summary>
    [Key(4)]
    public required byte[] Data { get; set; }
}

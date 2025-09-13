using MessagePack;


namespace Queuebal.Tasks.Models;

/// <summary>
/// Represents an individual task inside a WorkerTaskBatchPublishRequest.
/// </summary>
[MessagePackObject]
public class WorkerTaskBatchTask
{
    /// <summary>
    /// A value that can be used, in conjunction with 'TaskType' to determine how the 'data'
    /// field should be deserialized.
    /// </summary>
    [Key(0)]
    public string? TaskDomain { get; set; }

    /// <summary>
    /// A value that can be used to determine how the 'data' field should be deserialized.
    /// </summary>
    [Key(1)]
    public required string TaskType { get; set; }

    /// <summary>
    /// The serialized task data.
    /// </summary>
    [Key(2)]
    public required byte[] Data { get; set; }
}

using MessagePack;

namespace Queuebal.Tasks.Models;


/// <summary>
/// Represents a request to enqueue a batch of tasks for a given GroupKey.
/// </summary>
[MessagePackObject]
public class WorkerTaskBatchPublishRequest
{
    /// <summary>
    /// The key of the group to which the task belongs.
    /// </summary>
    [Key(0)]
    public required string GroupKey { get; set; }

    /// <summary>
    /// The tasks to publish for the GroupKey.
    /// </summary>
    [Key(1)]
    public required IEnumerable<WorkerTaskBatchTask> Tasks { get; set; }
}

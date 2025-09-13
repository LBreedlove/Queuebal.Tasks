using MessagePack;

namespace Queuebal.Tasks.Models;

/// <summary>
/// Represents a batch of tasks for a GroupKey.
/// </summary>
[MessagePackObject]
public class WorkerTaskBatch
{
    /// <summary>
    /// The key of the group to which the tasks belong.
    /// </summary>
    [Key(0)]
    public required string GroupKey { get; set; }

    /// <summary>
    /// The tasks included in the batch.
    /// </summary>
    [Key(1)]
    public required IEnumerable<WorkerTask> Tasks { get; set; }

    /// <summary>
    /// Gets an empty worker task batch.
    /// </summary>
    /// <returns>An empty worker task batch.</returns>
    public static WorkerTaskBatch Empty(string groupKey = "") => new WorkerTaskBatch
    {
        GroupKey = groupKey,
        Tasks = Array.Empty<WorkerTask>(),
    };
}

using MessagePack;

namespace Queuebal.Tasks.Models;


/// <summary>
/// Represents a batch of tasks for a GroupKey.
/// </summary>
[MessagePackObject]
public class WorkerTaskBatchCompleted
{
    /// <summary>
    /// The key of the group to which the tasks belong.
    /// </summary>
    [Key(0)]
    public required string GroupKey { get; set; }

    /// <summary>
    /// A string identifier for the processor that attempted to process the task.
    /// The ProcessorId is opaque to the task framework, and is intended to be
    /// useful for diagnostics, in the context of the processor implementation.
    /// </summary>
    [Key(1)]
    public required string ProcessorId { get; set; }

    /// <summary>
    /// The ID's of the worker tasks included in the batch that were processed.
    /// </summary>
    [Key(2)]
    public required IEnumerable<long> CompletedWorkerTaskIds { get; set; }
}

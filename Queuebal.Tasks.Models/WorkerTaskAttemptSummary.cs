using MessagePack;

namespace Queuebal.Tasks.Models;

[MessagePackObject]
public class WorkerTaskAttemptSummary
{
    /// <summary>
    /// The WorkerTaskAttempt data for the first time the task was
    /// attempted.
    /// </summary>
    [Key(0)]
    public required WorkerTaskAttempt FirstAttempt { get; set; }

    /// <summary>
    /// The WorkerTaskAttempt data for the last time the task was
    /// attempted.
    /// </summary>
    [Key(1)]
    public required WorkerTaskAttempt LastAttempt { get; set; }

    /// <summary>
    /// The number of times the task was attempted.
    /// </summary>
    [Key(2)]
    public required int Attempts { get; set; }

    /// <summary>
    /// The total amount of time spent processing the task.
    /// </summary>
    [Key(3)]
    public required TimeSpan TotalProcessingTime { get; set; }

    /// <summary>
    /// The total amount of time the task was in the queue, waiting to be processed.
    /// </summary>
    [Key(4)]
    public required TimeSpan TotalWaitTime { get; set; }

    /// <summary>
    /// Indicates if the task was successfully processed.
    /// </summary>
    [IgnoreMember]
    public bool Success => LastAttempt.Success;
}

namespace Queuebal.Tasks.Processor;


public class WorkerTaskProcessorConfiguration
{
    /// <summary>
    /// The maximum number of tasks the processor should take per batch.
    /// </summary>
    public int MaxTasksPerBatch { get; set; } = 100;

    /// <summary>
    /// The maximum amount of time the processor should wait for new tasks.
    /// </summary>
    public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The amount of time the processor should sleep after encountering an error
    /// calling the Consumer.GetTasks method.
    /// </summary>
    public TimeSpan SleepTimeAfterConsumerError { get; set; } = TimeSpan.FromSeconds(5);
}

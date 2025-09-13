using Queuebal.Tasks.Models;

namespace Queuebal.Tasks.Processor;


public class WorkerTaskProcessorException : Exception
{
    public WorkerTaskProcessorException(WorkerTaskBatch batch, long failedTaskId, string errorMessage, int? errorCode, TimeSpan? retryAfter, bool retryFailedTask = true)
        : base(errorMessage)
    {
        WorkerTaskBatch = batch;
        FailedTaskId = failedTaskId;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        RetryAfter = retryAfter;
        RetryFailedTask = retryFailedTask;
    }

    public WorkerTaskBatch WorkerTaskBatch { get; }
    public long FailedTaskId { get; }
    public string ErrorMessage { get; }
    public int? ErrorCode { get; }
    public TimeSpan? RetryAfter { get; }
    public bool RetryFailedTask { get; }
}
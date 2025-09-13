namespace Queuebal.Tasks.Core;

public interface IIdempotencyChecker
{
    /// <summary>
    /// Checks if a task with the given idempotency key has already been processed.
    /// </summary>
    /// <param name="idempotencyKey">The idempotency key of the task.</param>
    /// <returns>True if the task has already been processed; otherwise, false.</returns>
    Task<bool> IsDuplicateAsync(string idempotencyKey, CancellationToken cancellationToken);
}

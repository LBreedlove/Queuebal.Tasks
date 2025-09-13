namespace Queuebal.Tasks.Core;

// TODO: IMPLEMENT
public class WorkerTaskClientConfiguration { }
public class WorkerTaskConnectionInformation { }

public interface IWorkerTaskClient : IWorkerTaskConsumer, IWorkerTaskPublisher, IDisposable
{
    // The interface is defined by the interfaces it's derived from.
}

/// <summary>
/// The interface to be implemented by clients of the WorkerTask service.
/// </summary>
public interface IWorkerTaskClientFactory
{
    /// <summary>
    /// Attempts to connect to the worker task cluster using the provided configuration.
    /// </summary>
    /// <param name="configuration">The configuration for the client.</param>
    /// <returns>An IWorkerTaskClient instance that can be used to communicate with the cluster.</returns>
    Task<IWorkerTaskClient> Create(WorkerTaskClientConfiguration configuration);
}

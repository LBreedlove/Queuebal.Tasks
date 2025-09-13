using Moq;
using Queuebal.Tasks.Core;
using Queuebal.Tasks.Models;
using Queuebal.Tasks.Processor;

namespace Queuebal.UnitTests.Tasks.Processor;


[TestClass]
public class TestProcessorBase
{
    internal class FakeProcessor : ProcessorBase
    {
        private readonly long? _failedTaskId;

        public FakeProcessor(IWorkerTaskConsumer consumer, WorkerTaskProcessorConfiguration configuration, long? failedTaskId = null)
            : base(consumer, configuration, "fake_processor_id")
        {
            _failedTaskId = failedTaskId;
        }

        protected override async Task ProcessTaskBatch(WorkerTaskBatch batch, CancellationToken cancellationToken)
        {
            if (!_failedTaskId.HasValue)
            {
                await Task.FromResult(true);
                return;
            }

            if (_failedTaskId.Value == -1)
            {
                throw new Exception("Fail the whole batch");
            }

            throw new WorkerTaskProcessorException
            (
                batch,
                _failedTaskId.Value,
                "Testing failure",
                10060,
                TimeSpan.FromSeconds(30)
            );
        }
    }

    [TestMethod]
    public async Task test_run_when_cancellation_requested_does_not_call_consumer()
    {
        var consumerMock = new Mock<IWorkerTaskConsumer>();
        var config = new WorkerTaskProcessorConfiguration { MaxTasksPerBatch = 100, WaitTimeout = TimeSpan.FromSeconds(5) };

        var processor = new FakeProcessor(consumerMock.Object, config);
        var cancellationToken = new CancellationToken(true);

        await processor.Run(cancellationToken);
        consumerMock.Verify(mock => mock.GetTasks(It.IsAny<int>(), It.IsAny<TimeSpan>(), cancellationToken), Times.Never);
    }

    [TestMethod]
    public async Task test_run_when_processor_throws_generic_exception_fails_entire_batch()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var workerTaskBatch = new WorkerTaskBatch
        {
            GroupKey = "test_group_key",
            Tasks = new List<WorkerTask>
            {
                new WorkerTask
                {
                    WorkerTaskId = 123,
                    IdempotencyKey = "test_group_key:123",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                },
                new WorkerTask
                {
                    WorkerTaskId = 234,
                    IdempotencyKey = "test_group_key:234",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                }
            }
        };

        var consumerMock = new Mock<IWorkerTaskConsumer>();
        consumerMock.SetupSequence
        (
            o => o.GetTasks(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()).Result
        )
        .Returns(workerTaskBatch)
        .Returns(() => { cancellationTokenSource.Cancel(); return WorkerTaskBatch.Empty(); });

        var config = new WorkerTaskProcessorConfiguration { MaxTasksPerBatch = 100, WaitTimeout = TimeSpan.FromSeconds(5) };

        var processor = new FakeProcessor(consumerMock.Object, config, failedTaskId: -1);

        await processor.Run(cancellationToken);

        // we should call Commit for the first task, and Fail for the second.
        consumerMock.Verify(o => o.Commit(It.IsAny<WorkerTaskBatchCompleted>(), It.IsAny<CancellationToken>()), Times.Never);
        consumerMock.Verify(o => o.Fail(It.IsAny<WorkerTaskBatchFailure>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task test_run_when_processor_fails_task_splits_batch_into_completed_and_failed()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var workerTaskBatch = new WorkerTaskBatch
        {
            GroupKey = "test_group_key",
            Tasks = new List<WorkerTask>
            {
                new WorkerTask
                {
                    WorkerTaskId = 123,
                    IdempotencyKey = "test_group_key:123",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                },
                new WorkerTask
                {
                    WorkerTaskId = 234,
                    IdempotencyKey = "test_group_key:234",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                }
            }
        };

        var consumerMock = new Mock<IWorkerTaskConsumer>();
        consumerMock.SetupSequence
        (
            o => o.GetTasks(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()).Result
        )
        .Returns(workerTaskBatch)
        .Returns(() => { cancellationTokenSource.Cancel(); return WorkerTaskBatch.Empty(); });

        var config = new WorkerTaskProcessorConfiguration { MaxTasksPerBatch = 100, WaitTimeout = TimeSpan.FromSeconds(5) };

        var processor = new FakeProcessor(consumerMock.Object, config, failedTaskId: 234);

        await processor.Run(cancellationToken);

        // we should call Commit for the first task, and Fail for the second.
        consumerMock.Verify(o => o.Commit(It.IsAny<WorkerTaskBatchCompleted>(), It.IsAny<CancellationToken>()), Times.Once);
        consumerMock.Verify(o => o.Fail(It.IsAny<WorkerTaskBatchFailure>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task test_run_when_processor_succeeds_on_success_called()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var workerTaskBatch = new WorkerTaskBatch
        {
            GroupKey = "test_group_key",
            Tasks = new List<WorkerTask>
            {
                new WorkerTask
                {
                    WorkerTaskId = 123,
                    IdempotencyKey = "test_group_key:123",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                },
                new WorkerTask
                {
                    WorkerTaskId = 234,
                    IdempotencyKey = "test_group_key:234",
                    EnqueuedUTC = DateTime.UtcNow,
                    Attempts = 0,
                    TaskDomain = "test_domain",
                    TaskType = "test_task_type",
                    Data = Array.Empty<byte>(),
                }
            }
        };

        var consumerMock = new Mock<IWorkerTaskConsumer>();
        consumerMock.SetupSequence
        (
            o => o.GetTasks(It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()).Result
        )
        .Returns(workerTaskBatch)
        .Returns(() => { cancellationTokenSource.Cancel(); return WorkerTaskBatch.Empty(); });

        var config = new WorkerTaskProcessorConfiguration { MaxTasksPerBatch = 100, WaitTimeout = TimeSpan.FromSeconds(5) };

        var processor = new FakeProcessor(consumerMock.Object, config);

        await processor.Run(cancellationToken);

        consumerMock.Verify(o => o.Commit(It.IsAny<WorkerTaskBatchCompleted>(), It.IsAny<CancellationToken>()));
    }
}

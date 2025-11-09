using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Shouldly;

namespace RhythmCodex.Infrastructure.Tasks;

[TestFixture]
[NonParallelizable]
public class TaskRunIntegrationTests : BaseIntegrationFixture
{
    private class ProcessMock
    {
        public List<object> EndedInvocations { get; } = [];
        public List<object> CancelledInvocations { get; } = [];
        public List<Exception> ErroredInvocations { get; } = [];
        public List<ResultStub> SucceededInvocations { get; } = [];
        public List<TaskProgress> ProgressInvocations { get; } = [];

        public ITaskSpec<ConfigurationStub, ResultStub> Spec { get; }
        public ITaskProcess<ResultStub> Process { get; }
        public ConfigurationStub Configuration { get; }
        public ResultStub? Result { get; private set; }
        public TaskStatus TaskStatus { get; private set; }

        public ProcessMock(
            ITaskSpecFactory factory,
            Action<ITaskProcess<ResultStub>> beforeWait,
            Action<ResultStub> process
        )
        {
            Configuration = new ConfigurationStub();

            Spec = factory.CreateSpec<ConfigurationStub, ResultStub>((ctx, cfg) =>
            {
                var result = new ResultStub
                {
                    Configuration = cfg,
                    Context = ctx
                };

                process(result);
                return result;
            });

            Process = Spec.CreateProcess(Configuration);

            Process.Ended += () => EndedInvocations.Add(new { });
            Process.Cancelled += () => CancelledInvocations.Add(new { });
            Process.Errored += exception => ErroredInvocations.Add(exception);
            Process.Succeeded += result => SucceededInvocations.Add(result);
            Process.Progressed += progress => ProgressInvocations.Add(progress);

            var task = Process.RunAsync().ContinueWith(t =>
            {
                TaskStatus = t.Status;
                if (!t.IsFaulted)
                    Result = t.Result;
                return t;
            });

            beforeWait(Process);

            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                // Expected
            }
        }
    }

    private class ConfigurationStub;

    private class ResultStub
    {
        public object? Configuration { get; init; }
        public ITaskContext? Context { get; init; }
    }

    private ProcessMock RunProcessStub(
        Action<ITaskProcess<ResultStub>>? beforeWait = null,
        Action<ResultStub>? process = null
    )
    {
        var factory = Resolve<ITaskSpecFactory>();
        return new ProcessMock(factory, beforeWait ?? (_ => { }), process ?? (_ => { }));
    }

    [Test]
    public void RunAsync_PassesCorrectConfiguration()
    {
        var stub = RunProcessStub();
        stub.Result!.Configuration.ShouldBe(stub.Configuration);
    }

    [Test]
    public void RunAsync_RaisesErrored_WhenErrored()
    {
        var expectedException = new Exception();
        var stub = RunProcessStub(null, _ => throw expectedException);
        stub.EndedInvocations.Count.ShouldBe(1);
        stub.ErroredInvocations.Count.ShouldBe(1);
        stub.ErroredInvocations[0].ShouldBeOfType(typeof(AggregateException));
        ((AggregateException)stub.ErroredInvocations[0]).InnerExceptions.ToArray().ShouldBe([expectedException]);
    }

    [Test]
    public void RunAsync_RaisesEnded_WhenEnded()
    {
        var stub = RunProcessStub();
        stub.EndedInvocations.Count.ShouldBe(1);
    }

    [Test]
    public void RunAsync_RaisesSucceeded_WhenSucceeded()
    {
        var stub = RunProcessStub();
        stub.SucceededInvocations.Count.ShouldBe(1);
    }

    [Test]
    public void RunAsync_RaisesCancelled_WhenCancelled()
    {
        var stub = RunProcessStub(p => { p.Cancel(); }, _ => { Thread.Sleep(100); });
        stub.CancelledInvocations.Count.ShouldBe(1);
    }

    [Test]
    public void RunAsync_RaisesProgress_WhenProgressIsReported()
    {
        var expectedProgress = Build<TaskProgress>().Without(x => x.SubProgress).Create();
        var stub = RunProcessStub(null, r => { r.Context!.ReportProgress(expectedProgress); });
        stub.ProgressInvocations.Count.ShouldBe(1);
        stub.ProgressInvocations[0].ShouldBe(expectedProgress);
    }
}
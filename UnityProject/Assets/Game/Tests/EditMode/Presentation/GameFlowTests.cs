using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Orquvia.Starter.Presentation;
using Orquvia.Starter.Tests.Fakes;
using Orquvia.Starter.UseCase;

namespace Orquvia.Starter.Tests.Presentation
{
    public sealed class GameFlowTests
    {
        private SynchronizationContext _originalContext;
        private FakeSceneNavigator _navigator;
        private FakeGameLogger _logger;
        private GameFlow _flow;

        [SetUp]
        public void SetUp()
        {
            // UnitySynchronizationContext があると await の継続が次フレームに回されるため、
            // テスト中は外して継続を同期的に実行させる。
            _originalContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);

            _navigator = new FakeSceneNavigator();
            _logger = new FakeGameLogger();
            _flow = new GameFlow(_navigator, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            SynchronizationContext.SetSynchronizationContext(_originalContext);
        }

        [Test]
        public void RequestStartGame_LoadsGameScene()
        {
            _flow.RequestStartGame();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Game }));
        }

        [Test]
        public void RequestResult_LoadsResultScene()
        {
            _flow.RequestResult();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Result }));
        }

        [Test]
        public void RequestTitle_LoadsTitleScene()
        {
            _flow.RequestTitle();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Title }));
        }

        [Test]
        public void Request_Succeeded_LogsInfo()
        {
            _flow.RequestStartGame();

            Assert.That(_logger.Infos, Has.Count.EqualTo(1));
            Assert.That(_logger.Infos[0], Does.Contain(nameof(SceneId.Game)));
            Assert.That(_logger.Errors, Is.Empty);
        }

        [Test]
        public void Request_WhileTransitioning_IsIgnored()
        {
            var pending = new TaskCompletionSource<bool>();
            _navigator.Handler = (_, _) => pending.Task;

            _flow.RequestStartGame();
            _flow.RequestResult();
            _flow.RequestTitle();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Game }));
        }

        [Test]
        public void Request_AfterTransitionCompleted_IsAccepted()
        {
            var pending = new TaskCompletionSource<bool>();
            _navigator.Handler = (_, _) => pending.Task;

            _flow.RequestStartGame();
            pending.SetResult(true);
            _navigator.Handler = (_, _) => Task.CompletedTask;
            _flow.RequestResult();

            Assert.That(_navigator.Requests, Is.EqualTo(new[] { SceneId.Game, SceneId.Result }));
        }

        [Test]
        public void Request_NavigatorFails_LogsErrorAndAllowsNextRequest()
        {
            var failure = new InvalidOperationException("load failed");
            _navigator.Handler = (_, _) => Task.FromException(failure);

            _flow.RequestStartGame();

            Assert.That(_logger.Errors, Has.Count.EqualTo(1));
            Assert.That(_logger.Errors[0].Exception, Is.SameAs(failure));
            Assert.That(_logger.Infos, Is.Empty);

            _navigator.Handler = (_, _) => Task.CompletedTask;
            _flow.RequestStartGame();

            Assert.That(_navigator.Requests, Has.Count.EqualTo(2));
        }

        [Test]
        public void Request_NavigatorThrowsSynchronously_LogsError()
        {
            var failure = new InvalidOperationException("load failed");
            _navigator.Handler = (_, _) => throw failure;

            _flow.RequestStartGame();

            Assert.That(_logger.Errors, Has.Count.EqualTo(1));
            Assert.That(_logger.Errors[0].Exception, Is.SameAs(failure));
        }

        [Test]
        public void Request_CanceledWithoutDispose_LogsError()
        {
            // GameFlow 自身が破棄されていないキャンセルは想定外なのでエラー扱いになる
            _navigator.Handler = (_, _) => Task.FromException(new OperationCanceledException());

            _flow.RequestStartGame();

            Assert.That(_logger.Errors, Has.Count.EqualTo(1));
            Assert.That(_logger.Errors[0].Exception, Is.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public void Dispose_WhileTransitioning_CancelsWaitWithoutError()
        {
            var pending = new TaskCompletionSource<bool>();
            CancellationToken received = default;
            _navigator.Handler = (_, ct) =>
            {
                received = ct;
                ct.Register(() => pending.TrySetCanceled(ct));
                return pending.Task;
            };

            _flow.RequestStartGame();
            _flow.Dispose();

            Assert.That(received.IsCancellationRequested, Is.True);
            Assert.That(pending.Task.IsCanceled, Is.True);
            Assert.That(_logger.Errors, Is.Empty);
            Assert.That(_logger.Infos, Is.Empty);
        }

        [Test]
        public void Request_AfterDispose_IsIgnored()
        {
            _flow.Dispose();

            _flow.RequestStartGame();
            _flow.RequestResult();
            _flow.RequestTitle();

            Assert.That(_navigator.Requests, Is.Empty);
            Assert.That(_logger.Infos, Is.Empty);
            Assert.That(_logger.Warnings, Is.Empty);
            Assert.That(_logger.Errors, Is.Empty);
        }

        [Test]
        public void Dispose_Twice_DoesNotThrow()
        {
            _flow.Dispose();

            Assert.DoesNotThrow(() => _flow.Dispose());
        }
    }
}

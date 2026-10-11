using System;
using System.Collections.Generic;
using Orquvia.Starter.UseCase;

namespace Orquvia.Starter.Tests.Fakes
{
    public sealed class FakeGameLogger : IGameLogger
    {
        public List<string> Infos { get; } = new();
        public List<string> Warnings { get; } = new();
        public List<(string Message, Exception Exception)> Errors { get; } = new();

        public void Info(string message) => Infos.Add(message);
        public void Warning(string message) => Warnings.Add(message);
        public void Error(string message, Exception exception) => Errors.Add((message, exception));
    }
}

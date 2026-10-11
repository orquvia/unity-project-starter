using System;

namespace Orquvia.Starter.UseCase
{
    public interface IGameLogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception exception);
    }
}

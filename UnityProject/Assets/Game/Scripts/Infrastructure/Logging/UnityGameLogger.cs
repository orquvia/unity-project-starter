using System;
using Orquvia.Starter.UseCase;
using PlasticPipe.PlasticProtocol.Client.Proxies;
using UnityEngine;

namespace Qrquvia.Starter.Infrastructure
{
    public sealed class UnityGameLogger : IGameLogger
    {
        public void Info(string message) => UnityEngine.Debug.Log(message);
        public void Warning(string message) => Debug.LogWarning(message);
        public void Error(string message, Exception ex)
        {
            Debug.LogError(message);

            if (ex != null)
            {
                Debug.LogException(ex);
            }
        }
    }
}

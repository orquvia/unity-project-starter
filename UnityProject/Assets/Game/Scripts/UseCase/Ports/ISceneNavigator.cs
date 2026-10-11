using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Orquvia.Starter.UseCase
{
    public enum SceneId
    {
        Title,
        Game,
        Result
    }

    public interface ISceneNavigator
    {
        Task LoadAsync(
            SceneId sceneId,
            CancellationToken ct);
    }
}

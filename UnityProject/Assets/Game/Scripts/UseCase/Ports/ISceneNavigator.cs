using System.Threading;
using System.Threading.Tasks;

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
        // NOTE: UseCase層からUniTaskへの依存を分離するため、Taskを使用する
        /// <param name="ct">
        /// ロード完了の待機を打ち切るためのトークン。
        /// キャンセルしても、開始済みのシーンロード自体は中断されない。
        /// </param>
        Task LoadAsync(
            SceneId sceneId,
            CancellationToken ct);
    }
}

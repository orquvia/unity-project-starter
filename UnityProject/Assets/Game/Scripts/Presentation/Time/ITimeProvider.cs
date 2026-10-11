namespace Orquvia.Starter.Presentation
{
    // フレームの経過時間の取得元。テストで時間を任意に進められるように抽象化している。
    public interface ITimeProvider
    {
        float DeltaTime { get; }
    }
}

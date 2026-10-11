namespace Orquvia.Starter.Domain
{
    public interface IReadOnlyGameSession
    {
        int Score { get; }
        float RemainingSeconds { get; }
        GameStatus Status { get; }
    }
}

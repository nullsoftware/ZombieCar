namespace ZombieCar.Gameplay.Level
{
    public enum LevelOutcome
    {
        InProgress,
        Won,
        Lost,
    }

    /// <summary>
    /// Win/lose rules, kept apart from the game flow that reacts to them.
    /// </summary>
    public interface ILevelOutcome
    {
        LevelOutcome Evaluate();
    }
}

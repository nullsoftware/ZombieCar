using ZombieCar.Gameplay.Vehicles;

namespace ZombieCar.Gameplay.Level
{
    /// <summary>
    /// Lose when the car is destroyed, win when it reaches the end of the track.
    /// </summary>
    public sealed class LevelOutcomeEvaluator : ILevelOutcome
    {
        private readonly Car _car;
        private readonly LevelTrack _track;

        public LevelOutcomeEvaluator(Car car, LevelTrack track)
        {
            _car = car;
            _track = track;
        }

        public LevelOutcome Evaluate()
        {
            if (!_car.IsAlive)
            {
                return LevelOutcome.Lost;
            }

            return _track.IsFinishReached(_car.Position) ? LevelOutcome.Won : LevelOutcome.InProgress;
        }
    }
}

using VContainer.Unity;

namespace ZombieCar.Gameplay.Level
{
    /// <summary>
    /// Places the finish line marker at the end of the track configured in the level config.
    /// </summary>
    public sealed class FinishLinePresenter : IInitializable
    {
        private readonly LevelTrack _track;
        private readonly LevelView _view;

        public FinishLinePresenter(LevelTrack track, LevelView view)
        {
            _track = track;
            _view = view;
        }

        public void Initialize()
        {
            if (_view.FinishLine != null)
            {
                _view.FinishLine.SetPositionAndRotation(_track.FinishPosition, _track.StartPose.rotation);
            }
        }
    }
}

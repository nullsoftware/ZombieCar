namespace ZombieCar.Cameras
{
    public interface ICameraDirector
    {
        /// <summary>
        /// Cuts straight to the intro camera behind the car, with no blend or damping.
        /// </summary>
        void CutToIntro();

        void BlendToGameplay();

        void Shake(float force);
    }
}

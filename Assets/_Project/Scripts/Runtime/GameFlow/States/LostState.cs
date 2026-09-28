using ZombieCar.Configs;
using ZombieCar.Controls;

namespace ZombieCar.GameFlow.States
{
    public sealed class LostState : GameOverState
    {
        public LostState(ITapInput tapInput, GameFlowConfig config) : base(tapInput, config)
        {
        }

        public override GameStateId Id => GameStateId.Lost;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace ZombieCar.Gameplay.Enemies.Spawning
{
    /// <summary>
    /// Decides where enemies stand at the start of a level.
    /// Swap the implementation for hand-placed spawn points, waves from data, etc.
    /// </summary>
    public interface IEnemySpawnLayout
    {
        void Build(List<Pose> results);
    }
}

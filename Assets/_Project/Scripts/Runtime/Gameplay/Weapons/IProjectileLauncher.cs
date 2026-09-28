using UnityEngine;

namespace ZombieCar.Gameplay.Weapons
{
    public interface IProjectileLauncher
    {
        void Launch(Vector3 origin, Vector3 direction);
    }
}

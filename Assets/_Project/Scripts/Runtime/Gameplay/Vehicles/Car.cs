using System;
using UnityEngine;
using VContainer;
using ZombieCar.Configs;
using ZombieCar.Core.Combat;

namespace ZombieCar.Gameplay.Vehicles
{
    /// <summary>
    /// The player's vehicle: kinematic body, health and wheel visuals.
    /// Movement decisions live in <see cref="CarController"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Car : MonoBehaviour, ITargetable
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _bodyCollider;

        [Header("Wheels (optional)")]
        [SerializeField] private Transform[] _wheels = Array.Empty<Transform>();
        [SerializeField, Min(0.01f)] private float _wheelRadius = 0.36f;

        private Health _health;

        public IReadOnlyHealth Health => _health;
        public bool IsAlive => _health.IsAlive;
        public Vector3 Position => transform.position;

        [Inject]
        public void Construct(CarConfig config)
        {
            _health = new Health(config.MaxHealth);
        }

        public void TakeDamage(float amount) => _health.TakeDamage(amount);

        public Vector3 GetClosestPoint(Vector3 point) => _bodyCollider.ClosestPoint(point);

        public void RestoreHealth() => _health.Restore();

        public void Teleport(Pose pose)
        {
            _rigidbody.position = pose.position;
            _rigidbody.rotation = pose.rotation;
            transform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        /// <summary>
        /// Moves the car forward along its own facing. Call from FixedUpdate.
        /// </summary>
        public void Drive(float distance)
        {
            Vector3 forward = _rigidbody.rotation * Vector3.forward;
            _rigidbody.MovePosition(_rigidbody.position + forward * distance);
            SpinWheels(distance);
        }

        private void SpinWheels(float distance)
        {
            float angle = distance / _wheelRadius * Mathf.Rad2Deg;

            foreach (Transform wheel in _wheels)
            {
                wheel.Rotate(angle, 0f, 0f, Space.Self);
            }
        }

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bodyCollider = GetComponent<Collider>();
        }
    }
}

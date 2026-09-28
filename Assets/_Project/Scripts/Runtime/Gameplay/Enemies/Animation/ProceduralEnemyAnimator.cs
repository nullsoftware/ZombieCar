using UnityEngine;

namespace ZombieCar.Gameplay.Enemies.Animation
{
    /// <summary>
    /// Placeholder animation for a model without a rig: bobbing run, forward lunge on attack,
    /// flinch on hit, and falling backwards on death. It only moves the visual child transform.
    /// It is a fallback: once a <see cref="MecanimEnemyAnimator"/> on the same enemy has clips, that one is used.
    /// </summary>
    public sealed class ProceduralEnemyAnimator : EnemyAnimator
    {
        private enum Motion
        {
            Idle,
            Run,
            Attack,
            Death,
        }

        [SerializeField] private Transform _visual;

        [Header("Run")]
        [SerializeField] private float _runBobHeight = 0.12f;
        [SerializeField] private float _runBobFrequency = 9f;
        [SerializeField] private float _runLeanAngle = 12f;

        [Header("Attack")]
        [SerializeField] private float _attackLungeAngle = 30f;
        [SerializeField, Min(0.01f)] private float _attackDuration = 0.4f;

        [Header("Hit")]
        [SerializeField] private float _hitFlinchAngle = 15f;
        [SerializeField, Min(0.01f)] private float _hitDuration = 0.15f;

        [Header("Death")]
        [SerializeField, Min(0.01f)] private float _deathFallDuration = 0.45f;

        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private Motion _motion;
        private float _motionTime;
        private float _hitTimeLeft;

        public override bool IsAvailable => true;

        public override bool IsFallback => true;

        private void Awake()
        {
            if (_visual == null)
            {
                _visual = transform;
            }

            _basePosition = _visual.localPosition;
            _baseRotation = _visual.localRotation;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            _motionTime += deltaTime;
            _hitTimeLeft = Mathf.Max(0f, _hitTimeLeft - deltaTime);

            EvaluatePose(out float height, out float pitch);
            pitch -= _hitFlinchAngle * (_hitTimeLeft / _hitDuration);

            _visual.localPosition = _basePosition + Vector3.up * height;
            _visual.localRotation = _baseRotation * Quaternion.Euler(pitch, 0f, 0f);
        }

        public override void ResetPose()
        {
            SetMotion(Motion.Idle);
            _hitTimeLeft = 0f;

            if (_visual != null)
            {
                _visual.localPosition = _basePosition;
                _visual.localRotation = _baseRotation;
            }
        }

        public override void PlayIdle() => SetMotion(Motion.Idle);

        public override void PlayRun() => SetMotion(Motion.Run);

        public override void PlayAttack() => SetMotion(Motion.Attack);

        public override void PlayHit() => _hitTimeLeft = _hitDuration;

        public override void PlayDeath()
        {
            SetMotion(Motion.Death);
            _hitTimeLeft = 0f;
        }

        private void SetMotion(Motion motion)
        {
            _motion = motion;
            _motionTime = 0f;
        }

        private void EvaluatePose(out float height, out float pitch)
        {
            switch (_motion)
            {
                case Motion.Run:
                    height = Mathf.Abs(Mathf.Sin(_motionTime * _runBobFrequency)) * _runBobHeight;
                    pitch = _runLeanAngle;
                    break;
                case Motion.Attack:
                    float attack = Mathf.Clamp01(_motionTime / _attackDuration);
                    height = 0f;
                    pitch = Mathf.Sin(attack * Mathf.PI) * _attackLungeAngle;
                    break;
                case Motion.Death:
                    float fall = Mathf.Clamp01(_motionTime / _deathFallDuration);
                    height = 0f;
                    pitch = -90f * (1f - (1f - fall) * (1f - fall));
                    break;
                default:
                    height = Mathf.Sin(_motionTime * 2f) * 0.015f;
                    pitch = 0f;
                    break;
            }
        }
    }
}

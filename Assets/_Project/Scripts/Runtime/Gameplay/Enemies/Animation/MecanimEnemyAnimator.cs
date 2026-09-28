using UnityEngine;

namespace ZombieCar.Gameplay.Enemies.Animation
{
    /// <summary>
    /// Plays Mixamo (or any other) clips through an Animator Controller.
    /// It cross-fades to states by name, so the controller only needs the states and no transitions:
    /// Idle, Run, Attack and Death. An optional "Hit" trigger parameter is fired on non-lethal hits.
    /// Missing states or parameters are skipped quietly.
    /// </summary>
    public sealed class MecanimEnemyAnimator : EnemyAnimator
    {
        [SerializeField] private Animator _animator;
        [SerializeField, Min(0f)] private float _crossFadeDuration = 0.15f;
        [SerializeField, Min(0)] private int _layer;

        [Header("States")]
        [SerializeField] private string _idleState = "Idle";
        [SerializeField] private string _runState = "Run";
        [SerializeField] private string _attackState = "Attack";
        [SerializeField] private string _deathState = "Death";

        [Header("Parameters (optional)")]
        [SerializeField] private string _hitTrigger = "Hit";

        [Header("Root Motion")]
        [Tooltip("Pins the root bone horizontally, so clips with baked root motion (e.g. Mixamo 'Run Forward' " +
                 "without 'In Place') don't run ahead of the enemy and snap back each loop. The AI moves the enemy.")]
        [SerializeField] private bool _keepRootBoneInPlace = true;
        [Tooltip("Hips / root bone of the rig. Found automatically when empty (humanoid Hips or a bone named '*Hips').")]
        [SerializeField] private Transform _rootBone;

        private int _idleHash;
        private int _runHash;
        private int _attackHash;
        private int _deathHash;
        private int _hitTriggerHash;
        private bool _hasHitTrigger;
        private Vector3 _rootBoneRestPosition;

        // Available once the controller actually has clips assigned to its states.
        public override bool IsAvailable =>
            _animator != null &&
            _animator.runtimeAnimatorController != null &&
            _animator.runtimeAnimatorController.animationClips.Length > 0;

        private void Awake()
        {
            _idleHash = Animator.StringToHash(_idleState);
            _runHash = Animator.StringToHash(_runState);
            _attackHash = Animator.StringToHash(_attackState);
            _deathHash = Animator.StringToHash(_deathState);
            _hitTriggerHash = Animator.StringToHash(_hitTrigger);
            _hasHitTrigger = HasTriggerParameter(_hitTriggerHash);

            if (_rootBone == null)
            {
                _rootBone = FindRootBone();
            }

            if (_rootBone != null && _animator != null)
            {
                _rootBoneRestPosition = _animator.transform.InverseTransformPoint(_rootBone.position);
            }
        }

        // Runs after the Animator has written this frame's pose.
        private void LateUpdate()
        {
            if (!_keepRootBoneInPlace || _rootBone == null || _animator == null)
            {
                return;
            }

            // Work in the Animator's space: it stays upright whatever rotations the rig hierarchy has.
            Transform space = _animator.transform;
            Vector3 animated = space.InverseTransformPoint(_rootBone.position);
            _rootBone.position = space.TransformPoint(new Vector3(_rootBoneRestPosition.x, animated.y, _rootBoneRestPosition.z));
        }

        public override void ResetPose()
        {
            if (!IsReady)
            {
                return;
            }

            _animator.Rebind();
            _animator.Update(0f);
        }

        public override void PlayIdle() => CrossFade(_idleHash);

        public override void PlayRun() => CrossFade(_runHash);

        // Called on every swing: a one-shot attack clip restarts, a looping one just keeps playing.
        public override void PlayAttack() => CrossFade(_attackHash);

        public override void PlayHit()
        {
            if (IsReady && _hasHitTrigger)
            {
                _animator.SetTrigger(_hitTriggerHash);
            }
        }

        public override void PlayDeath() => CrossFade(_deathHash);

        private bool IsReady =>
            _animator != null && _animator.isActiveAndEnabled && _animator.runtimeAnimatorController != null;

        private void CrossFade(int stateHash)
        {
            if (!IsReady || !_animator.HasState(_layer, stateHash) || IsLoopingAlready(stateHash))
            {
                return;
            }

            _animator.CrossFadeInFixedTime(stateHash, _crossFadeDuration, _layer, 0f);
        }

        // Restarting a looping clip would cut it short, so let it keep going.
        private bool IsLoopingAlready(int stateHash)
        {
            if (_animator.IsInTransition(_layer))
            {
                return _animator.GetNextAnimatorStateInfo(_layer).shortNameHash == stateHash;
            }

            AnimatorStateInfo current = _animator.GetCurrentAnimatorStateInfo(_layer);
            return current.shortNameHash == stateHash && current.loop;
        }

        private bool HasTriggerParameter(int hash)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
            {
                return false;
            }

            foreach (AnimatorControllerParameter parameter in _animator.parameters)
            {
                if (parameter.nameHash == hash && parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    return true;
                }
            }

            return false;
        }

        private Transform FindRootBone()
        {
            if (_animator == null)
            {
                return null;
            }

            if (_animator.isHuman)
            {
                return _animator.GetBoneTransform(HumanBodyBones.Hips);
            }

            foreach (Transform bone in _animator.GetComponentsInChildren<Transform>(true))
            {
                if (bone.name.EndsWith("Hips"))
                {
                    return bone;
                }
            }

            return null;
        }

        private void Reset() => _animator = GetComponentInChildren<Animator>();
    }
}

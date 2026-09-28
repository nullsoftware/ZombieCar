using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace ZombieCar.Controls
{
    /// <summary>
    /// Mouse, pen and touch input through the generic &lt;Pointer&gt; device (Input System).
    /// </summary>
    public sealed class PointerInputService : ITapInput, IAimInput, IInitializable, IDisposable
    {
        private readonly InputAction _pressAction =
            new("PointerPress", InputActionType.Button, "<Pointer>/press");

        private readonly InputAction _deltaAction =
            new("PointerDelta", InputActionType.PassThrough, "<Pointer>/delta", expectedControlType: "Vector2");

        private int _lastConsumedTapFrame = -1;

        public bool IsAiming => _pressAction.IsPressed();

        public Vector2 AimDelta => IsAiming
            ? _deltaAction.ReadValue<Vector2>() / Mathf.Max(1, Screen.width)
            : Vector2.zero;

        public void Initialize()
        {
            _pressAction.Enable();
            _deltaAction.Enable();
        }

        public async UniTask WaitForTapAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(IsNewTap, cancellationToken: cancellationToken);
            _lastConsumedTapFrame = Time.frameCount;
        }

        public void Dispose()
        {
            _pressAction.Dispose();
            _deltaAction.Dispose();
        }

        // The frame check stops one physical tap from finishing two waits in a row
        // (e.g. "restart" and then immediately "start").
        private bool IsNewTap() =>
            _pressAction.WasPressedThisFrame() && Time.frameCount != _lastConsumedTapFrame;
    }
}

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZombieCar.UI
{
    /// <summary>
    /// A CanvasGroup panel that fades in and out with UniTask. A new fade cancels the one in progress.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class FadeablePanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Min(0f)] private float _fadeDuration = 0.25f;

        private CancellationTokenSource _fadeCancellation;

        public void SetVisible(bool isVisible, bool instant = false)
        {
            CancelFade();
            _canvasGroup.interactable = isVisible;
            _canvasGroup.blocksRaycasts = isVisible;

            float targetAlpha = isVisible ? 1f : 0f;

            if (instant || _fadeDuration <= 0f)
            {
                _canvasGroup.alpha = targetAlpha;
                return;
            }

            _fadeCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            FadeAsync(targetAlpha, _fadeCancellation.Token).Forget();
        }

        private async UniTaskVoid FadeAsync(float targetAlpha, CancellationToken cancellationToken)
        {
            float startAlpha = _canvasGroup.alpha;

            for (float elapsed = 0f; elapsed < _fadeDuration; elapsed += Time.unscaledDeltaTime)
            {
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);

                if (await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
                {
                    return;
                }
            }

            _canvasGroup.alpha = targetAlpha;
        }

        private void CancelFade()
        {
            if (_fadeCancellation == null)
            {
                return;
            }

            _fadeCancellation.Cancel();
            _fadeCancellation.Dispose();
            _fadeCancellation = null;
        }

        private void OnDestroy() => CancelFade();

        private void Reset() => _canvasGroup = GetComponent<CanvasGroup>();
    }
}

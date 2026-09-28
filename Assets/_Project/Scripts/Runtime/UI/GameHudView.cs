using UnityEngine;
using UnityEngine.UI;

namespace ZombieCar.UI
{
    public enum HudScreen
    {
        None,
        Start,
        Win,
        Lose,
    }

    [DisallowMultipleComponent]
    public sealed class GameHudView : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private FadeablePanel _hud;
        [SerializeField] private Image _healthFill;
        [SerializeField] private Image _progressFill;

        [Header("Overlays")]
        [SerializeField] private FadeablePanel _startScreen;
        [SerializeField] private FadeablePanel _winScreen;
        [SerializeField] private FadeablePanel _loseScreen;

        public void ShowScreen(HudScreen screen)
        {
            _startScreen.SetVisible(screen == HudScreen.Start);
            _winScreen.SetVisible(screen == HudScreen.Win);
            _loseScreen.SetVisible(screen == HudScreen.Lose);
        }

        public void SetHudVisible(bool isVisible) => _hud.SetVisible(isVisible);

        public void SetHealth(float normalized) => _healthFill.fillAmount = normalized;

        public void SetProgress(float normalized) => _progressFill.fillAmount = normalized;
    }
}

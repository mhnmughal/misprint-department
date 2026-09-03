using System;
using UnityEngine;
using UnityEngine.UI;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Maintenance card. Real controls only — sliders drive mixer groups via
    /// AudioController and persist to the local save.
    /// </summary>
    public sealed class SettingsScreen : UIPanel
    {
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider sfxSlider;
        [SerializeField] Slider uiSlider;
        [SerializeField] Toggle hapticsToggle;
        [SerializeField] Button resetButton;
        [SerializeField] Button creditsButton;
        [SerializeField] Button backButton;
        [SerializeField] ConfirmDialog dialog;

        public event Action Back, Credits, ProgressReset;

        bool _loading;

        protected override void Awake()
        {
            base.Awake();
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(v => Apply(() =>
            {
                Services.Save.Data.musicVolume = v; Services.Audio?.SetMusicVolume(v);
            }));
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(v => Apply(() =>
            {
                Services.Save.Data.sfxVolume = v; Services.Audio?.SetSfxVolume(v);
            }));
            if (uiSlider != null) uiSlider.onValueChanged.AddListener(v => Apply(() =>
            {
                Services.Save.Data.uiVolume = v; Services.Audio?.SetUiVolume(v);
            }));
            if (hapticsToggle != null) hapticsToggle.onValueChanged.AddListener(v => Apply(() =>
            {
                Services.Save.Data.hapticsEnabled = v; Services.Audio?.SetHaptics(v);
            }));
            if (resetButton != null) resetButton.onClick.AddListener(AskReset);
            if (creditsButton != null) creditsButton.onClick.AddListener(() => Credits?.Invoke());
            if (backButton != null) backButton.onClick.AddListener(() => { Services.Save?.Save(); Back?.Invoke(); });
        }

        protected override void OnShow() => LoadFromSave();
        protected override void OnHide() => Services.Save?.Save();

        void LoadFromSave()
        {
            var d = Services.Save?.Data;
            if (d == null) return;
            _loading = true;
            if (musicSlider != null) musicSlider.value = d.musicVolume;
            if (sfxSlider != null) sfxSlider.value = d.sfxVolume;
            if (uiSlider != null) uiSlider.value = d.uiVolume;
            if (hapticsToggle != null) hapticsToggle.isOn = d.hapticsEnabled;
            _loading = false;
        }

        void Apply(Action change)
        {
            if (_loading || Services.Save == null) return;
            change();
        }

        void AskReset()
        {
            if (dialog == null) { DoReset(); return; }
            dialog.Ask("Reset all progress?",
                "Every completed job and rating will be cleared. Audio settings are kept.",
                "Reset", DoReset);
        }

        void DoReset()
        {
            Services.Save?.ResetProgress();
            LoadFromSave();
            Services.Audio?.PlaySfx(Sfx.StampMark);
            ProgressReset?.Invoke();
        }
    }
}

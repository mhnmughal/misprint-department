using UnityEngine;
using UnityEngine.Audio;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Owns the AudioMixer and a small pool of one-shot sources. Volume sliders
    /// drive real mixer group levels. Lives on the persistent Bootstrap object.
    /// </summary>
    public sealed class AudioController : MonoBehaviour
    {
        [SerializeField] AudioMixer mixer;
        [SerializeField] SoundBank bank;
        [SerializeField] AudioSource musicSource;

        [Header("Exposed mixer parameters")]
        [SerializeField] string musicParam = "MusicVolume";
        [SerializeField] string sfxParam = "SFXVolume";
        [SerializeField] string uiParam = "UIVolume";
        [SerializeField] AudioMixerGroup sfxGroup;
        [SerializeField] AudioMixerGroup uiGroup;

        const int PoolSize = 8;
        AudioSource[] _pool;
        int _next;
        bool _haptics = true;

        void Awake()
        {
            _pool = new AudioSource[PoolSize];
            for (int i = 0; i < PoolSize; i++)
            {
                var go = new GameObject($"OneShot{i}");
                go.transform.SetParent(transform, false);
                _pool[i] = go.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
            }
        }

        public void ApplyFromSave(SaveData data)
        {
            SetMusicVolume(data.musicVolume);
            SetSfxVolume(data.sfxVolume);
            SetUiVolume(data.uiVolume);
            SetHaptics(data.hapticsEnabled);
            if (bank != null && bank.ambience != null && musicSource != null)
            {
                musicSource.clip = bank.ambience;
                musicSource.loop = true;
                musicSource.Play();
            }
        }

        // 0..1 linear slider -> mixer dB (-80 muted .. 0 full)
        public void SetMusicVolume(float v) => SetParam(musicParam, v);
        public void SetSfxVolume(float v) => SetParam(sfxParam, v);
        public void SetUiVolume(float v) => SetParam(uiParam, v);

        void SetParam(string param, float linear)
        {
            if (mixer == null || string.IsNullOrEmpty(param)) return;
            float dB = linear <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(linear)) * 20f;
            mixer.SetFloat(param, dB);
        }

        public void SetHaptics(bool on) => _haptics = on;

        public void PlayUi(Sfx id) => Play(id);
        public void PlaySfx(Sfx id) => Play(id);

        public void Play(Sfx id)
        {
            if (bank == null || !bank.TryGet(id, out var e)) return;
            var src = _pool[_next];
            _next = (_next + 1) % PoolSize;
            src.outputAudioMixerGroup = e.bus == SfxBus.Ui ? uiGroup : sfxGroup;
            src.volume = e.volume <= 0f ? 1f : e.volume;
            src.PlayOneShot(e.clip);
        }

        public void Vibrate()
        {
            if (!_haptics) return;
#if UNITY_IOS || UNITY_ANDROID
            Handheld.Vibrate();
#endif
        }
    }
}

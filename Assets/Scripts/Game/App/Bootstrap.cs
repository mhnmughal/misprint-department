using UnityEngine;
using UnityEngine.SceneManagement;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Sits on the "Boot" object in the Bootstrap scene. Initializes local
    /// services once, then hands off to MainWorkshop. The GameObject is kept
    /// alive for the session so AudioController persists.
    /// </summary>
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] LevelDatabase levelDatabase;
        [SerializeField] AudioController audioController;
        [SerializeField] float minSplashSeconds = 0.4f;

        static Bootstrap _instance;
        float _elapsed;
        bool _handedOff;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => _instance = null; // survives domain-reload-off play mode

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            if (!Services.Ready)
                Services.Init(levelDatabase, audioController);
        }

        // Update-driven rather than a coroutine so a domain-reload hitch on the
        // first frame can't strand the splash — this always fires.
        void Update()
        {
            if (_handedOff) return;
            if (SceneManager.GetActiveScene().name != SceneNames.Bootstrap) { _handedOff = true; return; }
            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed >= minSplashSeconds)
            {
                _handedOff = true;
                SceneManager.LoadScene(SceneNames.MainWorkshop);
            }
        }
    }
}

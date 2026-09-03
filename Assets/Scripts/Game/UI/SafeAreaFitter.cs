using UnityEngine;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Drives a RectTransform to the device safe area. Put on the "SafeArea"
    /// child of each screen Canvas. Anchors must be 0..1 stretch.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        RectTransform _rt;
        Rect _last;
        ScreenOrientation _lastOrientation;

        void Awake() { _rt = (RectTransform)transform; Apply(); }
        void OnEnable() { Apply(); }

        void Update()
        {
            if (Screen.safeArea != _last || Screen.orientation != _lastOrientation)
                Apply();
        }

        void Apply()
        {
            _last = Screen.safeArea;
            _lastOrientation = Screen.orientation;

            var area = Screen.safeArea;
            var min = area.position;
            var max = area.position + area.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;
            if (float.IsNaN(min.x) || float.IsNaN(max.x)) return;

            _rt.anchorMin = min;
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Renders the working command strip as a row of TokenViews. The container
    /// and a HorizontalLayoutGroup are authored in the scene; token slots are
    /// pooled from one authored prefab (dynamic content, allowed).
    /// </summary>
    public sealed class InstructionStripUI : MonoBehaviour
    {
        [SerializeField] RectTransform content;
        [SerializeField] TokenView tokenPrefab;

        readonly List<TokenView> _views = new List<TokenView>();
        int _selected = -1;

        public event Action<int> TokenClicked;

        public void Render(CommandStrip strip)
        {
            EnsureCount(strip.Count);
            for (int i = 0; i < _views.Count; i++)
            {
                bool used = i < strip.Count;
                _views[i].gameObject.SetActive(used);
                if (used) _views[i].Bind(i, strip[i]);
            }
            _selected = -1;
        }

        public void SetSelected(int index)
        {
            _selected = index;
            for (int i = 0; i < _views.Count; i++)
                _views[i].SetSelected(i == index);
        }

        public void ClearSelection() => SetSelected(-1);

        public void SetProofHighlight(int index)
        {
            for (int i = 0; i < _views.Count; i++)
                _views[i].SetProofHighlight(i == index);
        }

        public void SetInteractable(bool on)
        {
            for (int i = 0; i < _views.Count; i++)
                _views[i].SetInteractable(on);
        }

        public Vector3 TokenScreenPosition(int index)
        {
            if (index < 0 || index >= _views.Count) return Vector3.zero;
            return _views[index].transform.position;
        }

        void EnsureCount(int n)
        {
            while (_views.Count < n)
            {
                var v = Instantiate(tokenPrefab, content);
                v.Clicked += i => TokenClicked?.Invoke(i);
                _views.Add(v);
            }
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// The mechanical RUN PROOF lever. Authored in the scene as a button with a
    /// pivoting handle child; this animates the handle throw on press.
    /// </summary>
    public sealed class ProofControlsUI : MonoBehaviour
    {
        [SerializeField] Button leverButton;
        [SerializeField] RectTransform handle;
        [SerializeField] TMP_Text label;
        [SerializeField] float throwAngle = -32f;
        [SerializeField] float throwSeconds = 0.18f;

        public event Action Fired;

        void Awake()
        {
            if (leverButton != null) leverButton.onClick.AddListener(Throw);
            if (label != null) label.text = "RUN PROOF";
        }

        public void SetInteractable(bool on)
        {
            if (leverButton != null) leverButton.interactable = on;
        }

        void Throw()
        {
            StopAllCoroutines();
            StartCoroutine(ThrowRoutine());
        }

        IEnumerator ThrowRoutine()
        {
            SetInteractable(false);
            if (handle != null)
            {
                float from = 0f, to = throwAngle, t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / throwSeconds;
                    handle.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(from, to, t));
                    yield return null;
                }
            }
            Fired?.Invoke();
            yield return new WaitForSeconds(0.35f);
            if (handle != null)
            {
                float from = throwAngle, to = 0f, t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / (throwSeconds * 1.6f);
                    handle.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(from, to, t));
                    yield return null;
                }
                handle.localRotation = Quaternion.identity;
            }
        }
    }
}

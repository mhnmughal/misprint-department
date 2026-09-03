using UnityEngine;
using TMPro;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game.UI
{
    /// <summary>Shows the deterministic defect rule for the current job.</summary>
    public sealed class DefectCardUI : MonoBehaviour
    {
        [SerializeField] TMP_Text headline;
        [SerializeField] TMP_Text body;

        public void Bind(LevelDefinition level)
        {
            if (headline != null) headline.text = "TODAY'S DEFECT";
            if (body != null)
                body.text = level.defect != null ? level.defect.Describe() : "No defect on this run.";
        }
    }
}

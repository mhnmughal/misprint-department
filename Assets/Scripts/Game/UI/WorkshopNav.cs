using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MisprintDepartment.Game.UI
{
    /// <summary>
    /// Owns MainWorkshop panel navigation. All panels are authored in the scene;
    /// this only shows/hides them and starts the Gameplay scene.
    /// </summary>
    public sealed class WorkshopNav : MonoBehaviour
    {
        [SerializeField] WorkshopScreen workshop;
        [SerializeField] JobSelectScreen jobSelect;
        [SerializeField] SettingsScreen settings;
        [SerializeField] UIPanel manual;
        [SerializeField] UIPanel credits;
        [SerializeField] Button manualBackButton;
        [SerializeField] Button creditsBackButton;

        void Start()
        {
            Services.EnsureForEditor();

            workshop.StartInspection += StartInspection;
            workshop.JobFiles += () => Swap(workshop, jobSelect);
            workshop.Manual += () => Swap(workshop, manual);
            workshop.Settings += () => Swap(workshop, settings);
            workshop.Credits += () => Swap(workshop, credits);

            jobSelect.Back += () => Swap(jobSelect, workshop);
            jobSelect.JobChosen += OpenJob;

            settings.Back += () => Swap(settings, workshop);
            settings.Credits += () => Swap(settings, credits);

            if (manualBackButton != null) manualBackButton.onClick.AddListener(() => Swap(manual, workshop));
            if (creditsBackButton != null) creditsBackButton.onClick.AddListener(() => Swap(credits, workshop));

            workshop.SetInstant(true);
            jobSelect.SetInstant(false);
            settings.SetInstant(false);
            manual.SetInstant(false);
            credits.SetInstant(false);
            workshop.RefreshStatus();
        }

        void Swap(UIPanel from, UIPanel to)
        {
            Services.Audio?.PlayUi(Sfx.MechButton);
            from.Hide();
            to.Show();
        }

        void StartInspection()
        {
            var db = Services.Levels;
            var lvl = db != null ? db.ById(workshop.CurrentJobId) : null;
            if (lvl == null && db != null && db.Count > 0) lvl = db.ByIndex(0);
            if (lvl == null) return;
            GameSession.PendingLevel = lvl;
            Services.Audio?.PlaySfx(Sfx.TicketMove);
            SceneManager.LoadScene(SceneNames.Gameplay);
        }

        void OpenJob(int levelId)
        {
            var lvl = Services.Levels?.ById(levelId);
            if (lvl == null) return;
            GameSession.PendingLevel = lvl;
            Services.Audio?.PlaySfx(Sfx.TicketMove);
            SceneManager.LoadScene(SceneNames.Gameplay);
        }
    }
}

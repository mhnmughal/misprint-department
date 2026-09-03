using System;
using System.Collections.Generic;
using UnityEngine;

namespace MisprintDepartment.Game
{
    /// <summary>Maps Sfx ids to original generated AudioClips.</summary>
    [CreateAssetMenu(menuName = "Misprint/Sound Bank", fileName = "SoundBank")]
    public sealed class SoundBank : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public Sfx id;
            public AudioClip clip;
            public SfxBus bus;
            [Range(0f, 1f)] public float volume;
        }

        public List<Entry> entries = new List<Entry>();
        public AudioClip ambience;

        public bool TryGet(Sfx id, out Entry entry)
        {
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].id == id) { entry = entries[i]; return entry.clip != null; }
            entry = default;
            return false;
        }
    }
}

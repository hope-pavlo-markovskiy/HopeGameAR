using UnityEngine;

[CreateAssetMenu(fileName = "SoundAsset", menuName = "ScriptableObjects/New SoundAsset")]
public class SoundAsset : ScriptableObject
{
    public enum ShapeTrace { BalloonPop }
    public enum RoundUp { Walk, Wrong, Fly, Jump1, Jump2, Jump3, Jump4 }

    [System.Serializable]
    public class Group
    {
        [HideInInspector] public string name;

        public AudioClip[] clips;

        public bool loop;
        public float volume = 1f;
        [Range(0, 256)] public int priority = 128;
        public int max = 7;

        public AudioClip GetRandom() => clips[Random.Range(0, clips.Length)];

        public void ApplyToSource(AudioSource source)
        {
            source.clip = GetRandom();
            source.loop = loop;
            source.volume = volume;
            source.priority = priority;
        }
    }

    public Group[] groups;

    private void OnValidate()
    {
        foreach (var group in groups)
        {
            if (groups.Length > 0)
            {
                group.name = group.clips[0].name;
            }
        }
    }
}
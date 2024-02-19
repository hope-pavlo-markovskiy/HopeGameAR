using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] SoundAsset asset;

    Dictionary<SoundAsset.Group, int> groupCount = new();
    public static Action OnInc, OnDec;

    HashSet<AudioSource> sources = new();

    public class SoundInstance
    {
        public AudioSource source;
        public SoundAsset.Group group;

        public SoundInstance(AudioSource source, SoundAsset.Group group)
        {
            this.source = source;
            this.group = group;
        }
    }

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public SoundInstance TryPlay<T>(T soundName, GameObject emitter = null) where T : Enum
    {
        SoundAsset.Group group = asset.groups[Convert.ToInt32(soundName)];

        groupCount.TryAdd(group, 0);

        if (groupCount[group] + 1 > group.max)
            return null;

        groupCount[group]++;
        OnInc?.Invoke();

        AudioSource source = GetSource();
        group.ApplyToSource(source);

        if (!source.loop)
        {
            StartCoroutine(DecGroupCount(group, source.clip.length));
        }

        source.Play();

        return new(source, group);


        AudioSource GetSource()
        {
            foreach (var s in sources)
            {
                if (!s.isPlaying)
                {
                    return s;
                }
            }

            if (emitter == null)
            {
                emitter = gameObject;
            }
            var newSource = emitter.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            sources.Add(newSource);
            return newSource;
        }
    }

    public void StopInstance(SoundInstance instance)
    {
        if (instance != null)
        {
            instance.source.Stop();
            StartCoroutine(DecGroupCount(instance.group));
        }
    }

    IEnumerator DecGroupCount(SoundAsset.Group group, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        OnDec?.Invoke();

        groupCount[group]--;
    }
}
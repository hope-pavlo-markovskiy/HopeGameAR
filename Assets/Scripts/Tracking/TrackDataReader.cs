using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackDataReader : MonoBehaviour
{
    public enum Tags { HandLm, HandHandedness, HandGesture }

    public Dictionary<Tags, Action<List<string>>> Datasets { get; private set; }

    public static TrackDataReader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Datasets = new();
        foreach (Tags tag in Enum.GetValues(typeof(Tags)))
        {
            Datasets.Add(tag, default);
        }
    }

    private void Start()
    {
        TrackDataReceiver.Instance.OnReceived += ReadDatasets;
        //HandMappedFileReader.Instance.OnReceived += ReadDatasets;
    }

    int startI, endI;
    Tags tag;
    HashSet<Tags> seenTags = new();
    List<string> dataset = new();
    int dataCount;
    int datasetI;
    void ReadDatasets(string str)
    {
        startI = 0;
        endI = 0;
        seenTags.Clear();
        // For each dataset
        while ((startI = str.IndexOf('(', endI)) != -1)
        {
            // Get tag
            startI++;
            endI = str.IndexOf(',', startI) - 1;
            tag = (Tags)int.Parse(SubstringSelect()); // Tag no
            // Only check each tag once
            if (seenTags.Contains(tag))
                break;
            else
                seenTags.Add(tag);

            // Get dataset
            startI = endI + 3 + 1;
            dataset.Clear();
            dataCount = 1;
            datasetI = startI;
            do
            {
                int nextOpen = str.IndexOf('[', datasetI),
                    nextClose = str.IndexOf(']', datasetI);

                if (nextClose < nextOpen || nextOpen == -1)
                {
                    datasetI = nextClose;
                    dataCount--;
                }
                else if (nextOpen < nextClose)
                {
                    datasetI = nextOpen;
                    dataCount++;
                }

                if (dataCount == 1)
                {
                    endI = datasetI;
                    dataset.Add(SubstringSelect());
                    startI = str.IndexOf('[', datasetI);
                }

                datasetI++;
            }
            while (dataCount != 0);

            // Broadcast updated dataset
            Datasets[tag]?.Invoke(dataset);


            string SubstringSelect() => str.Substring(startI, endI - startI + 1);
        }
    }
}
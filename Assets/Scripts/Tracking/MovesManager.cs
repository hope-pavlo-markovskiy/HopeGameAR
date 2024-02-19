using System.Collections.Generic;
using UnityEngine;
using Hand = HandReader.Hand;

public class MovesManager : MonoBehaviour
{
    HashSet<MoveAsset> moveset = new();

    public static MovesManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HandReader.Instance.OnHandsRefresh += ReadInput;
    }

    public void Add(MoveAsset moveAsset)
    {
        moveset.Add(moveAsset);

        foreach (var move in moveset)
        {
            move.Initialise();
        }
    }

    void ReadInput(List<Hand> hands)
    {
        if (hands.Count == 0)
        {
            foreach (var move in moveset)
            {
                move.Deactivate();
            }
            return;
        }

        foreach (var move in moveset)
        {
            foreach (var hand in hands)
            {
                move.TryActivate(hand);
            }
        }
    }
}
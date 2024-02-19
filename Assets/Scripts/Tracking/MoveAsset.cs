using System;
using System.Collections.Generic;
using UnityEngine;
using Hand = HandReader.Hand;

[CreateAssetMenu(fileName = "MoveAsset", menuName = "ScriptableObjects/New MoveAsset")]
public class MoveAsset : ScriptableObject
{
    [SerializeField] MoveAssetCondition[] conditions;

    bool active;

    public event Action<Hand> OnEnterUpdate;
    public event Action OnExit;

    public void Initialise()
    {
        foreach (var condition in conditions)
        {
            condition.Initialise();
        }
    }

    public void TryActivate(Hand hand)
    {
        foreach (var condition in conditions)
        {
            if (!condition.IsMet(hand))
            {
                Deactivate();
                return;
            }
        }

        active = true;
        OnEnterUpdate?.Invoke(hand);
    }

    public void Deactivate()
    {
        if (active)
        {
            active = false;
            OnExit?.Invoke();
        }
    }

    public MoveAssetCondition GetCondition(Type type)
    {
        foreach (var condition in conditions)
        {
            if (condition.GetType() == type)
            {
                Debug.Log(type);
                return condition;
            }
        }
        return null;
    }

    private void OnValidate()
    {
        Initialise();
    }
}
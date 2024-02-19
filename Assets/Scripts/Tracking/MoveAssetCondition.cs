using UnityEngine;
using Hand = HandReader.Hand;

public abstract class MoveAssetCondition : ScriptableObject
{
    public virtual void Initialise() { }

    public abstract bool IsMet(Hand hand);

    protected virtual void OnValidate()
    {
        Initialise();
    }
}
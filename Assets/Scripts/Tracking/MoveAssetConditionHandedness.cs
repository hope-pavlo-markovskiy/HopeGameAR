using UnityEngine;
using Hand = HandReader.Hand;

[CreateAssetMenu(fileName = "Handedness Condition", menuName = "ScriptableObjects/Move Asset Conditions/Handedness")]
public class MoveAssetConditionHandedness : MoveAssetCondition
{
    public Hand._Handedness handedness;

    public override bool IsMet(Hand hand) => hand.Handedness == handedness;
}
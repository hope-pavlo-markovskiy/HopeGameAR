using UnityEngine;
using Hand = HandReader.Hand;

[CreateAssetMenu(fileName = "Gesture Condition", menuName = "ScriptableObjects/Move Asset Conditions/Gesture")]
public class MoveAssetConditionGesture : MoveAssetCondition
{
    public Hand.Gestures gesture;

    public override bool IsMet(Hand hand) => hand.Gesture == gesture;
}
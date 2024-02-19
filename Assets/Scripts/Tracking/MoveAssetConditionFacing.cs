using UnityEngine;
using Hand = HandReader.Hand;

[CreateAssetMenu(fileName = "Facing Condition", menuName = "ScriptableObjects/Move Asset Conditions/Facing")]
public class MoveAssetConditionFacing : MoveAssetCondition
{
    public Hand._Facing facing;

    public override bool IsMet(Hand hand) => hand.Facing == facing;
}
using UnityEngine;
using Hand = HandReader.Hand;

[CreateAssetMenu(fileName = "Rotation Condition", menuName = "ScriptableObjects/Move Asset Conditions/Rotation")]
public class MoveAssetConditionRotation : MoveAssetCondition
{
    [SerializeField] Vector3 rotation;
    [SerializeField, Range(0f, 1f)] float rotationAccuracy;
    [SerializeField] Vector3 direction;
    public enum TargetOrientation { Wrist, Up, Right, Forward, Pointer }
    [SerializeField] TargetOrientation targetOrientation;

    public float LastAccuracy { get; private set; }

    public override void Initialise()
    {
        Vector3 adjustedRotation = new(rotation.x, rotation.y, rotation.z + 90f);
        direction = (Quaternion.Euler(adjustedRotation) * Vector3.right).normalized;
    }

    public override bool IsMet(Hand hand)
    {
        var targetDirection = targetOrientation switch
        {
            TargetOrientation.Wrist     => hand.WristOrientation.Direction,
            TargetOrientation.Up        => hand.UpOrientation.Direction,
            TargetOrientation.Right     => hand.RightOrientation.Direction,
            TargetOrientation.Forward   => hand.ForwardOrientation.Direction,
            TargetOrientation.Pointer   => hand.PointerOrientation.Direction,
            _ => default,
        };

        LastAccuracy = Vector3.Dot(targetDirection, direction);

        return LastAccuracy >= rotationAccuracy;
    }
}
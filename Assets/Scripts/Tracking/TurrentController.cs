using UnityEngine;
using Hand = HandReader.Hand;

public class TurrentController : MonoBehaviour
{
    [SerializeField] Transform turrent;

    Vector3 dir;
    Vector3 dirCurrentVelocity;
    [SerializeField] float dirSmoothTime;
    Vector3 smoothDir;

    [SerializeField, Range(0f, 1f)] float dirDeadZone;

    [SerializeField] MoveAsset pointMove;

    MovesManager movesManager;

    private void Awake()
    {
        pointMove.OnEnterUpdate += hands =>
        {
            UpdateRotation(hands);
            SetTurrentActive(true);
        };
        pointMove.OnExit += () => { SetTurrentActive(false); };
    }

    private void Start()
    {
        movesManager = MovesManager.Instance;
        movesManager.Add(pointMove);
    }

    private void Update()
    {
        smoothDir = Vector3.SmoothDamp(
            smoothDir, dir, ref dirCurrentVelocity, dirSmoothTime);
        if (smoothDir != Vector3.zero)
        {
            turrent.rotation = Quaternion.LookRotation(smoothDir);
        }
    }

    void UpdateRotation(Hand hand)
    {
        if (Vector3.Dot(dir, hand.PointerOrientation.Direction) >= 1f - dirDeadZone)
            return;
        dir = hand.PointerOrientation.Direction;
    }

    void SetTurrentActive(bool value)
    {
        turrent.gameObject.SetActive(value);
    }
}
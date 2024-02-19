using UnityEngine;

public class AimController : MonoBehaviour
{
    [SerializeField] MoveAsset aimLeftMove, aimRightMove;

    MovesManager movesManager;

    private void Awake()
    {
        aimLeftMove.OnEnterUpdate += AimLeft;
        aimRightMove.OnEnterUpdate += AimRight;
    }

    private void Start()
    {
        movesManager = MovesManager.Instance;
        movesManager.Add(aimLeftMove);
        movesManager.Add(aimRightMove);
    }

    void AimLeft(HandReader.Hand hand)
    {
        print("aim left");
    }

    void AimRight(HandReader.Hand hand)
    {
        print("aim right");
    }
}
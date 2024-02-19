using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField] MoveAsset grabMove;

    [SerializeField] LayerMask targetLayerMask;

    MovesManager movesManager;

    [SerializeField] float borderMultiplier;

    [SerializeField] Transform humanSegPos;

    public float depthBoundsMultiplier;

    Camera cam;

    private void Awake()
    {
        grabMove.OnEnterUpdate += Grab;
    }

    private void Start()
    {
        movesManager = MovesManager.Instance;
        movesManager.Add(grabMove);

        cam = Camera.main;
    }

    void Grab(HandReader.Hand hand)
    {
        Ray ray = cam.ScreenPointToRay(cam.WorldToScreenPoint(hand.Bounds.center));
        ray.origin += Vector3.forward * humanSegPos.position.z * depthBoundsMultiplier;
        if (Physics.BoxCast(ray.origin, hand.Bounds.extents, ray.direction, out RaycastHit hit, Quaternion.identity, 1000, targetLayerMask))
        {
            Destroy(hit.transform.gameObject);
        }
    }
}
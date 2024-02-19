using UnityEngine;

public class ScreenUtility : MonoBehaviour
{
    //public const float PROCESSED_WIDTH = 1920, PROCESSED_HEIGHT = 1080;
    public const float PROCESSED_WIDTH = 640, PROCESSED_HEIGHT = 480;

    [SerializeField] Canvas canvas;

    Camera cam;

    [SerializeField] Transform humanSegT;

    [SerializeField] bool applyOverlayPos, applyOverlayRot, applyOverlayScale;

    public static ScreenUtility Instance { get; private set; }

    public void Awake()
    {
        Instance = this;

        cam = Camera.main;
    }

    public Vector3 ScreenToOverlayPos(Vector3 screenPos)
    {
        screenPos.y = PROCESSED_HEIGHT - screenPos.y; // Flip y

        float depth = screenPos.z;

        Vector3 viewPort = new(
            screenPos.x / PROCESSED_WIDTH,
            screenPos.y / PROCESSED_HEIGHT,
            depth);

        viewPort *= canvas.scaleFactor;

        viewPort.z += cam.nearClipPlane;

        Ray ray = cam.ViewportPointToRay(viewPort);
        Vector3 worldPos = ray.origin + ray.direction * (-ray.origin.z / ray.direction.z);
        worldPos += depth * Vector3.Distance(ray.origin, worldPos) * ray.direction;

        // Selective Transform Point
        var worldToLocalMatrix = Matrix4x4.TRS(
            applyOverlayPos ? humanSegT.position : Vector3.one,
            applyOverlayRot ? humanSegT.rotation : Quaternion.identity,
            applyOverlayScale ? humanSegT.localScale : Vector3.one);
        return worldToLocalMatrix.MultiplyPoint3x4(worldPos);
    }
}
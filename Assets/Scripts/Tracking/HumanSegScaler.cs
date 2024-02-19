using UnityEngine;

public class HumanSegScaler : MonoBehaviour
{
    [SerializeField] Transform aspectScale;

    Camera cam;
    float lastScreenWidth, lastScreenHeight;

    public static HumanSegScaler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        cam = Camera.main;

        UpdateTransform();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateTransform();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    public void UpdateTransform()
    {
        float
            startX = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 10f)).x,
            endX = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 10f)).x;
        float
            startY = cam.ViewportToWorldPoint(new Vector3(0.5f, 0, 10f)).y,
            endY = cam.ViewportToWorldPoint(new Vector3(0.5f, 1, 10f)).y;

        aspectScale.localScale = new Vector3(
            endX - startX,
            endY - startY, 1f);
    }
}
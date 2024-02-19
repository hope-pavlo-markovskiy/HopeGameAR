using UnityEngine;
using UnityEngine.UI;

public class CamDisplay : MonoBehaviour
{
    const int
        WIDTH = 1920,
        HEIGHT = 1080;

    [SerializeField] RawImage img;
    [SerializeField] Material mat;
    MeshRenderer rend;

    Texture2D tex2d;

    CamFeedReceiver feedReceiver;

    //MaterialPropertyBlock propertyBlock;

    public static CamDisplay Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        tex2d = new(WIDTH, HEIGHT);

        //propertyBlock = new MaterialPropertyBlock();

        //rend = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        feedReceiver = CamFeedReceiver.Instance;
        feedReceiver.OnReceived += Refresh;
    }

    void Refresh(byte[] frameBytes)
    {
        tex2d.LoadImage(frameBytes); // Deserialise data
        //img.texture = tex2d;
        mat.mainTexture = tex2d;

        //propertyBlock.main//SetTexture("Input Texture", tex2d);
        //rend.SetPropertyBlock(propertyBlock);
    }
}
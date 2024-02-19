using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;

public class MemoryMappedFileReader : MonoBehaviour
{
    public RawImage rawImage;
    int width = 640;
    int height = 480;
    int bytesPerPixel = 3;
    int imageSize;
    FileStream fileStream;
    Texture2D tex;

    async void Start()
    {
        imageSize = width * height * bytesPerPixel;
        tex = new Texture2D(width, height);

        await Task.Run(() => ReadMemoryMappedFileRepeatedly());
    }

    async Task ReadMemoryMappedFileRepeatedly()
    {
        string path = $"{Application.dataPath}\\Python\\App\\video_capture.bin";
        while (true)
        {
            using (fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] fileData = new byte[imageSize];
                int bytesRead = await fileStream.ReadAsync(fileData, 0, imageSize);

                if (bytesRead == imageSize)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(
                        () => {
                            tex.LoadImage(fileData);
                            rawImage.texture = tex;
                        });

                }
            }
            await Task.Delay(33); // Delay for approximately 30 fps
        }
    }

    void OnApplicationQuit()
    {
        if (fileStream != null)
        {
            fileStream.Close();
            fileStream.Dispose();
        }

        if (tex != null)
        {
            Destroy(tex);
        }
    }
}

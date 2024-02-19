using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Text;
using System;

public class HandMappedFileReader : MonoBehaviour
{
    FileStream fileStream;

    public event Action<string> OnReceived;

    public static HandMappedFileReader Instance;

    private void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        await Task.Run(() => ReadMemoryMappedFileRepeatedly());
    }

    async Task ReadMemoryMappedFileRepeatedly()
    {
        string path = $"{Application.dataPath}\\Python\\App\\hand_lm.bin";
        while (true)
        {
            using (fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] data = new byte[256564];
                int bytesRead = await fileStream.ReadAsync(data, 0, data.Length);
                string text = Encoding.Default.GetString(data);

                if (text != "")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(
                    () => {
                        OnReceived?.Invoke(text);
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
    }
}

using System.Diagnostics;
using UnityEngine;

public class PythonProcessor : MonoBehaviour
{
    // Path must be consistent per device
    //const string INTERPRETER_PATH = @"C:\Users\Mark\AppData\Local\Programs\Python\Python311\python.exe";
    //const string INTERPRETER_PATH = @"C:\Program Files\Python311\python.exe";
    const string INTERPRETER_PATH = @"C:\Users\Mark\AppData\Local\Programs\Python\Python311\python.exe";
    
    [System.Serializable]
    public struct ScriptReference
    {
        public string name;
        public string directory;
    }

    [SerializeField] ScriptReference[] scriptRefs;
    [SerializeField] int scriptIndex;

    Process process;

    public static PythonProcessor Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //StartProgram();
    }

    public void StartProgram()
    {
        UnityEngine.Debug.Log("Starting Program");

        ScriptReference scriptRef = scriptRefs[scriptIndex];

        // Innit start info
        ProcessStartInfo startInfo = new();
        startInfo.FileName = INTERPRETER_PATH;

        // Assign directory
        string location = $"{Application.dataPath}\\Python\\{scriptRef.directory}";
        startInfo.Arguments = $"{location}\\{scriptRef.name}.py";
        startInfo.WorkingDirectory = location;

        // Settings
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;

        // Innit process
        process = new();
        process.StartInfo = startInfo;

        // Log Python console
        process.OutputDataReceived +=
            (sender, args) => UnityEngine.Debug.Log("Python Output: " + args.Data);
        process.ErrorDataReceived +=
            (sender, args) => UnityEngine.Debug.LogError("Python Error: " + args.Data);

        // Start process
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    // Terminate process and cleanup
    void EndProcess()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            process.Dispose();
        }
    }
    private void OnDisable() => EndProcess();
    private void OnDestroy() => EndProcess();
    private void OnApplicationQuit() => EndProcess();
}

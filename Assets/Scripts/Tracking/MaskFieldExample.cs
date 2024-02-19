using UnityEngine;
using UnityEditor;

public class MaskFieldExample : EditorWindow
{
    static int flags = 0;
    static string[] options = new string[] { "CanJump", "CanShoot", "CanSwim" };

    [MenuItem("Examples/Mask Field usage")]
    static void Init()
    {
        MaskFieldExample window = (MaskFieldExample)GetWindow(typeof(MaskFieldExample));
        window.Show();
    }

    void OnGUI()
    {
        flags = EditorGUILayout.MaskField("Player Flags", flags, options);

        // Display the flags in disabled toggles
        GUI.enabled = false;
        for (var i = 0; i < options.Length; i++)
        {
            var value = (flags & (1 << i)) != 0;
            EditorGUILayout.Toggle(options[i], value);
        }
        GUI.enabled = true;
    }
}
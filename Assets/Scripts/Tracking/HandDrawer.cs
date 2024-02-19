using UnityEditor;
using UnityEngine;
using LmTag = HandReader.Hand.LmTag;

public class HandDrawer : MonoBehaviour
{
    float lineThickness;

    HandReader reader;

    private void Start()
    {
        reader = HandReader.Instance;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        foreach (var h in reader.hands)
        {
            // Landmarks
            Gizmos.color = Color.yellow;
            foreach (var lm in h.Landmarks)
            {
                Gizmos.DrawSphere(lm, 0.0035f);
            }


            // Hand Lines
            lineThickness = 4;
            Handles.color = Color.grey;

            HandLine(LmTag.THUMB_CMC, LmTag.THUMB_MCP);

            HandLine(LmTag.THUMB_MCP, LmTag.THUMB_IP);
            HandLine(LmTag.THUMB_IP, LmTag.THUMB_TIP);

            HandLine(LmTag.WRIST, LmTag.INDEX_FINGER_MCP);

            HandLine(LmTag.INDEX_FINGER_MCP, LmTag.INDEX_FINGER_PIP);
            HandLine(LmTag.INDEX_FINGER_PIP, LmTag.INDEX_FINGER_DIP);
            HandLine(LmTag.INDEX_FINGER_DIP, LmTag.INDEX_FINGER_TIP);

            HandLine(LmTag.INDEX_FINGER_MCP, LmTag.MIDDLE_FINGER_MCP);

            HandLine(LmTag.MIDDLE_FINGER_MCP, LmTag.MIDDLE_FINGER_PIP);
            HandLine(LmTag.MIDDLE_FINGER_PIP, LmTag.MIDDLE_FINGER_DIP);
            HandLine(LmTag.MIDDLE_FINGER_DIP, LmTag.MIDDLE_FINGER_TIP);

            HandLine(LmTag.MIDDLE_FINGER_MCP, LmTag.RING_FINGER_MCP);

            HandLine(LmTag.RING_FINGER_MCP, LmTag.RING_FINGER_PIP);
            HandLine(LmTag.RING_FINGER_PIP, LmTag.RING_FINGER_DIP);
            HandLine(LmTag.RING_FINGER_DIP, LmTag.RING_FINGER_TIP);

            HandLine(LmTag.RING_FINGER_MCP, LmTag.PINKY_MCP);

            HandLine(LmTag.PINKY_MCP, LmTag.PINKY_PIP);
            HandLine(LmTag.PINKY_PIP, LmTag.PINKY_DIP);
            HandLine(LmTag.PINKY_DIP, LmTag.PINKY_TIP);

            HandLine(LmTag.PINKY_MCP, LmTag.WRIST);


            // Bounds
            lineThickness = 3;
            Handles.color = new Color(255, 0, 255);

            // Border
            BorderSide(-1, -1, 1, -1);
            BorderSide(1, -1, 1, 1);
            BorderSide(1, 1, -1, 1);
            BorderSide(-1, 1, -1, -1);

            // Center
            Gizmos.color = new Color(255, 0, 255);
            Gizmos.DrawSphere(h.Bounds.center, 0.0035f);


            // Gesture & Handedness
            float screenSizeFactor = HandleUtility.GetHandleSize(Vector3.zero) * 0.1f;
            GUIStyle style = new(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(255, 0, 255);
            style.fontSize = Mathf.RoundToInt(100 * screenSizeFactor);

            Handles.Label(h.GetLm(LmTag.WRIST), h.Gesture.ToString());
            Handles.Label(h.GetLm(LmTag.WRIST) + (Vector3.down * 50f), h.Handedness.ToString());


            // Axis
            lineThickness = 5;
            Handles.color = Color.green;
            Line(h.GetLm(LmTag.WRIST), h.GetLm(LmTag.WRIST) + h.UpOrientation.Direction);

            Handles.color = Color.red;
            Line(h.GetLm(LmTag.WRIST), h.GetLm(LmTag.WRIST) + h.RightOrientation.Direction);

            Handles.color = Color.blue;
            Line(h.GetLm(LmTag.WRIST), h.GetLm(LmTag.WRIST) + h.ForwardOrientation.Direction);



            // Helpers
            void Line(Vector3 start, Vector3 end) => Handles.DrawLine(start, end, lineThickness);
            void HandLine(LmTag start, LmTag end) => Line(h.GetLm(start), h.GetLm(end));

            void BorderSide(int signStartX, float signStartY, int signEndX, float signEndY)
            {
                Line(
                    h.Bounds.center + new Vector3(h.Bounds.extents.x * signStartX, h.Bounds.extents.y * signStartY),
                    h.Bounds.center + new Vector3(h.Bounds.extents.x * signEndX, h.Bounds.extents.y * signEndY));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class HandReader : MonoBehaviour
{
    const int MAX_NUM_HANDS = 2; // MAX_NUM_PLAYERS * 2

    const int POINT_Y_OFFSET = 3, END_POINT_OFFSET = 5;

    public class Hand
    {
        public const int NUM_LM = 21;

        public enum LmTag
        {
            WRIST = 0,
            THUMB_CMC = 1, THUMB_MCP = 2, THUMB_IP = 3, THUMB_TIP = 4,
            INDEX_FINGER_MCP = 5, INDEX_FINGER_PIP = 6, INDEX_FINGER_DIP = 7, INDEX_FINGER_TIP = 8,
            MIDDLE_FINGER_MCP = 9, MIDDLE_FINGER_PIP = 10, MIDDLE_FINGER_DIP = 11, MIDDLE_FINGER_TIP = 12,
            RING_FINGER_MCP = 13, RING_FINGER_PIP = 14, RING_FINGER_DIP = 15, RING_FINGER_TIP = 16,
            PINKY_MCP = 17, PINKY_PIP = 18, PINKY_DIP = 19, PINKY_TIP = 20
        }

        public Vector3[] Landmarks { get; set; }

        public Bounds Bounds { get; set; }

        public class Orientation
        {
            public Vector3 Direction;
            public Quaternion Rotation;

            public Orientation Updated(Vector3 dir, bool flip = false)
            {
                if (dir != Vector3.zero)
                {
                    if (flip)
                    {
                        dir = -dir;
                    }

                    Rotation = Quaternion.LookRotation(Direction = dir.normalized);
                }
                return this;
            }
            public Orientation Updated(Vector3 dir, Vector3 rotationOffset, bool flip = false)
            {
                Vector3
                    up = Vector3.Cross(dir, Vector3.up),
                    right = Vector3.Cross(dir, Vector3.right),
                    forward = Vector3.Cross(dir, Vector3.forward);

                Quaternion
                    rotationUp = Quaternion.AngleAxis(rotationOffset.y, up),
                    rotationRight = Quaternion.AngleAxis(rotationOffset.x, right),
                    rotationForward = Quaternion.AngleAxis(rotationOffset.z, forward);

                dir = rotationUp * rotationRight * rotationForward * dir;

                Updated(dir, flip);
                return this;
            }
        }

        Orientation _wristOrientation = new();
        public Orientation WristOrientation => _wristOrientation.Updated(
            GetLm(LmTag.MIDDLE_FINGER_MCP) - GetLm(LmTag.WRIST));

        public Vector3 UP_ORIENTATION_OFFSET = new (0, 0, 10);
        Orientation _upOrientation = new();
        public Orientation UpOrientation => _upOrientation.Updated(
            WristOrientation.Direction,
            UP_ORIENTATION_OFFSET);

        public Vector3 RIGHT_ORIENTATION_OFFSET = new (5, -5, -5);
        Orientation _rightOrientation = new();
        public Orientation RightOrientation => _rightOrientation.Updated(
            GetLm(LmTag.INDEX_FINGER_MCP) - GetLm(LmTag.RING_FINGER_MCP),
            RIGHT_ORIENTATION_OFFSET);

        Orientation _forwardOrientation = new();
        public Orientation ForwardOrientation => _forwardOrientation.Updated(
            Vector3.Cross(UpOrientation.Direction, RightOrientation.Direction),
            Handedness == _Handedness.Right);

        Orientation _pointerOrientation = new();
        public Orientation PointerOrientation => _pointerOrientation.Updated(
            GetLm(LmTag.INDEX_FINGER_TIP) - GetLm(LmTag.INDEX_FINGER_DIP));

        public enum _Handedness { Left, Right }
        public _Handedness Handedness { get; set; }

        public enum _Facing { Front, Back }
        public _Facing Facing => Vector3.Dot(ForwardOrientation.Direction, Camera.main.transform.forward) > 0.5f
            ? _Facing.Back
            : _Facing.Front;

        public enum Gestures { Open, Close, Pointer }
        
        Gestures rawGesture;
        const int GESTURE_FRAME_COUNT_THRESHOLD = 4;
        int gestureFrameCount;

        Gestures gesture;
        public Gestures Gesture
        {
            get => gesture;
            set
            {
                if (rawGesture == value)
                {
                    gestureFrameCount++;
                    if (gestureFrameCount >= GESTURE_FRAME_COUNT_THRESHOLD)
                    {
                        gesture = rawGesture;
                    }
                }
                else
                {
                    gestureFrameCount = 0;
                }

                rawGesture = value;
            }
        }

        public Hand()
        {
            Landmarks = new Vector3[NUM_LM];
        }

        public Vector3 GetLm(LmTag lmTag) => Landmarks[(int)lmTag];

        public void UpdateBounds()
        {
            var center = Vector3.zero;
            float
                maxX = float.MinValue,
                maxY = float.MinValue;
            float minZ = float.MaxValue;

            foreach (var lm in Landmarks)
            {
                center += lm;

                maxX = Mathf.Max(lm.x, maxX);
                maxY = Mathf.Max(lm.y, maxY);

                minZ = Mathf.Min(lm.z, minZ);
            }
            center /= NUM_LM;
            center.z = minZ;

            Bounds = new(center, new Vector2(
                maxX - center.x,
                maxY - center.y) * 2f);
        }
    }

    public List<Hand> hands = new();

    public event Action<List<Hand>> OnHandsRefresh;

    [SerializeField] bool updateBounds = true;

    [SerializeField] float handLifeTime;
    float handLifeTimer;

    ScreenUtility screenUtility;

    public static HandReader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        screenUtility = ScreenUtility.Instance;

        var trackDataReader = TrackDataReader.Instance;
        trackDataReader.Datasets[TrackDataReader.Tags.HandLm] += SerialiseHandLm;
        trackDataReader.Datasets[TrackDataReader.Tags.HandHandedness] += SerialiseHandedness;
        trackDataReader.Datasets[TrackDataReader.Tags.HandGesture] += SerialiseHandGesture;
    }

    string TrimFirstLast(string data, int length = 1)
    {
        data = data.Remove(0, length);
        data = data.Remove(data.Length - length, length);
        return data;
    }

    void AdjustHandCount(float newCount)
    {
        while (hands.Count < newCount)
        {
            hands.Add(new Hand());
        }
        while (hands.Count > newCount)
        {
            hands.RemoveAt(0);
        }
    }

    string[] lmTemp = new string[Hand.NUM_LM * 2 * MAX_NUM_HANDS];
    void SerialiseHandLm(List<string> dataset)
    {
        AdjustHandCount(dataset.Count);

        for (int i = 0; i < hands.Count; i++)
        {
            string data = TrimFirstLast(dataset[i]);

            // Read
            int pointI = 0;
            int startI = 1;
            bool endPoint;
            for (int endI = 1; endI < data.Length; endI++)
            {
                endPoint = data[endI] == ']';
                if (data[endI] == ',' || endPoint)
                {
                    endI--;
                    lmTemp[pointI] = data.Substring(startI, endI - startI + 1);
                    pointI++;

                    endI += endPoint ? END_POINT_OFFSET : POINT_Y_OFFSET;
                    startI = endI;
                }
            }

            for (int j = 0; j < Hand.NUM_LM; j++)
            {
                hands[i].Landmarks[j] = screenUtility.ScreenToOverlayPos(GetPos(j));
            }

            handLifeTimer = handLifeTime;
        }

        OnHandsRefresh?.Invoke(hands);


        Vector3 GetPos(int i) => new(
            float.Parse(lmTemp[i * 3]),
            float.Parse(lmTemp[i * 3 + 1]),
            float.Parse(lmTemp[i * 3 + 2]));
    }

    private void Update()
    {
        handLifeTimer = Mathf.Clamp(handLifeTimer - Time.deltaTime, 0, handLifeTime);
        if (handLifeTimer <= 0f)
        {
            hands.Clear();
            OnHandsRefresh?.Invoke(hands);
        }

        foreach (var hand in hands)
        {
            if (updateBounds)
            {
                hand.UpdateBounds();
            }
        }
    }

    void SerialiseHandedness(List<string> dataset)
    {
        AdjustHandCount(dataset.Count);

        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].Handedness = (Hand._Handedness)int.Parse(TrimFirstLast(dataset[i]));
        }
        OnHandsRefresh?.Invoke(hands);
    }

    void SerialiseHandGesture(List<string> dataset)
    {
        AdjustHandCount(dataset.Count);

        for (int i = 0; i < hands.Count; i++)
        {
            hands[i].Gesture = (Hand.Gestures)int.Parse(TrimFirstLast(dataset[i]));
        }
        OnHandsRefresh?.Invoke(hands);
    }
}
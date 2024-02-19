using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VirtualCamera : MonoBehaviour
{
    [SerializeField] private CinemachineTrackedDolly trackedDolly;

    private Camera mainCamera;

    public GameObject[] positions;

    [SerializeField] private bool isMoving = false;

    private bool complete = false;

    void Start()
    {
        trackedDolly = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(isMoving)
        {
            float newPathPos = Mathf.Lerp(trackedDolly.m_PathPosition, 2, Time.deltaTime);
            //trackedDolly.m_PathPosition = newPathPos;

            if (Mathf.Approximately(newPathPos, 2) || Mathf.Approximately(newPathPos, 2 - 0.05f))
            {
                trackedDolly.m_PathPosition = 2;
                isMoving = false;
            }
            else
            {
                trackedDolly.m_PathPosition = newPathPos;
            }
        }
        
        if(!complete)
        {
            if(Vector3.Distance(mainCamera.transform.position, transform.position) < 0.05f)
            {
                isMoving = true;
                complete = true;
            }
        }
    }
}

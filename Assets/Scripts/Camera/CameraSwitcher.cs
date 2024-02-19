using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance;

    public CinemachineVirtualCamera[] virtualCameras;
    [SerializeField] private int currentCameraIndex = 0;

    private bool introComplete = false;

    public float delay = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SwitchToCamera(currentCameraIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(!introComplete)
            {
                currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Length;

                SwitchToCamera(currentCameraIndex);
            }
            else
            {

            }            
        }
    }

    public void ToggleCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Length;

        SwitchToCamera(currentCameraIndex);
    }

    private void SwitchToCamera(int targetIndex)
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].enabled = i == targetIndex;
        }
    }

    [ContextMenu("Get All Virtual Cameras")]
    private void GetAllVirtualCameras()
    {
        virtualCameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
    }

    public void CockpitView()
    {
        //StartCoroutine(cView());
    }

    IEnumerator cView()
    {
        SwitchToCamera(1);
        yield return new WaitForSeconds(delay);
        SwitchToCamera(2);

    }

    /*    public CinemachineVirtualCamera mainCamera;

        public CinemachineVirtualCamera[] virtualCameras;

        private void Awake()
        {

        }

        private void Start()
        {
            SwitchToCamera(mainCamera);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {

            }
        }

        private void SwitchToCamera(CinemachineVirtualCamera targetCam)
        {
            foreach(CinemachineVirtualCamera cam in virtualCameras) 
            {
                cam.enabled = cam == targetCam;
            }
        }*/
}

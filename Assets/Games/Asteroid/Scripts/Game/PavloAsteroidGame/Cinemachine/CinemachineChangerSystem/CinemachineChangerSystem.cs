using System;
using System.Collections.Generic;
using Cinemachine;
using CinemachineCamerasDatas;
using UnityEngine;

namespace CinemachineChangerSystem
{
    public class CinemachineChangerSystem : MonoBehaviour
    {
        [SerializeField] private List<CameraLines> _camerasLists = new();
        
        //test      
        public void Start()
        {
            UseCameraFromList(0, 2);
        }

        public void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                UseNextCameraFromList(0);
            }
        }
        //
        public void UseCameraFromList(int cameraList, int virtualCameraIndex)
        {
            _camerasLists[cameraList].UseCamera(virtualCameraIndex);
        }

        public void UseNextCameraFromList(int cameraList)
        {
            _camerasLists[cameraList].UseNextCamera();
        }
    }
}
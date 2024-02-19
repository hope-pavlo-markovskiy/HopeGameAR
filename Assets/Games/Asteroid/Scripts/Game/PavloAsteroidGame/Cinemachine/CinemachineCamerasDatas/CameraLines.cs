using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

namespace CinemachineCamerasDatas
{
    [Serializable]
    public class CameraLines
    {
        [SerializeField] private List<CinemachineVirtualCamera> _virtualCameras;
        [SerializeField] private List<float> _delayTimes;
        [SerializeField] private CinemachineBrain _cinemachineBrain;
    
        private CancellationTokenSource _cancellationToken = new();
        private CinemachineVirtualCamera _currentCamera;
        private bool _isPlayLine;
        private bool _cancel;
        
        public List<CinemachineVirtualCamera> VirtualCameras
        {
            get => _virtualCameras;
            set => _virtualCameras = value;
        }
        public List<float> DelayTimes
        {
            get => _delayTimes;
            set => _delayTimes = value;
        }
        public CinemachineBrain CmBrain
        {
            get => _cinemachineBrain;
            set => _cinemachineBrain = value;
        }
        
        public void Dispose()
        {
            _cancellationToken.Cancel();
            _cancellationToken = new CancellationTokenSource();
        }
        
        public void Cancel()
        {
            _isPlayLine = false;
            _cancel = true;
        }
        public async Task PlayLine()
        {
            if (!_cancellationToken.Token.IsCancellationRequested && !_isPlayLine)
            {
                _isPlayLine = true;
                for (int i = 0; i < _virtualCameras.Count; i++)
                {
                    if (_cancel)
                    {
                        Dispose();
                        return;
                    }
                    Debug.Log(_virtualCameras[i]);
                    SwitchCamera(i);
                    await Task.Delay((int) (_delayTimes[i] * 1000));
                }
            
                _isPlayLine = false;
                _cancellationToken.Cancel();
            }
        }

        public bool IsPlayLine()
        {
            return _isPlayLine;
        }

        public void UseCamera(int cameraIndex)
        {
            ChangeCameraSwap(_delayTimes[cameraIndex]);
            SwitchCamera(cameraIndex);
        }

        public void UseNextCamera()
        {
            if (_currentCamera == null)
            {
                int firstCamera = 0;
                _currentCamera = _virtualCameras[firstCamera];
            }
            for (int i = 0; i < _virtualCameras.Count; i++)
            {
                
                if (i != _virtualCameras.Count-1 && _virtualCameras[i] == _currentCamera)
                {
                    int nextCameraIndex = i + 1;
                    ChangeCameraSwap(_delayTimes[i]);
                    SwitchCamera(nextCameraIndex);
                    return;
                }
                if(i == _virtualCameras.Count-1 && _virtualCameras[i] == _currentCamera)
                {
                    int nextCameraIndex = 0;
                    ChangeCameraSwap(_delayTimes[i]);
                    SwitchCamera(nextCameraIndex);
                    return;
                }
            }
        }
        
        /// <summary>
        /// time = 0 - no delay, 
        /// time = 2 - big delay
        /// </summary>
        public void ChangeCameraSwap(float time)
        {
            _cinemachineBrain.m_DefaultBlend.m_Time = time;
        }
        
        private void SwitchCamera(int index)
        {
            for (int i = 0; i < _virtualCameras.Count; i++)
            {
                if (i == index)
                {
                    _virtualCameras[i].enabled = true;
                    _virtualCameras[i].Priority = 10;
                    _currentCamera = _virtualCameras[i];
                    Debug.Log(_currentCamera);
                }
                else
                {
                    _virtualCameras[i].enabled = false;
                    _virtualCameras[i].Priority = 0;
                }
            }
        }
    }
}
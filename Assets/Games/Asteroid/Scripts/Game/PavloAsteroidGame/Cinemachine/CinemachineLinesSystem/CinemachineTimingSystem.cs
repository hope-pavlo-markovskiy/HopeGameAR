using System.Collections.Generic;
using Cinemachine;
using CinemachineCamerasDatas;
using UnityEngine;

namespace CinemachineLinesSystem
{
    public class CinemachineTimingSystem : MonoBehaviour
    {
        [SerializeField] private List<CameraLines> _camerasLines;
        private CameraLines _currentLine = new();
        //test
        private void Start()
        {
            ChangeCameraSwapForAll(2);
            PlayNewLine(1);
            CancelLines();
            PlayNewLine(0);
        }
        //
        private void OnDestroy()
        {
            _currentLine.Dispose();
        }

        private async void StartLine(int lineIndex)
        {
            _currentLine = _camerasLines[lineIndex];
            await _currentLine.PlayLine();
        
        }

        private void ChangeCameraSwapForAll(float delay)
        {
            for (int i = 0;i <_camerasLines.Count; i++ )
            {
                _camerasLines[i].ChangeCameraSwap(delay);
            }
        }
        public void PlayNewLine(int index)
        {
            if (!_currentLine.IsPlayLine())
            {
                StartLine(index);
            }
            else
            {
                Debug.LogError("The line is running! Stop current line first");
            }
        }

        public void CancelLines()
        {
            List<CinemachineVirtualCamera> virtualCameras;
            CinemachineBrain cinemachineBrain;
            List<float> delayTimes;
            
            for (int i = 0; i < _camerasLines.Count; i++)
            {
                _camerasLines[i].Cancel();
                virtualCameras = _camerasLines[i].VirtualCameras;
                delayTimes =  _camerasLines[i].DelayTimes;
                cinemachineBrain = _camerasLines[i].CmBrain;
                _camerasLines[i] = new CameraLines();
                _camerasLines[i].VirtualCameras = virtualCameras;
                _camerasLines[i].DelayTimes = delayTimes;
                _camerasLines[i].CmBrain  = cinemachineBrain;
            }
        }
    }
}
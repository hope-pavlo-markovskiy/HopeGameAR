using System;
using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip
{
    [Serializable]
    public class HandMovementManager
    {
        [Header("Ship States")]
        [SerializeField] private bool _canMove = true;
        
        [Header("Hands")]
        [SerializeField] private bool _leftHandEnabled;
        [SerializeField] private bool _rightHandEnabled;
        
        [Header("MoveData")]
        [SerializeField] private float _maxMovementDistance = 5f;
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _horizontalInputSmoothTime;
        [SerializeField] private float _horizontalInputRaw;
        [SerializeField] private float _horizontalInput;
        
        private float horizontalInputVelocity;
        private Animator _anim;
        private GameObject _currentShip;

        public void Init( GameObject currentShip, Animator anim )
        {
            _anim = anim;
            _currentShip = currentShip;
        }

        public void HandleMovement()
        {
            _horizontalInput = Mathf.SmoothDamp(_horizontalInput, _horizontalInputRaw, ref horizontalInputVelocity, _horizontalInputSmoothTime);
            Vector3 newPosition = _currentShip.transform.position + new Vector3(_horizontalInput, 0f, 0f) * _movementSpeed * Time.deltaTime;
            if(!_leftHandEnabled && !_rightHandEnabled)
            {
                _horizontalInputRaw = 0;
            }
            newPosition.x = Mathf.Clamp(newPosition.x, -_maxMovementDistance, _maxMovementDistance);
            _currentShip.transform.position = newPosition;
            _anim.SetFloat("Horizontal", _horizontalInputRaw);
        }
        
        public void LeftHandEntry(HandReader.Hand hand)
        {
            if (_canMove)
            {
                _leftHandEnabled = true;
                _horizontalInputRaw = -1f;
            }
        }

        public void LeftHandExit() 
        {
            _leftHandEnabled = false;
        }

        public void RightHandEntry(HandReader.Hand hand) 
        {
            if (_canMove)
            {
                _rightHandEnabled = true;
                _horizontalInputRaw = 1f;
            }
        }

        public void RightHandExit()
        {
            _rightHandEnabled = false;   
        }
        
        public void CanMoveSwitcher()
        {
            _horizontalInputRaw = 0;
            _canMove = !_canMove;
        }
    }
}
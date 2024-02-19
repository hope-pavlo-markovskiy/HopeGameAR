using System;
using System.Collections.Generic;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.AsteroidManagers;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AudioSystem;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AudioSystem.ScroiptableObject;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip
{
    [Serializable]
    public class ShipTouchingManager
    {
        [Header("Audio")]
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private List<AudioConfig> _characterSoundSets;
        [SerializeField] private AudioConfig _shipSound;
        [SerializeField] private Transform _audioContainer;
        
        [Header("Layers")]
        [SerializeField] private LayerMask _asteroidLayer;

        [Header("ShipColliders")]
        [SerializeField] private Vector3 _hitBoxOffset;
        [SerializeField] private Vector3 _hitBoxSize;
        [SerializeField] private Vector3 _nearDetectOffset;
        [SerializeField] private Vector3 _nearDetectSize;
        [SerializeField] private float _aheadDetectRadius;
        [SerializeField] private float _aheadDetectDistance;
        
        [Header("Managers")]
        [SerializeField] private HitAnimationManager _hitAnimationManager;
        
        private Collider detectedAheadAsteroid;
        private Collider detectedNearAsteroid;
        private Collider detectedHitAsteroid;
        private GameObject _currentShip;

        public event Action OnCanMoveChanger;
        
        public void Init(GameObject currentShip)
        {
            _currentShip = currentShip;
            _hitAnimationManager.Init(currentShip);
        }
        
        public void CheckCollisionDetection()
        {
            Vector3 shipPosition = _currentShip.transform.position;
            bool isHitDetected = GetBoxCollision(shipPosition + _hitBoxOffset, _hitBoxSize, out Collider asteroidHit);
            bool isNearDetected = GetBoxCollision(shipPosition + _nearDetectOffset, _nearDetectSize, out Collider asteroidNear);
            bool isAheadDetected = GetSphereCollision(shipPosition + (Vector3.forward * _aheadDetectDistance), _aheadDetectRadius, out Collider asteroidAhead);

            if (isHitDetected && asteroidHit != detectedHitAsteroid)
            {
                HitColliderTouched(asteroidHit);
                detectedHitAsteroid = asteroidHit;
            }
            if (isNearDetected && asteroidNear != detectedNearAsteroid)
            {
                NearColliderTouched();
                detectedNearAsteroid = asteroidNear;
            }
            if (isAheadDetected && asteroidAhead != detectedAheadAsteroid)
            {
                AheadColliderTouched();
                detectedAheadAsteroid = asteroidAhead;
            }
        }

        private bool GetBoxCollision(Vector3 pos, Vector3 size, out Collider asteroidCollider)
        {
            Collider[] c = new Collider[1];
            if (Physics.OverlapBoxNonAlloc(pos, size * 0.5f, c, Quaternion.identity, _asteroidLayer) == 1)
            {
                asteroidCollider = c[0];
                return true;
            }
            
            asteroidCollider = null;
            return false;
        }

        private bool GetSphereCollision(Vector3 pos, float radius, out Collider asteroidCollider)
        {
            
            Collider[] c = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(pos, radius, c, _asteroidLayer) == 1)
            {
                asteroidCollider = c[0];
                return true;
            }
            else
            {
                asteroidCollider = null;
                return false;
            }
        }

        private void HitColliderTouched(Collider other)
        {
            OnCanMoveChanger?.Invoke();

            _hitAnimationManager.ForceFieldFlashAnim();
            GameObject gameObject = other.gameObject;
            gameObject.GetComponent<AsteroidController>().Hit(); // Destroy asteroid
            _hitAnimationManager.KnockBack(gameObject);

            OnCanMoveChanger?.Invoke();

        } 
        
        private void AheadColliderTouched()
        {
            _audioManager.PlaySound(_shipSound, _audioContainer);
        }
        
        private void NearColliderTouched()
        {
            AudioConfig characterVoice = _characterSoundSets[Random.Range(0, _characterSoundSets.Count)];
            _audioManager.PlayVoice(characterVoice, _audioContainer);
        }
        
    }
}
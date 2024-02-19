using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.AsteroidManagers
{
    public class AsteroidController : MonoBehaviour
    {
        //[SerializeField] private GameObject[] _pieces;
        [Header("The main object that will move")]
        [SerializeField] private GameObject _generalObject;
        [Header("The main object that will disappear (With mesh)")]
        [SerializeField] private GameObject _visibleObject;
        //[SerializeField] private GameObject _piecesGroup;
        [SerializeField] private GameObject _explodeEffect;
        [SerializeField] private int _movingSpeed;
        [SerializeField] private float _rotationSpeedMin = 10f;
        [SerializeField] private float _rotationSpeedMax = 50f;
        [SerializeField] private float _destroyingDistance;
        //[SerializeField] private float _explosionForce;
        [SerializeField] private float _disappearingDelay = 2;
        
        private Transform _mainAsteroidDestination;
        private Vector3 _decoyDestination;
        private Vector3 _randomRotationAxis;
        private float _randomRotationSpeed;
        private bool _isDecoy;
        private bool _isHitted;
        private CancellationTokenSource _cancellationToken = new();

        public static event Action OnAddScore;
        public static event Action<GameObject> OnResetAsteroid;
        
        private void Start()
        {
            _randomRotationAxis = Random.onUnitSphere;
            _randomRotationSpeed = Random.Range(_rotationSpeedMin, _rotationSpeedMax);
        }

        private void Update()
        {
            
            RotateAsteroid();
            MoveObject();
            CheckDestinationDistance();
        }

        private void OnDestroy()
        {
            _cancellationToken.Cancel();
        }

        private void OnDisable()
        {
            _cancellationToken.Cancel();
        }

        public void Init(bool isDecoy)
        {
            _isDecoy = isDecoy;
        }

        public void SetDestination(Transform destination)
        {
            _mainAsteroidDestination = destination;
        }

        public void SetDestination(Vector3 destination)
        {
            _decoyDestination = destination;
        }
        // If we need to make different type of objects. Just send here ScriptableObjcet with all datas and assemble it
        public void AssembleAsteroid()
        {
            
        }
        //
        public async void Hit()
        {
            if (!_isHitted)
            {
                _isHitted = true;
                _cancellationToken = new CancellationTokenSource();
                await ExplodeObject();
            }
        }
        
        private void CheckDestinationDistance()
        {
            float dist;
            if (_isDecoy && _decoyDestination.z > _generalObject.transform.position.z)
            {
                ResetObject();
            }
            else if (!_isDecoy)
            {
                dist = Vector3.Distance(_mainAsteroidDestination.position, gameObject.transform.position);
                if (dist < _destroyingDistance && !_isHitted)
                {
                    OnAddScore?.Invoke();
                    ResetObject();
                }
            }
        }
        private void RotateAsteroid()
        {
            transform.Rotate(_randomRotationAxis, _randomRotationSpeed * Time.deltaTime);
        }

        private void MoveObject()
        {
            if (_isDecoy)
            {
                MoveDecoy();
            }
            else
            {
                MoveAsteroid();
            }
        }
        private void MoveAsteroid()
        {
            _generalObject.transform.position = Vector3.MoveTowards(_generalObject.transform.position, _mainAsteroidDestination.position, _movingSpeed * Time.deltaTime);
            
        }

        private void MoveDecoy()
        {
            _generalObject.transform.Translate(-Vector3.forward * _movingSpeed * Time.deltaTime);
        }

        private void ResetObject()
        {
            if (_visibleObject)
            {
                _visibleObject.GetComponent<Renderer>().enabled = true;
            }

            _cancellationToken.Cancel();
            _isHitted = false;
            _mainAsteroidDestination = null;
            _isDecoy = false;
            OnResetAsteroid?.Invoke(_generalObject);
        }
        private async Task ExplodeObject()
        {
            if (!_cancellationToken.Token.IsCancellationRequested)
            {
                _visibleObject.GetComponent<Renderer>().enabled = false;
                GameObject explodeEffect = Instantiate(_explodeEffect, transform.position, transform.rotation);
                explodeEffect.SetActive(true);
                
                //Explotion system
               // ExplosionSystem _explosionSystem = new(_piecesGroup.transform, _explosionForce);
                //_explosionSystem.ExplodeObject(gameObject, _piecesGroup, _pieces);
                //
                
                await Task.Delay((int) (_disappearingDelay * 1000));
               ResetObject();
            }
        }
    }
}

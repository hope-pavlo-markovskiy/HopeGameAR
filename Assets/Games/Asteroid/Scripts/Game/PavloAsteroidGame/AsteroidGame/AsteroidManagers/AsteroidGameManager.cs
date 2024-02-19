using System;
using System.Threading;
using System.Threading.Tasks;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.PoolObjectSystem.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.AsteroidManagers
{
    [Serializable]
    public class AsteroidGameManager
    {
        [SerializeField] private GameObject _asteroidPrefab;
        [SerializeField] private Transform _asteroidContainer;
        
        [SerializeField] private Transform[] _spawnLocations;
        [SerializeField] private Transform[] _destinationLocations;
        [SerializeField] private Transform[] _decoyLocations;
        [SerializeField] private Vector3 _decoysDestination;
        
        [SerializeField] private float _spawnAsteroidsDaley;
        [SerializeField] private float _spawnDecoyDaley;

        [SerializeField] private int _firstAsteroidQuantity = 3;
        
        [SerializeField] private bool _canSpawnAsteroids;
        [SerializeField] private bool _canSpawnDecoy;
        
        private ObjectPool _asteroids = new();
        private CancellationTokenSource _cancellationSpawnAsteroids = new();
        private CancellationTokenSource _cancellationSpawnDecoy = new();

        public AsteroidGameManager()
        {
            Subscribe();
        }
        public void Dispose()
        {
            Unsubscribe();
            FinishAsteroids();
        }
        
        public void RunAsteroids()
        {
            _asteroids.InitializePool(_firstAsteroidQuantity, _asteroidPrefab, _asteroidContainer);
            StartSpawning();
        }
        
        public void FinishAsteroids()
        {
            _cancellationSpawnAsteroids.Cancel();
            _cancellationSpawnDecoy.Cancel();
        }

        private void Subscribe()
        {
            AsteroidController.OnResetAsteroid += _asteroids.DisableComponent;
        }
        
        private void Unsubscribe()
        {
            AsteroidController.OnResetAsteroid -= _asteroids.DisableComponent;
        }
        
        private void StartSpawning()
        {
            SpawnAsteroids();
            SpawnDecoy();
        }
        private async Task SpawnAsteroids()
        {
            while (!_cancellationSpawnAsteroids.Token.IsCancellationRequested && _canSpawnAsteroids)
            {
                PrepareAsteroid();
                await Task.Delay((int)(_spawnAsteroidsDaley * 1000));
            }
        }
        
        private async Task SpawnDecoy()
        {
            while(!_cancellationSpawnDecoy.Token.IsCancellationRequested && _canSpawnDecoy)
            {
                PrepareDecoy();
                await Task.Delay((int)(_spawnDecoyDaley * 1000));
            }
        }

        private void PrepareAsteroid()
        {
            int rand = Random.Range(0, 2);
            GameObject newObject = _asteroids.GetComponent();
            newObject.transform.position = _spawnLocations[rand].position;
            newObject.SetActive(true);
            newObject.transform.GetComponentInChildren<AsteroidController>().SetDestination(_destinationLocations[rand]);
            
        }

        private void PrepareDecoy()
        {
            int rand = Random.Range(0, 4);
            GameObject newObject = _asteroids.GetComponent();
            newObject.transform.position = _decoyLocations[rand].position;
            newObject.SetActive(true);
            newObject.transform.GetComponentInChildren<AsteroidController>().Init(true);
            newObject.transform.GetComponentInChildren<AsteroidController>().SetDestination(_decoysDestination);
            
        }
    }
}

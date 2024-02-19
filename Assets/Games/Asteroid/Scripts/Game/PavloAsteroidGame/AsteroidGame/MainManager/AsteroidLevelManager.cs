using System.Threading;
using System.Threading.Tasks;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.AsteroidManagers;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AudioSystem;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.AudioSystem.ScroiptableObject;
using Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip;
using UnityEngine;
using UnityEngine.Playables;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.MainManager
{
    public class AsteroidLevelManager : MonoBehaviour
    {
        public static AsteroidLevelManager Instance;
        
        [Header("Managers")]
        [SerializeField] private AsteroidGameManager _asteroidGameManager;
        [SerializeField] private PlayableDirector _timeline;
        [SerializeField] private SpaceShipManager _shipManager;
        [SerializeField] private AudioManager _audioManager;

        [Header("Effects")]
        [SerializeField] private GameObject _asterBg;
        [SerializeField] private GameObject _dustParticle;
        [SerializeField] private GameObject _teleporter;
        [SerializeField] private GameObject _playerQuad;
        
        [Header("Sounds")]
        [SerializeField] private AudioConfig _lookAudio;
        [SerializeField] private AudioConfig _ziggyWoo;
        [SerializeField] private AudioConfig _spaceshipWoosh;

        [Header("Containers")] 
        [SerializeField] private Transform _audioContainer;
        
        [Header("Delays")]
        [SerializeField] private float _delayGameStart = 3f;
        [SerializeField] private float _teleportDelay;
        [SerializeField] private float _appearingDaley = 1f;
        
        [Header("Other")]
        [SerializeField] private int _asteroidsComplete;
        [SerializeField] private int _asteroidsRequired;
        [SerializeField] private bool _spawnAsteroids = true;

        private readonly CancellationTokenSource _cancellationToken = new();
        
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }
            Subscribe();
        }   
        private void Start()
        {
            PythonProcessor.Instance.StartProgram();
        }
        private void Update()
        {
            CheckProgress();
        }

        private void OnDestroy()
        {
            _asteroidGameManager.Dispose();
            _cancellationToken.Cancel();
            Unsubscribe();
        }
    
        private void Subscribe()
        {
            _timeline.stopped += TimelineEnd;
            AsteroidController.OnAddScore += AddScore;
        }
        
        private void Unsubscribe()
        {
            _timeline.stopped -= TimelineEnd;
            AsteroidController.OnAddScore -= AddScore;
        }

        private async void TimelineEnd(PlayableDirector dir)
        {
            await PlayerAppearing(_teleportDelay, _appearingDaley);
            await StartGame(_delayGameStart);
        }
        private async Task PlayerAppearing(float teleportDelay, float appearingDaley)
        {
            if (!_cancellationToken.Token.IsCancellationRequested)
            {
                await Task.Delay((int) (teleportDelay * 1000));
                Debug.Log("Teleporter");
                _teleporter.SetActive(true);

                await Task.Delay((int) (appearingDaley * 1000));
                Debug.Log("Apearing");
                _playerQuad.SetActive(true);
                _audioManager.PlayVoice(_lookAudio,_audioContainer);
            }
        }    
        private async Task StartGame(float delay)
        {
            if (!_cancellationToken.Token.IsCancellationRequested)
            {
                await Task.Delay((int) (delay * 1000));
                Debug.Log("Start");
                CameraSwitcher.Instance.ToggleCamera();
                PlayStartGameSounds();
                ToggleShipControl();
                PlayStartGameEffects();
                if (_spawnAsteroids)
                {
                    _asteroidGameManager.RunAsteroids();
                }
            }
        }
        
        private void CheckProgress()
        {
            if(_asteroidsComplete >= _asteroidsRequired)
            {
                _asteroidGameManager.FinishAsteroids();
                Debug.Log("Game Complete");
            }
        }
        
        private void PlayStartGameSounds()
        {
            _audioManager.PlaySound(_spaceshipWoosh,_audioContainer);
            _audioManager.PlayVoice(_ziggyWoo,_audioContainer);
        }
        
        private void PlayStartGameEffects()
        {
            _asterBg.SetActive(true);
            _dustParticle.SetActive(true);
        }
        
        private void ToggleShipControl()
        {
            _shipManager.enabled = true;
        }

        private void AddScore()
        {
            _asteroidsComplete++;
        }
    }
}

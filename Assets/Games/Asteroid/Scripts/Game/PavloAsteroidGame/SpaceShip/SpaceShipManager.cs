using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip
{
    public class SpaceShipManager : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private MoveAsset _leftHand;
        [SerializeField] private MoveAsset _rightHand;

        [Header("Animation")]
        [SerializeField] private Animator _anim;

        [Header("Managers")]
        [SerializeField] private ShipTouchingManager _shipTouchingManager;

        [SerializeField] private HandMovementManager _handMovementManager;
        
        private MovesManager movesManager;
        
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            _handMovementManager.HandleMovement();
            _shipTouchingManager.CheckCollisionDetection();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            UnSubscribe();
        }
        
        public void Initialize()
        {
            _shipTouchingManager.Init(gameObject);
            _handMovementManager.Init(gameObject, _anim);
            InitializeMover();
        }
        
        private void InitializeMover()
        {
            movesManager = MovesManager.Instance;
            movesManager.Add(_leftHand);
            movesManager.Add(_rightHand);
        }

        private void Subscribe()
        {
            _shipTouchingManager.OnCanMoveChanger += _handMovementManager.CanMoveSwitcher;
            
            _leftHand.OnEnterUpdate += _handMovementManager.LeftHandEntry;
            _leftHand.OnExit += _handMovementManager.LeftHandExit;

            _rightHand.OnEnterUpdate += _handMovementManager.RightHandEntry;
            _rightHand.OnExit += _handMovementManager.RightHandExit;
        }

        private void UnSubscribe()
        {
            _shipTouchingManager.OnCanMoveChanger -= _handMovementManager.CanMoveSwitcher;
            
            _leftHand.OnEnterUpdate -= _handMovementManager.LeftHandEntry;
            _leftHand.OnExit -= _handMovementManager.LeftHandExit;

            _rightHand.OnEnterUpdate -= _handMovementManager.RightHandEntry;
            _rightHand.OnExit -= _handMovementManager.RightHandExit;
        }
    }
}
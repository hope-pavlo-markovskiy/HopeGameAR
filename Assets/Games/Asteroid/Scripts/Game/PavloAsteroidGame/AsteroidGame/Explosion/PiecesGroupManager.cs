using System.Collections.Generic;
using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame.Explosion
{
    public class PiecesGroupManager : MonoBehaviour
    {
        [SerializeField] private float _scaleSpeed = 1f;
        [SerializeField] private float _minScale = 0.1f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private GameObject[] _pieces;
        [SerializeField] private List<Vector3> _startPiecesScale;
        [SerializeField] private List<Transform> _startPiecesPosition;

        private bool _hasExploded;

        public void Start()
        {
            for (int i = 0; i < _pieces.Length; i++)
            {
                _startPiecesScale.Add(_pieces[i].transform.localScale);
            }
        }

        private void OnEnable()
        {
            _hasExploded = true;
            
        }
        private void Update()
        {
            if(_hasExploded)
            {
                for (int i = 0; i < _pieces.Length; i++)
                {
                    _pieces[i].transform.localScale -=  new Vector3(_scaleSpeed, _scaleSpeed, _scaleSpeed) * Time.deltaTime * _speed;

                    _pieces[i].transform.localScale =  Vector3.Max(_pieces[i].transform.localScale, new Vector3(_minScale, _minScale, _minScale));

                    if (_pieces[i].transform.localScale.x <= _minScale)
                    {
                        _pieces[i].SetActive(false);
                    }
                }
            }
        }

        public void ResetAllPieces()
        {
            for (int i = 0; i < _pieces.Length; i++)
            {
                if (!_pieces[i].activeInHierarchy && _startPiecesScale.Count > 0)
                {
                    _pieces[i].transform.localScale = _startPiecesScale[i];
                    _pieces[i].transform.position = _startPiecesPosition[i].position;
                    _pieces[i].SetActive(true);
                }
            }
        }
    }
}
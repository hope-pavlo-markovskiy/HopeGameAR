using System.Collections.Generic;
using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.AsteroidGame
{
    public class ExplosionSystem
    {
        private Transform _explosionPosition;
        private float _force;
        public ExplosionSystem(Transform explosionPosition, float force)
        {
            _explosionPosition = explosionPosition;
            _force = force;
        }
        public void ExplodeObject(GameObject currentObject,GameObject pieceGroup, GameObject[] pieces)
        {
            currentObject.GetComponent<Renderer>().enabled = false;
            pieceGroup.SetActive(true);
            
            Rigidbody[] piecesRb = new Rigidbody[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].SetActive(true);
                piecesRb[i] = pieces[i].GetComponent<Rigidbody>();
            }

            for (var index = 0; index < piecesRb.Length; index++)
            {
                Rigidbody piece = piecesRb[index];
                // Check if the Rigidbody exists
                if (piece != null)
                {
                    // Calculate direction from the explosion position to the piece
                    Vector3 direction = piece.transform.position - _explosionPosition.position;

                    // Calculate distance to determine the strength of the force
                    float distance = direction.magnitude;

                    // Apply explosion force to the piece with falloff
                    piece.AddForce(direction.normalized * _force, ForceMode.Impulse);
                }
            }
        }
    }
}
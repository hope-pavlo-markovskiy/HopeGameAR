using UnityEngine;

namespace Games.Asteroid.Scripts.Game.PavloAsteroidGame.SpaceShip
{
    public class GizmosDrawer : MonoBehaviour
    {
        [SerializeField] Vector3 nearDetectOffset;
        [SerializeField] Vector3 nearDetectSize;

        [SerializeField] float aheadDetectRadius;
        [SerializeField] float aheadDetectDistance;
        [SerializeField] Vector3 hitBoxOffset;
        [SerializeField] Vector3 hitBoxSize;
        private void OnDrawGizmosSelected()
        {
            // Ahead
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + (Vector3.forward * aheadDetectDistance), aheadDetectRadius);

            // Near
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + nearDetectOffset, nearDetectSize);

            // Hit
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + hitBoxOffset, hitBoxSize);
        }
    }
}
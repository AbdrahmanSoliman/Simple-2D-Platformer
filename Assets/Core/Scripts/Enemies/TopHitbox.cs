using UnityEngine;
using Platformer.Player;

namespace Platformer.Enemies
{
    public class TopHitbox : MonoBehaviour
    {
        [SerializeField] private float _bounceForce = 10f;

        private IEnemy _enemy;

        private void Awake()
        {
            _enemy = GetComponentInParent<IEnemy>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                // Only count as stomp if player is above the hitbox and moving downward/level (not walking from the side)
                bool isAboveHitbox = other.bounds.min.y >= transform.position.y - 0.25f;
                bool isNotMovingUpward = other.attachedRigidbody == null || other.attachedRigidbody.linearVelocity.y <= 0.5f;

                if (isAboveHitbox && isNotMovingUpward)
                {
                    player.Bounce(_bounceForce);
                    _enemy?.Die();
                }
            }
        }
    }
}

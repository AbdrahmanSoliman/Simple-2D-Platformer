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
                player.Bounce(_bounceForce);
                _enemy?.Die();
            }
        }
    }
}

using System;
using UnityEngine;

namespace Platformer.Enemies
{
    public class EnemyPatroller : MonoBehaviour, IEnemy
    {
        [Header("Enemy ID")]
        [SerializeField] private int _id;

        [Header("Patrol Settings")]
        [SerializeField] private Transform _patrolPointA;
        [SerializeField] private Transform _patrolPointB;
        [SerializeField] private float _moveSpeed = 3f;

        private Transform _currentTarget;

        public int Id => _id;
        public event Action<IEnemy> OnDied;

        private void Start()
        {
            if (_patrolPointB != null)
            {
                _currentTarget = _patrolPointB;
            }
            else if (_patrolPointA != null)
            {
                _currentTarget = _patrolPointA;
            }
        }

        private void Update()
        {
            if (_currentTarget == null) return;

            Patrol();
        }

        private void Patrol()
        {
            Vector2 targetPosition = new Vector2(_currentTarget.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            float distanceX = _currentTarget.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(distanceX) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            if (Mathf.Abs(distanceX) < 0.05f)
            {
                _currentTarget = (_currentTarget == _patrolPointA) ? _patrolPointB : _patrolPointA;
            }
        }

        public void TakeDamage(int amount)
        {
            Die();
        }

        public void Die()
        {
            OnDied?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}

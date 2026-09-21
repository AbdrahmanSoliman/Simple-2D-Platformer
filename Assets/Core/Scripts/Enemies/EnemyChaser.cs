using System;
using UnityEngine;
using Platformer.Player;

namespace Platformer.Enemies
{
    public class EnemyChaser : MonoBehaviour, IEnemy
    {
        private enum State
        {
            Patrolling,
            Chasing
        }

        [Header("Enemy ID")]
        [SerializeField] private int _id;

        [Header("Patrol Settings")]
        [SerializeField] private Transform _patrolPointA;
        [SerializeField] private Transform _patrolPointB;
        [SerializeField] private float _patrolSpeed = 2.5f;

        [Header("Chase Settings")]
        [SerializeField] private float _chaseSpeed = 4.5f;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _stoppingDistance = 0.75f;
        [SerializeField] private bool _drawDetectionRadius = true;
        [SerializeField] private Transform _player;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _obstacleCheckDistance = 0.15f;

        private State _state = State.Patrolling;
        private Transform _currentTarget;
        private Collider2D _collider;

        public int Id => _id;
        public event Action<IEnemy> OnDied;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            if (_obstacleMask.value == 0)
            {
                _obstacleMask = LayerMask.GetMask("Ground");
            }

            if (_player == null)
            {
                var playerObj = FindFirstObjectByType<PlayerController>();
                if (playerObj != null)
                {
                    _player = playerObj.transform;
                }
            }

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
            float distanceToPlayer = (_player != null) ? Vector2.Distance(transform.position, _player.position) : float.MaxValue;

            switch (_state)
            {
                case State.Patrolling:
                    if (distanceToPlayer <= _detectionRadius)
                    {
                        _state = State.Chasing;
                    }
                    else
                    {
                        Patrol();
                    }
                    break;

                case State.Chasing:
                    if (distanceToPlayer > _detectionRadius)
                    {
                        _state = State.Patrolling;
                        _currentTarget = GetNearestPatrolPoint();
                    }
                    else
                    {
                        Chase();
                    }
                    break;
            }
        }

        private void Patrol()
        {
            if (_currentTarget == null) return;

            float distanceX = _currentTarget.position.x - transform.position.x;
            UpdateFacing(distanceX);

            if (Mathf.Abs(distanceX) < 0.05f || IsBlockedByObstacle(distanceX))
            {
                _currentTarget = (_currentTarget == _patrolPointA) ? _patrolPointB : _patrolPointA;
                return;
            }

            Vector2 targetPosition = new Vector2(_currentTarget.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, _patrolSpeed * Time.deltaTime);
        }

        private void Chase()
        {
            if (_player == null) return;

            float distanceX = _player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) <= _stoppingDistance)
            {
                return;
            }

            UpdateFacing(distanceX);

            if (IsBlockedByObstacle(distanceX))
            {
                return;
            }

            Vector2 targetPosition = new Vector2(_player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, _chaseSpeed * Time.deltaTime);
        }

        private bool IsBlockedByObstacle(float directionX)
        {
            if (_obstacleMask.value == 0) return false;

            Vector2 direction = Vector2.right * Mathf.Sign(directionX);
            float halfWidth = _collider != null ? _collider.bounds.extents.x : 0.5f;
            Vector2 origin = (Vector2)transform.position + direction * halfWidth;

            RaycastHit2D hit = Physics2D.BoxCast(origin, new Vector2(0.1f, 0.8f), 0f, direction, _obstacleCheckDistance, _obstacleMask);
            return hit.collider != null && hit.collider.gameObject != gameObject && !hit.collider.transform.IsChildOf(transform);
        }

        private void UpdateFacing(float directionX)
        {
            if (Mathf.Abs(directionX) > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Sign(directionX) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }

        private Transform GetNearestPatrolPoint()
        {
            if (_patrolPointA == null) return _patrolPointB;
            if (_patrolPointB == null) return _patrolPointA;

            float distA = Vector2.Distance(transform.position, _patrolPointA.position);
            float distB = Vector2.Distance(transform.position, _patrolPointB.position);

            return (distA <= distB) ? _patrolPointA : _patrolPointB;
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

        private void OnDrawGizmosSelected()
        {
            if (!_drawDetectionRadius) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}

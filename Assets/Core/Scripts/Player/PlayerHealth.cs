using System;
using UnityEngine;
using Platformer.Enemies;

namespace Platformer.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int _maxHP = 4;
        [SerializeField] private float _invincibilityDuration = 1f;

        private int _currentHP;
        private float _invincibilityTimer;

        public int CurrentHP => _currentHP;
        public int MaxHP => _maxHP;
        public bool IsDead => _currentHP <= 0;
        public bool IsInvincible => _invincibilityTimer > 0f;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        private void Awake()
        {
            _currentHP = _maxHP;
        }

        private void Update()
        {
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
            }
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || IsInvincible) return;

            _currentHP = Mathf.Max(0, _currentHP - amount);
            _invincibilityTimer = _invincibilityDuration;

            OnHealthChanged?.Invoke(_currentHP, _maxHP);

            if (_currentHP <= 0)
            {
                OnDied?.Invoke();
            }
        }

        public void ResetHP()
        {
            _currentHP = _maxHP;
            _invincibilityTimer = 0f;
            OnHealthChanged?.Invoke(_currentHP, _maxHP);
        }

        public void Restore(int hp)
        {
            _currentHP = Mathf.Clamp(hp, 0, _maxHP);
            _invincibilityTimer = 0f;
            OnHealthChanged?.Invoke(_currentHP, _maxHP);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponentInParent<IEnemy>() != null)
            {
                TakeDamage(1);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<TopHitbox>() != null) return;

            if (other.GetComponentInParent<IEnemy>() != null)
            {
                TakeDamage(1);
            }
        }
    }
}

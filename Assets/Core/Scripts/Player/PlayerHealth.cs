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

        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _invincibleColor;

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

            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Update()
        {
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;

                if (!IsInvincible)
                {
                    UpdateInvincibleVisuals(false);
                }
            }
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || IsInvincible) return;

            _currentHP = Mathf.Max(0, _currentHP - amount);
            _invincibilityTimer = _invincibilityDuration;

            UpdateInvincibleVisuals(IsInvincible);

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
            UpdateInvincibleVisuals(false);
        }

        public void Restore(int hp)
        {
            _currentHP = Mathf.Clamp(hp, 0, _maxHP);
            _invincibilityTimer = 0f;
            OnHealthChanged?.Invoke(_currentHP, _maxHP);
            UpdateInvincibleVisuals(false);
        }

        private void UpdateInvincibleVisuals(bool isInvincible)
        {
            if (isInvincible)
            {
                _spriteRenderer.color = _invincibleColor;
            }
            else
            {
                _spriteRenderer.color = Color.white;
            }
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

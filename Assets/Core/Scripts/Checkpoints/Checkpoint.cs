using System;
using UnityEngine;
using Platformer.Player;

namespace Platformer.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint ID")]
        [SerializeField] private int _id;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _activeColor = Color.green;
        [SerializeField] private Color _inactiveColor = Color.gray;

        public int Id => _id;
        public Vector2 Position => transform.position;
        public bool IsActivated { get; private set; }

        public event Action<Checkpoint> OnActivated;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _inactiveColor;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                if (!IsActivated)
                {
                    Activate();
                }
            }
        }

        public void Activate()
        {
            IsActivated = true;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _activeColor;
            }

            OnActivated?.Invoke(this);
        }
    }
}

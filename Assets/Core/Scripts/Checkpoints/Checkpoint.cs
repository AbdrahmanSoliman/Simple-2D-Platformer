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
            if (IsActivated) return;

            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                Activate();
            }
        }

        public void Activate()
        {
            SetActivated(true);

            OnActivated?.Invoke(this);
        }

        public void SetActivated(bool activated)
        {
            IsActivated = activated;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = activated ? _activeColor : _inactiveColor;
            }
        }
    }
}

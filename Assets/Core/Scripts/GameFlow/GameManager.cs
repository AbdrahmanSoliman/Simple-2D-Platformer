using System;
using UnityEngine;
using Platformer.Player;
using Platformer.Checkpoints;

namespace Platformer.GameFlow
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private CheckpointManager _checkpointManager;

        public event Action OnGameReady;

        private void Awake()
        {
            transform.SetParent(null);

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeScene();
        }

        public void InitializeScene()
        {
            if (_playerController == null)
            {
                _playerController = FindFirstObjectByType<PlayerController>();
            }

            if (_playerHealth == null)
            {
                _playerHealth = FindFirstObjectByType<PlayerHealth>();
            }

            if (_checkpointManager == null)
            {
                _checkpointManager = FindFirstObjectByType<CheckpointManager>();
            }

            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
                _playerHealth.OnDied += HandlePlayerDied;
            }

            OnGameReady?.Invoke();
        }

        private void HandlePlayerDied()
        {
            Vector2 respawnPosition = (_checkpointManager != null) ? _checkpointManager.RespawnPosition : Vector2.zero;

            if (_playerController != null)
            {
                _playerController.Teleport(respawnPosition);
            }

            if (_playerHealth != null)
            {
                _playerHealth.ResetHP();
            }
        }

        public void LevelCompleted()
        {
            Debug.Log("[GameManager] Level Completed!");
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
            }
        }
    }
}

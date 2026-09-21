using System;
using UnityEngine;
using Platformer.Player;
using Platformer.Checkpoints;
using Platformer.Pickups;
using Platformer.SaveLoad;

namespace Platformer.GameFlow
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Player References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerHealth _playerHealth;
        [Header("Tracker References")]
        [SerializeField] private CoinTracker _coinTracker;
        [SerializeField] private DefeatedEnemyTracker _enemyTracker;
        [SerializeField] private CheckpointManager _checkpointManager;

        public bool ShouldLoadSave { get; set; }
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

            if (_coinTracker == null)
            {
                _coinTracker = FindFirstObjectByType<CoinTracker>();
            }

            if (_enemyTracker == null)
            {
                _enemyTracker = FindFirstObjectByType<DefeatedEnemyTracker>();
            }

            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
                _playerHealth.OnDied += HandlePlayerDied;
            }

            if (ShouldLoadSave && SaveLoadManager.HasSave())
            {
                SaveData data = SaveLoadManager.Load();
                if (data != null)
                {
                    RestoreFromSave(data);
                    ShouldLoadSave = false;
                    return;
                }
            }

            OnGameReady?.Invoke();
        }

        public void RestoreFromSave(SaveData data)
        {
            if (data == null) return;

            if (_playerHealth != null)
            {
                _playerHealth.Restore(data.playerHP);
            }

            if (_playerController != null)
            {
                _playerController.Teleport(new Vector2(data.playerX, data.playerY));
            }

            if (_coinTracker != null)
            {
                _coinTracker.Restore(data.collectedCoinIds, data.coinCount);
            }

            if (_enemyTracker != null)
            {
                _enemyTracker.Restore(data.defeatedEnemyIds);
            }

            if (_checkpointManager != null)
            {
                _checkpointManager.Restore(data.lastCheckpointId, new Vector2(data.checkpointX, data.checkpointY));
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

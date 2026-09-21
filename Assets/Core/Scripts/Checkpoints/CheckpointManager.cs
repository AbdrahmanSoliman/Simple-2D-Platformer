using System;
using UnityEngine;
using Platformer.Player;
using Platformer.Pickups;
using Platformer.SaveLoad;

namespace Platformer.Checkpoints
{
    public class CheckpointManager : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform _initialSpawnPoint;

        private Vector2 _respawnPosition;
        private int _activeCheckpointId = -1;

        public Vector2 RespawnPosition => _respawnPosition;
        public int ActiveCheckpointId => _activeCheckpointId;

        public event Action<Checkpoint> OnCheckpointReached;

        private void Awake()
        {
            if (_initialSpawnPoint != null)
            {
                _respawnPosition = _initialSpawnPoint.position;
            }
            else
            {
                _respawnPosition = Vector3.zero;
            }
        }

        private void Start()
        {
            var checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.OnActivated += HandleCheckpointActivated;
            }
        }

        private void HandleCheckpointActivated(Checkpoint checkpoint)
        {
            if (checkpoint.Id <= _activeCheckpointId)
            {
                return;
            }

            _activeCheckpointId = checkpoint.Id;
            _respawnPosition = checkpoint.Position;
            OnCheckpointReached?.Invoke(checkpoint);

            SaveCurrentState(checkpoint);
        }

        private void SaveCurrentState(Checkpoint checkpoint)
        {
            var playerHealth = FindFirstObjectByType<PlayerHealth>();
            var coinTracker = FindFirstObjectByType<CoinTracker>();
            var enemyTracker = FindFirstObjectByType<DefeatedEnemyTracker>();

            var saveData = new SaveData
            {
                playerHP = (playerHealth != null) ? playerHealth.CurrentHP : 4,
                playerX = checkpoint.Position.x,
                playerY = checkpoint.Position.y,
                coinCount = (coinTracker != null) ? coinTracker.Count : 0,
                collectedCoinIds = (coinTracker != null) ? coinTracker.GetCollectedIds() : Array.Empty<int>(),
                defeatedEnemyIds = (enemyTracker != null) ? enemyTracker.GetDefeatedIds() : Array.Empty<int>(),
                lastCheckpointId = checkpoint.Id,
                checkpointX = checkpoint.Position.x,
                checkpointY = checkpoint.Position.y
            };

            SaveLoadManager.Save(saveData);
        }

        public void Restore(int checkpointId, Vector2 position)
        {
            _activeCheckpointId = checkpointId;
            _respawnPosition = position;

            var checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
            foreach (var checkpoint in checkpoints)
            {
                if (checkpoint.Id == checkpointId)
                {
                    checkpoint.Activate();
                }
            }
        }
    }
}

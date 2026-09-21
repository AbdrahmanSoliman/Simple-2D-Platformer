using System;
using UnityEngine;

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
            _activeCheckpointId = checkpoint.Id;
            _respawnPosition = checkpoint.Position;
            OnCheckpointReached?.Invoke(checkpoint);
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

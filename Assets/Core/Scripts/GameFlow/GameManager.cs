using System;
using Platformer.Checkpoints;
using Platformer.Pickups;
using Platformer.Player;
using Platformer.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.GameFlow
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Scene Names")]
        [SerializeField] private string _mainMenuSceneName = "MainMenuScene";
        [SerializeField] private string _gameSceneName = "GameScene";

        [Header("Player References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private PlayerHealth _playerHealth;

        [Header("Tracker References")]
        [SerializeField] private CoinTracker _coinTracker;
        [SerializeField] private DefeatedEnemyTracker _enemyTracker;
        [SerializeField] private CheckpointManager _checkpointManager;

        private bool _loadSave;

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

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == _gameSceneName)
            {
                InitializeScene();
            }
        }

        public void StartNewGame()
        {
            SaveLoadManager.DeleteSave();

            _loadSave = false;

            SceneManager.LoadScene(_gameSceneName);
        }

        public void ContinueGame()
        {
            if (!SaveLoadManager.HasSave())
            {
                Debug.LogWarning("[GameManager] Cannot continue because no save exists.");
                return;
            }

            _loadSave = true;

            SceneManager.LoadScene(_gameSceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == _gameSceneName)
            {
                InitializeScene();
            }
            else if (scene.name == _mainMenuSceneName)
            {
                ClearSceneReferences();
            }
        }

        private void InitializeScene()
        {
            FindSceneObjects();

            SubscribeToPlayerHealth();

            if (_loadSave)
            {
                RestoreGame();
                _loadSave = false;
            }
            else
            {
                OnGameReady?.Invoke();
            }
        }

        private void FindSceneObjects()
        {
            _playerController = FindFirstObjectByType<PlayerController>();
            _playerHealth = FindFirstObjectByType<PlayerHealth>();

            _coinTracker = FindFirstObjectByType<CoinTracker>();
            _enemyTracker = FindFirstObjectByType<DefeatedEnemyTracker>();
            _checkpointManager = FindFirstObjectByType<CheckpointManager>();
        }

        private void SubscribeToPlayerHealth()
        {
            if (_playerHealth == null)
                return;

            _playerHealth.OnDied -= HandlePlayerDied;
            _playerHealth.OnDied += HandlePlayerDied;
        }

        private void RestoreGame()
        {
            SaveData data = SaveLoadManager.Load();

            if (data == null)
            {
                Debug.LogWarning("[GameManager] Save could not be loaded. Starting a new game.");
                OnGameReady?.Invoke();
                return;
            }

            if (data.isLevelCompleted)
            {
                Debug.Log("[GameManager] Save belongs to a completed level.");

                OnGameReady?.Invoke();
                return;
            }

            RestoreFromSave(data);
        }

        public void RestoreFromSave(SaveData data)
        {
            if (data == null)
                return;

            if (_playerHealth != null)
            {
                _playerHealth.Restore(data.playerHP);
            }

            if (_playerController != null)
            {
                _playerController.Teleport(
                    new Vector2(data.playerX, data.playerY)
                );
            }

            if (_coinTracker != null)
            {
                _coinTracker.Restore(
                    data.collectedCoinIds,
                    data.coinCount
                );
            }

            if (_enemyTracker != null)
            {
                _enemyTracker.Restore(data.defeatedEnemyIds);
            }

            if (_checkpointManager != null)
            {
                _checkpointManager.Restore(
                    data.lastCheckpointId,
                    new Vector2(data.checkpointX, data.checkpointY)
                );
            }

            OnGameReady?.Invoke();
        }

        private void HandlePlayerDied()
        {
            Vector2 respawnPosition =
                _checkpointManager != null
                    ? _checkpointManager.RespawnPosition
                    : Vector2.zero;

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
            if (SaveLoadManager.HasSave())
            {
                SaveData data = SaveLoadManager.Load();

                if (data != null)
                {
                    data.isLevelCompleted = true;

                    if (_playerHealth != null)
                    {
                        data.playerHP = _playerHealth.CurrentHP;
                    }

                    if (_playerController != null)
                    {
                        Vector2 position = _playerController.transform.position;
                        data.playerX = position.x;
                        data.playerY = position.y;
                    }

                    if (_coinTracker != null)
                    {
                        data.coinCount = _coinTracker.Count;
                        data.collectedCoinIds = _coinTracker.GetCollectedIds();
                    }

                    if (_enemyTracker != null)
                    {
                        data.defeatedEnemyIds = _enemyTracker.GetDefeatedIds();
                    }

                    if (_checkpointManager != null)
                    {
                        data.lastCheckpointId = _checkpointManager.ActiveCheckpointId;

                        Vector2 checkpointPosition =
                            _checkpointManager.RespawnPosition;

                        data.checkpointX = checkpointPosition.x;
                        data.checkpointY = checkpointPosition.y;
                    }

                    SaveLoadManager.Save(data);
                }
            }

            Debug.Log("[GameManager] Level Completed!");

            SceneManager.LoadScene(_mainMenuSceneName);
        }

        private void ClearSceneReferences()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
            }

            _playerController = null;
            _playerHealth = null;
            _coinTracker = null;
            _enemyTracker = null;
            _checkpointManager = null;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
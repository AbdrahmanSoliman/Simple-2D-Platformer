using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Platformer.Player;
using Platformer.Pickups;
using Platformer.GameFlow;

namespace Platformer.UI
{
    public class HudUI : MonoBehaviour
    {
        [Header("HP Display")]
        [SerializeField] private Image[] _hpIcons;

        [Header("Coin Display")]
        [SerializeField] private TMP_Text _coinText;

        [Header("Scene References")]
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private CoinTracker _coinTracker;

        private void Start()
        {
            FindSceneReferences();
            SubscribeEvents();
            RefreshAll();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void FindSceneReferences()
        {
            if (_playerHealth == null)
            {
                _playerHealth = FindFirstObjectByType<PlayerHealth>();
            }

            if (_coinTracker == null)
            {
                _coinTracker = FindFirstObjectByType<CoinTracker>();
            }
        }

        private void SubscribeEvents()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= UpdateHP;
                _playerHealth.OnHealthChanged += UpdateHP;
            }

            if (_coinTracker != null)
            {
                _coinTracker.OnCoinsChanged -= UpdateCoins;
                _coinTracker.OnCoinsChanged += UpdateCoins;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameReady -= RefreshAll;
                GameManager.Instance.OnGameReady += RefreshAll;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= UpdateHP;
            }

            if (_coinTracker != null)
            {
                _coinTracker.OnCoinsChanged -= UpdateCoins;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameReady -= RefreshAll;
            }
        }

        public void UpdateHP(int currentHP, int maxHP)
        {
            if (_hpIcons == null) return;

            for (int i = 0; i < _hpIcons.Length; i++)
            {
                if (_hpIcons[i] != null)
                {
                    _hpIcons[i].enabled = (i < currentHP);
                }
            }
        }

        public void UpdateCoins(int count)
        {
            if (_coinText != null)
            {
                _coinText.text = $"x {count}";
            }
        }

        public void RefreshAll()
        {
            FindSceneReferences();

            if (_playerHealth != null)
            {
                UpdateHP(_playerHealth.CurrentHP, _playerHealth.MaxHP);
            }

            if (_coinTracker != null)
            {
                UpdateCoins(_coinTracker.Count);
            }
        }
    }
}

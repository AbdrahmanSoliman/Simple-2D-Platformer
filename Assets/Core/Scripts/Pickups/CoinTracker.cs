using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Pickups
{
    public class CoinTracker : MonoBehaviour
    {
        private readonly HashSet<int> _collectedIds = new();
        private int _count;

        public int Count => _count;
        public event Action<int> OnCoinsChanged;

        private void Start()
        {
            var coins = FindObjectsByType<CoinPickup>(FindObjectsSortMode.None);
            foreach (var coin in coins)
            {
                coin.OnCollected += HandleCoinCollected;
            }
        }

        private void HandleCoinCollected(CoinPickup coin)
        {
            _collectedIds.Add(coin.Id);
            _count++;
            OnCoinsChanged?.Invoke(_count);
            Debug.Log($"Coin with ID: {coin.Id} collected");
        }

        public int[] GetCollectedIds()
        {
            var array = new int[_collectedIds.Count];
            _collectedIds.CopyTo(array);
            return array;
        }

        public void Restore(int[] ids, int count)
        {
            _collectedIds.Clear();
            if (ids != null)
            {
                foreach (int id in ids)
                {
                    _collectedIds.Add(id);
                }
            }

            _count = count;

            var coins = FindObjectsByType<CoinPickup>(FindObjectsSortMode.None);
            foreach (var coin in coins)
            {
                if (_collectedIds.Contains(coin.Id))
                {
                    coin.gameObject.SetActive(false);
                }
            }

            OnCoinsChanged?.Invoke(_count);
        }
    }
}

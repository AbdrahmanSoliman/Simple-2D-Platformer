using System;
using UnityEngine;
using Platformer.Player;

namespace Platformer.Pickups
{
    public class CoinPickup : MonoBehaviour
    {
        [Header("Coin ID")]
        [SerializeField] private int _id;

        public int Id => _id;
        public event Action<CoinPickup> OnCollected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                OnCollected?.Invoke(this);
                gameObject.SetActive(false);
            }
        }
    }
}

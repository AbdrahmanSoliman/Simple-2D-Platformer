using UnityEngine;
using Platformer.Player;

namespace Platformer.GameFlow
{
    public class LevelEndTrigger : MonoBehaviour
    {
        private bool _triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered) return;

            if (other.GetComponent<PlayerController>() != null || other.CompareTag("Player"))
            {
                _triggered = true;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LevelCompleted();
                }
                else
                {
                    Debug.Log("[LevelEndTrigger] Level Completed (GameManager instance not found)!");
                }
            }
        }
    }
}

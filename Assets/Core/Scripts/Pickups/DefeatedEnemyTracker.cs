using System.Collections.Generic;
using UnityEngine;
using Platformer.Enemies;

namespace Platformer.Pickups
{
    public class DefeatedEnemyTracker : MonoBehaviour
    {
        private readonly HashSet<int> _defeatedIds = new();

        private void Start()
        {
            var patrollers = FindObjectsByType<EnemyPatroller>(FindObjectsSortMode.None);
            foreach (var enemy in patrollers)
            {
                enemy.OnDied += HandleEnemyDied;
            }

            var chasers = FindObjectsByType<EnemyChaser>(FindObjectsSortMode.None);
            foreach (var enemy in chasers)
            {
                enemy.OnDied += HandleEnemyDied;
            }
        }

        private void HandleEnemyDied(IEnemy enemy)
        {
            _defeatedIds.Add(enemy.Id);
            Debug.Log($"Enemy with ID: {enemy.Id} defeated");
        }

        public int[] GetDefeatedIds()
        {
            var array = new int[_defeatedIds.Count];
            _defeatedIds.CopyTo(array);
            return array;
        }

        public void Restore(int[] ids)
        {
            _defeatedIds.Clear();
            if (ids == null) return;

            foreach (int id in ids)
            {
                _defeatedIds.Add(id);
            }

            var patrollers = FindObjectsByType<EnemyPatroller>(FindObjectsSortMode.None);
            foreach (var enemy in patrollers)
            {
                if (_defeatedIds.Contains(enemy.Id))
                {
                    enemy.gameObject.SetActive(false);
                }
            }

            var chasers = FindObjectsByType<EnemyChaser>(FindObjectsSortMode.None);
            foreach (var enemy in chasers)
            {
                if (_defeatedIds.Contains(enemy.Id))
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }
    }
}

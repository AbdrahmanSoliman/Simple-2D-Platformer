using System;

namespace Platformer.Enemies
{
    public interface IEnemy
    {
        int Id { get; }
        void TakeDamage(int amount);
        void Die();
        event Action<IEnemy> OnDied;
    }
}

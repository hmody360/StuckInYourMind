using UnityEngine;

public interface IDamageable // Interface Script for Damageables (Player and Enemy)
{
    public void takeDamage(float damage);
    public void onDeath();
}

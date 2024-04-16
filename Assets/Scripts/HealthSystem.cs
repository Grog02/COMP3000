using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public static event Action OnLose;
    public static event Action OnWin;

    public enum HealthOwnerType
    {
        Player,
        Enemy
    }

    [SerializeField] private HealthOwnerType ownerType;


    private int playersDead = 0;
    public int enemiesDead = 0;
    /*private bool lose = false;
    private bool win = false;*/
    [SerializeField] private int health = 100;
    private int healthMax;
    private EnemiesLeft enemiesLeftScript;

    private void Awake()
    {
        healthMax = health;
        enemiesLeftScript = FindObjectOfType<EnemiesLeft>();
        if (enemiesLeftScript == null)
        {
            Debug.Log("EnemiesLeft script not found in the scene.");
        }
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            health = 0;
            OnDamaged?.Invoke(this, EventArgs.Empty);
            Die();
        }
        else
        {
            OnDamaged?.Invoke(this, EventArgs.Empty);
        }

        Debug.Log(health);
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        health = Mathf.Min(health, healthMax); // Ensure health does not exceed maximum

        // Optionally, invoke an event or perform additional actions here
        OnDamaged?.Invoke(this, EventArgs.Empty);
        Debug.Log("Healed for " +  healAmount);

        Debug.Log(health);
    }

    private void Die()
    {
        // Check if the owner is an enemy and handle accordingly
        if (ownerType == HealthOwnerType.Enemy)
        {
            // This is an enemy
            enemiesDead++;
            Debug.Log("An enemy has died.");
            if (enemiesDead == 9)
            {
                OnWin?.Invoke();
            }
            if (enemiesLeftScript != null)
            {
                enemiesLeftScript.DecreaseEnemyCount(); // Call the public method in EnemiesLeft
            }
        }
        else 
        {
            Debug.Log("Player has died.");
            playersDead++;
            if(playersDead == 4)
            {
                OnLose?.Invoke();
            }
            if (enemiesLeftScript != null)
            {
                enemiesLeftScript.DecreaseUnitCount(); // Call the public method in EnemiesLeft
            }
        }

        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalised()
    {
        return (float)health / healthMax;
    }

    
}

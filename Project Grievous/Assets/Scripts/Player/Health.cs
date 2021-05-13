using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    public int currentHealth { get; set; }
    public bool invincible { get; set; }

    public static event Action<float> onHealthUpdate;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        GameManager.OnGameStateChanged += GameStateChanged;
    }

    public void TakeDamage(int damage, GameObject damageSource)
    {
        if (invincible)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthUpdate?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.GameOver);
        }
    }

    private void GameStateChanged(GameManager.GameState state)
    {
        onHealthUpdate?.Invoke(currentHealth);
    }
}

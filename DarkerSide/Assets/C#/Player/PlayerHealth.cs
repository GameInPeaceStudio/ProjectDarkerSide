using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerHealth=100f;
    // Start is called before the first frame update
    public void TakeDamage(float BotDealDamage)
    {
        playerHealth -= BotDealDamage;
        Debug.Log(playerHealth);
        if (playerHealth <= 0) 
        {
            Die();
        }
    }
    void Die()
    {

        Destroy(gameObject);
    }
}

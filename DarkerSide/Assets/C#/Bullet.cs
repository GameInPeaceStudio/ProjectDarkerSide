using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float damageAmount = 20f;
    public float destroyAfterSecond = 2f;

    private void Start()
    {
        
        StartCoroutine(DestroyAfterSeconds());
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        HealtDamage bot = collision.gameObject.GetComponent<HealtDamage>();
        if (bot != null)
        {
            // Düşmanın canını azalt
            bot.TakeDamage(damageAmount);

            // Mermiyi yok et
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroyAfterSecond);
        Destroy(gameObject);
    }


}

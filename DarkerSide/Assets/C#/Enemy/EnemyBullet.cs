using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    public float damageAmount = 20f;
    public float destroyAfterSecond = 2f;
    
    


    private void Start()
    {

        StartCoroutine(DestroyAfterSeconds());
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(destroyAfterSecond);
        Destroy(gameObject);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {

        PlayerHealth Player = collision.gameObject.GetComponent<PlayerHealth>();// player objesine bagli script


        if (Player != null)
        {

            Player.TakeDamage(damageAmount);

        }
    }


}

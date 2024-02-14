using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    public Transform player; // Oyuncunun transform bileşeni
    public GameObject bulletPrefab; // Mermi prefabı
    public Transform firePoint; // Mermi ateşleme noktası
    public float bulletSpeed = 20f; 
    public float fireRate = 1f; // Ateş hızı (saniyede ateş edilen mermi sayısı)
    private float lastFireTime = 0f; // Bir sonraki ateş zamanı

    void FixedUpdate()
    {
        if(player != null)
        {
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            direction.z = 0f;
        }
        // Oyuncuya doğru yönelme
        


        // Ateş etme
        if (Time.time > lastFireTime + fireRate)
        {
            lastFireTime = Time.time;
            //nextFireTime = Time.time / fireRate;
            Fire();
        }
    }

    void Fire()
    {
        // Mermi oluşturma ve ateşleme
        GameObject AirDefanceBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        AirDefanceBullet.GetComponent<Rigidbody2D>().velocity = AirDefanceBullet.transform.up*bulletSpeed;

    }
}

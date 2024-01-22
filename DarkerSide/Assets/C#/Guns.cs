using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    public float rotationSpeed = 5f;
    
    
    public float bulletSpeed;
    public float fireRate = 0.5f;
    public float lastFireTime;
    public GameObject bulletPrefab;
    public GameObject BulletSpawnPosition;
    

    private Rigidbody2D Bulletrb;
    // Start is called before the first frame update
    void Start()
    {
        Bulletrb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RotateTowards();
        Fire();

    }
    void RotateTowards()
    {

        if (Input.GetMouseButton(0))
        {
            // Fare pozisyonunu al
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 direction = mousePosition - transform.position;

            direction.z = 0f; // 2D oyun olduğu için z eksenini sıfıra ayarladim

            // Hedefe doğru dönme işlemi

            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        }


    }
    void Fire()
    {
        if (Input.GetMouseButton(0)&&Time.time>lastFireTime+fireRate)
        {
           SpawnBullet();
           lastFireTime = Time.time;
        }
    }

    void SpawnBullet()
    {
        
        Vector3 spawnPosition = BulletSpawnPosition.transform.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);

        // Opsiyonel: Mermi objesine hız eklemek istiyorsanız Rigidbody2D bileşenini kullanabilirsiniz
        bullet.GetComponent<Rigidbody2D>().velocity = transform.up * bulletSpeed;
    }
}

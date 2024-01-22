using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    // Döneceğimiz hız
    public float rotationSpeed = 5f;
    public float thrustForce = 1f;
    public float thrustForceMin = 1f;
    public float thrustForceMax = 5f;


    private Rigidbody2D rb;

  


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }
    void Update()
    {
        RotateTowards();
        AddForce();
        
    }

    void RotateTowards()
    {

        if (Input.GetMouseButton(1))
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
    void AddForce()
    {
        if (Input.GetMouseButton(1))
        {
            // uzay gemisine itme gucu ekledim

            rb.AddForce(transform.up * thrustForce * Time.deltaTime);
            if (thrustForce <= thrustForceMax)
            {
                thrustForce++;
            }
        }
        // sag mouse tiklanmadiginda itme gucu yok.
        else
        {
            thrustForce = 0;
        }
    }

    










}
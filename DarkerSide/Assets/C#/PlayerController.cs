using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Döneceğimiz hız
    public float rotationSpeed = 5f;
    public float thrustForce = 1f;
    public float fullSpeedForce = 1f;
    public float maxSpeed = 5f;

    bool isMoving = false;

    private Rigidbody2D rb;

  


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }
    void FixedUpdate()
    {
        AddForce();
    }

    void RotateCharacter(Vector2 moveDirection)
    {

        if (moveDirection != Vector2.zero)
        {
            // Hareket vektörüne göre karakterin rotasyonunu belirle
            float targetRotation = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;

            // Dönüşü yumuşatmak için slerp kullan
            float rotation = Mathf.LerpAngle(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Rotasyonu uygula
            rb.SetRotation(rotation);
            rb.angularVelocity = 0f;

            isMoving = true;
            ///

        }
        else
        {
            if (isMoving)
            {
                rb.angularVelocity = 0f;
                isMoving = false;
            }
        }

    }
    void AddForce()
    {
        // dikey ve yatay inputlar
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection=new Vector2(horizontalInput, verticalInput).normalized;

        // moveDirection belirlendigine göre artik Force ekleyebiliriz
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * thrustForce);

            if (Input.GetButton("fullSpeed"))
            {
                rb.AddForce(moveDirection * (thrustForce + fullSpeedForce));
            }
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
            //Debug.Log("MaxSpeed");
        }
        else
        {
            rb.AddForce(moveDirection * thrustForce);
        }

        if (Input.GetButtonDown("Dash"))
        {
            rb.MovePosition(rb.position + moveDirection.normalized * 50f);
            Debug.Log("Dash");
        }
        
     
        RotateCharacter(moveDirection);

    }

  

}



///////////////////////////////////////////////////////////////////////////////
/*
 * mouse pozisyonuna dogru bak
 * 
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
*/


///////////////////////////////////////////////////////////////////////////
/* Add force
 *
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

 */
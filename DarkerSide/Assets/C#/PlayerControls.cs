using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Döneceğimiz hız
    public float rotationSpeed = 5f;
    public float thrustForce = 1f;
    public float fullSpeedForce = 1f;
    public float speedLimiter = 5f;
    public float dashDistance = 50f;

    public Rigidbody2D rb;
    public bool isRotating=false;

    


    public void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
       


        Skills(moveDirection);
    }
    void FixedUpdate()
    {
        // triggers
        

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        
        if (moveDirection != Vector2.zero)
        {
            RotateCharacter(moveDirection);
            
            AddForce(moveDirection);
        }

    }


    void RotateCharacter(Vector2 moveDirection)
    {

        if (moveDirection != Vector2.zero)
        {
           
            float targetRotation = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            float rotation = Mathf.LerpAngle(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            rb.SetRotation(rotation);

            rb.angularVelocity = 0f;  
            isRotating = true;
                
        }
        else
        {
            
            if (isRotating)
            {
                rb.angularVelocity = 0f;
                isRotating = false;

            }
            
        }

    }
    
    void AddForce(Vector2 moveDirection)
    {
        
        if (moveDirection != Vector2.zero)
        {
            
            rb.AddForce(moveDirection * thrustForce);
            //float rightTriggerValue = InputSystem.GetDevice<Gamepad>().rightTrigger.ReadValue();
            Gamepad gamepad = InputSystem.GetDevice<Gamepad>();
            if (gamepad != null)
            {
                float leftTriggerValue = InputSystem.GetDevice<Gamepad>().leftTrigger.ReadValue();
                if(leftTriggerValue != 0f) 
                {
                    rb.AddForce(moveDirection * (thrustForce + fullSpeedForce));
                }
            }

            
            if (Input.GetButton("fullSpeed"))
            {
                rb.AddForce(moveDirection * (thrustForce + fullSpeedForce));

            }
            if (rb.velocity.magnitude > speedLimiter)
            {
                rb.velocity = rb.velocity.normalized * speedLimiter;
                //Debug.Log(rb.velocity.magnitude);
            }
        }

    }

    void Skills(Vector2 moveDirection)
    {

        if (Input.GetButtonDown("Dash"))
        {
            rb.MovePosition(rb.position + moveDirection.normalized * dashDistance);
            Debug.Log("Dash");
        }
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
using System.Collections;
using UnityEngine;


public class Bot : MonoBehaviour
{
    
    public Rigidbody2D rb;
    private Transform player;
    public float speedLimiter=150;
    public float addForce = 5f;

    public float BotDealDamage = 10f;
    private bool canDealDamage = true;
    public float damageCooldown = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if(player != null)
        {
            float distaceToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 moveDirection = (player.position - transform.position).normalized;

            if (rb.velocity.magnitude <= speedLimiter)
            {
                rb.AddForce(moveDirection * (addForce * distaceToPlayer * Time.deltaTime));
            }
            else 
            {
                rb.velocity = moveDirection * speedLimiter;
                
            }
           
            
            
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (canDealDamage)
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                
                player.TakeDamage(BotDealDamage);
                StartCoroutine(DamageCooldown());
            }
        }
        
    }

    public IEnumerator DamageCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
    }



}

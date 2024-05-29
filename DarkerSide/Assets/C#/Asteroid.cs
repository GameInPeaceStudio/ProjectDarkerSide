using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float initialForceMagnitude = 10f;  // Başlangıç kuvvetinin büyüklüğü

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rastgele bir yön seç
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Başlangıç kuvvetini uygula
        rb.AddForce(randomDirection * initialForceMagnitude, ForceMode2D.Impulse);
    }
}
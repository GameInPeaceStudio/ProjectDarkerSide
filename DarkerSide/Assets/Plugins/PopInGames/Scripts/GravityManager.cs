using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public float gravitationalConstant = 0.1f;  // Çekim sabiti
    private List<Rigidbody2D> rigidbodies;

    void Start()
    {
        // Tüm Rigidbody2D bileşenlerini bul ve listeye ekle
        rigidbodies = new List<Rigidbody2D>(FindObjectsOfType<Rigidbody2D>());
    }

    void FixedUpdate()
    {
        // Her bir rigidbody için çekim kuvvetini uygula
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            for (int j = 0; j < rigidbodies.Count; j++)
            {
                if (i == j) continue;  // Kendine kuvvet uygulama

                ApplyGravity(rigidbodies[i], rigidbodies[j]);
            }
        }
    }

    void ApplyGravity(Rigidbody2D body1, Rigidbody2D body2)
    {
        Vector2 direction = body2.position - body1.position;
        float distance = direction.magnitude;

        // Küçük mesafelerde sonsuz çekim kuvvetini önlemek için minumum mesafe
        if (distance < 0.1f) return;

        float forceMagnitude = gravitationalConstant * (body1.mass * body2.mass) / Mathf.Pow(distance, 2);
        Vector2 force = direction.normalized * forceMagnitude;

        body1.AddForce(force);
    }
}

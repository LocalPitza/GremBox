using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FoodPellet : MonoBehaviour
{
    public float nutritionValue = 15f;
    public float lifeTime = 10f;
    public ParticleSystem eatEffect;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CreatureNeeds>(out var creature))
        {
            creature.Feed(nutritionValue);
            if (eatEffect != null) Instantiate(eatEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

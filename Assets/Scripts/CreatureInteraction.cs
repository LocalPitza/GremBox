using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CreatureNeeds))]
public class CreatureInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float petAmount = 10f;
    public ParticleSystem happyParticles;

    private CreatureNeeds needs;

    void Start()
    {
        needs = GetComponent<CreatureNeeds>();
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0)) // Right-click
        {
            Pet();
        }
    }

    public void Pet()
    {
        needs.Pet(petAmount);
        if (happyParticles != null) happyParticles.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureElevation : MonoBehaviour
{
    [Header("Height Settings")]
    public float baseHeight = 0f;
    public float heightVariation = 0.3f;
    public float stepHeight = 0.05f;
    public float settleSpeed = 8f;

    [Header("Sorting")]
    public int baseSortingOrder = 0;
    public int sortingMultiplier = 100;

    private float randomHeightOffset;
    private float currentStepOffset;
    private CreatureMovement movement;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        movement = GetComponent<CreatureMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Safer random offset
        randomHeightOffset = Mathf.Clamp(
            Random.Range(-heightVariation, heightVariation),
            -heightVariation * 0.8f,
            heightVariation * 0.8f
        );
    }

    void LateUpdate()
    {
        UpdateMovementHeight();
        UpdateSorting();
    }

    void UpdateMovementHeight()
    {
        float targetStepOffset = movement.IsMoving() ? stepHeight : 0f;
        currentStepOffset = Mathf.Lerp(
            currentStepOffset,
            targetStepOffset,
            (movement.IsMoving() ? 5f : settleSpeed) * Time.deltaTime
        );

        Vector3 pos = transform.position;
        pos.y = baseHeight + randomHeightOffset + currentStepOffset;
        transform.position = pos;
    }

    void UpdateSorting()
    {
        // Positive = higher in scene (rendered on top)
        float sortPriority = (randomHeightOffset + currentStepOffset);
        spriteRenderer.sortingOrder = baseSortingOrder + 
            Mathf.RoundToInt(sortPriority * sortingMultiplier);
        
        // Debug visualization
        Debug.DrawLine(
            transform.position,
            new Vector3(transform.position.x, baseHeight, transform.position.z),
            sortPriority > 0 ? Color.green : Color.red
        );
    }
}

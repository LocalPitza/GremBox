using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CreatureMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public Bounds walkableArea;

    [Header("Overlap Reaction")]
    [Range(0, 100)] public float reactionChance = 30f;
    public float reactionCooldown = 2f;

    [Header("Personality Effects")]
    public float baseMoveSpeed; // Set this in Start()
    public float baseReactionChance;

    [Header("Animation")]
    public string walkAnimBool = "IsWalking";
    
    private Vector2 targetPosition;
    private float idleTimer;
    private float lastReactionTime;
    private Animator anim;
    private SpriteRenderer spriteRenderer; // For flipping
    private Vector2 previousPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
        PickNewTarget();
        baseMoveSpeed = moveSpeed;
        baseReactionChance = reactionChance;
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            idleTimer -= Time.deltaTime;
            anim.SetBool(walkAnimBool, false);
            if (idleTimer <= 0) PickNewTarget();
        }
        else
        {
            MoveTowardTarget();
            UpdateFacingDirection(currentPosition);
            anim.SetBool(walkAnimBool, true);
        }

        previousPosition = currentPosition;
    }

    void MoveTowardTarget()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    void UpdateFacingDirection(Vector2 currentPosition)
    {
        float moveDirection = currentPosition.x - previousPosition.x;
        
        if (Mathf.Abs(moveDirection) > 0.01f)
        {
            spriteRenderer.flipX = moveDirection > 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Creature") && 
            Time.time > lastReactionTime + reactionCooldown)
        {
            if (Random.Range(0, 100) < reactionChance)
            {
                PickNewTarget();
                lastReactionTime = Time.time;
                Debug.Log($"{name} reacted to overlap!");
            }
        }
    }

    void PickNewTarget()
    {
        targetPosition = new Vector2(
            Random.Range(walkableArea.min.x, walkableArea.max.x),
            walkableArea.min.y
        );
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(walkableArea.center, walkableArea.size);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector2(walkableArea.min.x, walkableArea.min.y),
            new Vector2(walkableArea.max.x, walkableArea.min.y)
        );
    }

    public bool IsMoving()
    {
        return Vector2.Distance(transform.position, targetPosition) > 0.1f;
    }

    public Vector2 GetMoveDirection()
    {
        return (targetPosition - (Vector2)transform.position).normalized;
    }
}

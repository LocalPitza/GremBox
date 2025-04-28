using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class FoodPelletSystem : MonoBehaviour
{
    [Header("Settings")]
    public GameObject foodPelletPrefab;
    public float pelletCost = 5f;
    public LayerMask groundLayer;
        
    [Header("UI")]
    public TMP_Text feedModeText;
    public Button feedModeButton;

    [Header("Feedback")]
    public AudioClip dropSound;
    public ParticleSystem dropEffect;

    private bool isFeedingMode = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateFeedModeUI();
    }

    public void ToggleFeedingMode()
    {
        isFeedingMode = !isFeedingMode;
        UpdateFeedModeUI();
        Debug.Log($"Feeding mode: {isFeedingMode}");
    }

    void Update()
    {
        if (!isFeedingMode) return;

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            EconomyManager.Instance.TrySpendMoney(pelletCost);
            TryDropPellet();
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void TryDropPellet()
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0; // Important for 2D

        // Visual debug
        Debug.DrawRay(worldPos, Vector2.down * 0.5f, Color.green, 1f);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.down, 0.5f, groundLayer);
        if (hit.collider != null)
        {
            SpawnPellet(hit.point);
        }
        else
        {
            // Fallback: spawn at cursor position
            SpawnPellet(worldPos);
        }
    }

    void SpawnPellet(Vector2 position)
    {
        GameObject pellet = Instantiate(foodPelletPrefab, position, Quaternion.identity);
        
        // 2D Physics
        Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(new Vector2(
                Random.Range(-0.5f, 0.5f),
                3f
            ), ForceMode2D.Impulse);
        }

        // Effects
        if (dropEffect != null) Instantiate(dropEffect, position, Quaternion.identity);
        if (dropSound != null) AudioSource.PlayClipAtPoint(dropSound, position);
    }
    void UpdateFeedModeUI()
    {
        if (feedModeText != null)
            feedModeText.text = isFeedingMode ? "FEEDING MODE (LMB)" : $"FEED (${pelletCost})";
        
        if (feedModeButton != null)
            feedModeButton.image.color = isFeedingMode ? Color.green : Color.white;
    }
}

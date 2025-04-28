using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ShopButton : MonoBehaviour
{
    [System.Serializable]
    public class CreatureTier
    {
        public GameObject prefab;
        [Tooltip("Higher weight = more common")]
        [Range(1, 100)] 
        public float spawnWeight = 10f;
        public Color rarityColor = Color.white;
        public int baseCost = 50; // Base cost for this tier
    }

    [Header("Spawning")]
    public List<CreatureTier> creatureTiers = new List<CreatureTier>();
    public Transform spawnPoint;
    public float spawnPulseScale = 1.2f;
    public float spawnPulseDuration = 0.5f;
        
    [Header("Economy")]
    public int costIncreasePerPurchase = 5;
    public int maxCost = 200;
    
    [Header("UI")]
    public TMP_Text buyText;

    [Header("Rare Spawn Effects")]
    public ParticleSystem rareSpawnEffect;
    public AudioClip rareSpawnSound;
    [Tooltip("Weights below this are considered rare")]
    public float rareThreshold = 15f; 

    private float totalSpawnWeight;
    private int purchaseCount = 0;
    private int currentCost;

    void Start()
    {
        totalSpawnWeight = creatureTiers.Sum(t => t.spawnWeight);
        currentCost = GetBaseCreatureCost();
        UpdateBuyButtonUI();
    }

    public void OnBuyAttempt()
    {
        if (EconomyManager.Instance.TrySpendMoney(currentCost))
        {
            StartCoroutine(SpawnCreatureRoutine());
        }
    }

    IEnumerator SpawnCreatureRoutine()
    {
        // Select creature
        GameObject creatureToSpawn = SelectCreatureByWeight();
        if (creatureToSpawn == null)
        {
            Debug.LogError("No creature prefab selected!");
            yield break;
        }

        // Instantiate
        GameObject newCreature = Instantiate(creatureToSpawn, spawnPoint.position, Quaternion.identity);
        
        // Wait for components to initialize
        yield return null; 

        // Configure
        ConfigureCreaturePersonality(newCreature);
        PlayRarityEffects(creatureToSpawn);
        StartCoroutine(PulseCreature(newCreature.transform));

        // Register with journal
        if (CreatureJournal.Instance != null)
        {
            CreatureJournal.Instance.RegisterCreature(newCreature);
        }
        else
        {
            Debug.LogError("CreatureJournal instance not found!");
        }

        // Update economy
        purchaseCount++;
        currentCost = Mathf.Min(
            GetBaseCreatureCost() + (costIncreasePerPurchase * purchaseCount),
            maxCost
        );
        UpdateBuyButtonUI();
    }

    int GetBaseCreatureCost()
    {
        float totalCost = 0f;
        foreach (var tier in creatureTiers)
        {
            float probability = tier.spawnWeight / totalSpawnWeight;
            totalCost += tier.baseCost * probability;
        }
        return Mathf.RoundToInt(totalCost);
    }

    void UpdateBuyButtonUI(float currentMoney = -1)
    {
        if (currentMoney < 0) currentMoney = EconomyManager.Instance.currentMoney;
        buyText.text = $"Get New Grem\n${currentCost}";
    }

    GameObject SelectCreatureByWeight()
    {
        if (creatureTiers.Count == 0)
        {
            Debug.LogError("No creatures assigned in tiers!");
            return null;
        }

        float randomPoint = Random.Range(0f, totalSpawnWeight);
        float cumulativeWeight = 0f;

        foreach (var tier in creatureTiers.OrderBy(t => t.spawnWeight))
        {
            cumulativeWeight += tier.spawnWeight;
            if (randomPoint <= cumulativeWeight)
            {
                return tier.prefab;
            }
        }

        return creatureTiers[0].prefab; // Fallback
    }

    void PlayRarityEffects(GameObject spawnedPrefab)
    {
        var tier = creatureTiers.Find(t => t.prefab == spawnedPrefab);
        if (tier == null || tier.spawnWeight >= rareThreshold) return;

        if (rareSpawnEffect != null)
            Instantiate(rareSpawnEffect, spawnPoint.position, Quaternion.identity);
        
        if (rareSpawnSound != null)
            AudioSource.PlayClipAtPoint(rareSpawnSound, spawnPoint.position);
    }

    void ConfigureCreaturePersonality(GameObject creature)
    {
        if (creature.TryGetComponent<CreatureNeeds>(out var needs))
        {
            needs.RandomizePersonality();
        }
        else
        {
            Debug.LogError("Creature missing CreatureNeeds component!", creature);
        }
    }

    IEnumerator PulseCreature(Transform creature)
    {
        float timer = 0;
        Vector3 originalScale = creature.localScale;
        
        while (timer < spawnPulseDuration)
        {
            float pulse = Mathf.PingPong(timer / spawnPulseDuration * 2f, 1f);
            creature.localScale = originalScale * (1f + (pulse * (spawnPulseScale - 1f)));
            timer += Time.deltaTime;
            yield return null;
        }
        creature.localScale = originalScale;
    }

    // Debug helper
    [ContextMenu("Print Spawn Weights")]
    void PrintSpawnWeights()
    {
        foreach (var tier in creatureTiers)
        {
            float probability = (tier.spawnWeight / totalSpawnWeight) * 100;
            Debug.Log($"{tier.prefab.name}: {probability:F1}% chance");
        }
    }
}

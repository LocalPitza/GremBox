using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Settings")]
    public float startingMoney = 100.0f; // Explicit float
    public float passiveIncomeInterval = 5.0f;
    public float incomePerCreature = 1.5f;
    
    [Header("Callbacks")]
    public UnityEvent<float> onMoneyChanged = new UnityEvent<float>();

    [Header("UI")]
    public TMP_Text moneyText;
    public string moneyFormat = "F1"; // Shows 1 decimal (e.g., "$25.5")

    [Header("Creature Costs")]
    public float baseCreatureCost = 50f;
    public float costIncreasePerCreature = 5f;
    
    public float creatureCost {
        get {
            int creatureCount = FindObjectsOfType<CreatureNeeds>().Length;
            return baseCreatureCost + (creatureCount * costIncreasePerCreature);
        }
    }
    public bool CanAffordCreature() => currentMoney >= creatureCost;

    private float _currentMoney;
    public float currentMoney {
        get => _currentMoney;
        private set {
            _currentMoney = value;
            onMoneyChanged.Invoke(_currentMoney);
            UpdateMoneyUI();
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        currentMoney = startingMoney; // Uses property setter
        InvokeRepeating(nameof(GeneratePassiveIncome), passiveIncomeInterval, passiveIncomeInterval);
    }

    void UpdateMoneyUI()
    {
        moneyText.text = $"${currentMoney.ToString(moneyFormat)}";
    }
    public float GetIncomeForCreature(CreatureNeeds creature)
    {
        float multiplier = creature.personality switch {
            PersonalityType.Glutton => 0.8f,
            PersonalityType.Lazy => 1.2f,
            PersonalityType.Playful => 1.1f,
            _ => 1f
        };
        return incomePerCreature * multiplier;
    }

    public void AddMoney(float amount)
    {
        if (amount < 0) {
            Debug.LogWarning($"Tried to add negative money: {amount}");
            return;
        }
        currentMoney += amount;
    }

    public bool TrySpendMoney(float amount)
    {
        if (amount < 0) {
            Debug.LogWarning($"Tried to spend negative money: {amount}");
            return false;
        }

        if (currentMoney >= amount) {
            currentMoney -= amount;
            return true;
        }
        return false;
    }
    public bool TryBuyCreature()
    {
        if (!CanAffordCreature()) return false;
        currentMoney -= creatureCost;
        return true;
    }

    void GeneratePassiveIncome()
    {
        float total = 0;
        foreach(var creature in FindObjectsOfType<CreatureNeeds>())
        {
            total += GetIncomeForCreature(creature);
        }
        AddMoney(total);
    }
}

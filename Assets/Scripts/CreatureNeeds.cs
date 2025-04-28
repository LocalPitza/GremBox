using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureNeeds : MonoBehaviour
{

    [Header("Needs Settings")]
    public float hungerDecayRate = 0.1f;
    public float happinessDecayRate = 0.05f;
    public float maxHunger = 100f;
    public float maxHappiness = 100f;

    [Header("Current Stats")]
    [Range(0, 100)] public float hunger;
    [Range(0, 100)] public float happiness;

    [Header("Status Sprites")]
    public GameObject hungerSprite;
    public GameObject playSprite;
    public float hoverHeight = 0.2f;
    public float hoverSpeed = 2f;

    [Header("Feedback")]
    public ParticleSystem feedParticles;
    public ParticleSystem playParticles;
    public float statusThreshold = 30f;

    [Header("Personality")]
    public PersonalityType personality;
    public SpriteRenderer personalityIcon; // For visual personality indicator

    private Vector3 hungerSpriteBasePos;
    private Vector3 playSpriteBasePos;
    private float baseHungerDecay = 1f;
    private float baseHappinessDecay = 0.5f;

    void Start()
    {
        baseHungerDecay = hungerDecayRate;
        baseHappinessDecay = happinessDecayRate;
        
        hunger = maxHunger;
        happiness = maxHappiness;
        
        if (hungerSprite != null) hungerSpriteBasePos = hungerSprite.transform.localPosition;
        if (playSprite != null) playSpriteBasePos = playSprite.transform.localPosition;
        
        SetStatusSprites(false, false);
        ApplyPersonality();
    }

    void Update()
    {
        UpdateNeeds();
        UpdateStatusSprites();
    }

    void UpdateNeeds()
    {
        hunger = Mathf.Max(0, hunger - hungerDecayRate * Time.deltaTime);
        happiness = Mathf.Max(0, happiness - happinessDecayRate * Time.deltaTime);
    }

    void UpdateStatusSprites()
    {
        bool showHunger = hunger <= statusThreshold;
        bool showPlay = happiness <= statusThreshold && !showHunger;
        
        SetStatusSprites(showHunger, showPlay);
        
        if (showHunger || showPlay) 
        {
            float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            if (showHunger) hungerSprite.transform.localPosition = hungerSpriteBasePos + Vector3.up * hoverOffset;
            if (showPlay) playSprite.transform.localPosition = playSpriteBasePos + Vector3.up * hoverOffset;
        }
    }

    void SetStatusSprites(bool hungerActive, bool playActive)
    {
        if (hungerSprite != null) hungerSprite.SetActive(hungerActive);
        if (playSprite != null) playSprite.SetActive(playActive);
    }

    public void Feed(float amount)
    {
        float effectiveness = personality == PersonalityType.Glutton ? 1.5f : 1f;
        hunger = Mathf.Min(maxHunger, hunger + amount * effectiveness);
        if (feedParticles != null) feedParticles.Play();
    }

    public void Pet(float amount)
    {
        float effectiveness = personality == PersonalityType.Playful ? 1.5f : 
                           (personality == PersonalityType.Shy ? 0.7f : 1f);
        happiness = Mathf.Min(maxHappiness, happiness + amount * effectiveness);
        if (playParticles != null) playParticles.Play();
    }

    public void ApplyPersonality()
    {
        // Reset to base values first
        hungerDecayRate = baseHungerDecay;
        happinessDecayRate = baseHappinessDecay;

        switch(personality)
        {
            case PersonalityType.Playful:
                happinessDecayRate *= 1.5f;
                if (personalityIcon != null) personalityIcon.color = Color.cyan;
                break;
                
            case PersonalityType.Glutton:
                hungerDecayRate *= 2f;
                if (personalityIcon != null) personalityIcon.color = Color.red;
                break;
                
            case PersonalityType.Lazy:
                happinessDecayRate *= 0.8f;
                if (personalityIcon != null) personalityIcon.color = Color.gray;
                GetComponent<CreatureMovement>().moveSpeed *= 0.7f;
                break;
                
            case PersonalityType.Curious:
                if (personalityIcon != null) personalityIcon.color = Color.yellow;
                GetComponent<CreatureMovement>().reactionChance = 80f;
                break;
                
            case PersonalityType.Shy:
                hungerDecayRate *= 0.8f;
                if (personalityIcon != null) personalityIcon.color = new Color(1, 0.5f, 0.8f);
                GetComponent<CreatureMovement>().reactionChance = 10f;
                break;
        }
    }

    [ContextMenu("Randomize Personality")]
    public void RandomizePersonality()
    {
        personality = (PersonalityType)Random.Range(0, System.Enum.GetValues(typeof(PersonalityType)).Length);
        ApplyPersonality();
    }
}

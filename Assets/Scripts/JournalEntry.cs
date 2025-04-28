using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JournalEntry
{
    public string creatureName;
    public PersonalityType personality;
    public float incomeValue;
    public bool isFavorite;
    public GameObject linkedCreature;
    public Sprite creatureIcon;
}

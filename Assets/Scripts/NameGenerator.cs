using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NameGenerator : MonoBehaviour
{
    [Header("Name Lists")]
    public List<string> firstNameList = new List<string>() { "Fluffy", "Sparky", "Mochi" };
    public List<string> lastNameList = new List<string>() { "the Curious", "of Doom", "Snuggles" };

    [Header("Personality-based Names")]
    public List<string> playfulNames = new List<string>() { "Bouncer", "Jester", "Tumbler" };
    public List<string> lazyNames = new List<string>() { "Snorer", "Slouch", "Dozy" };
    public List<string> gluttonNames = new List<string>() { "Muncher", "Gobbler", "Chompy" };
    public List<string> curiousNames = new List<string>() { "Explorer", "Snooper", "Scout" };
    public List<string> shyNames = new List<string>() { "Whisper", "Shadow", "Blush" };

    [Header("Settings")]
    public bool usePersonalityNames = true;
    [Range(0, 1)] public float chanceToUseLastName = 0.7f;
    public string separator = " ";

    public string GenerateName(PersonalityType personality)
    {
        string firstName;
        string lastName = "";

        // Get first name based on personality if enabled
        if (usePersonalityNames)
        {
            firstName = personality switch
            {
                PersonalityType.Playful => GetRandom(playfulNames),
                PersonalityType.Lazy => GetRandom(lazyNames),
                PersonalityType.Glutton => GetRandom(gluttonNames),
                PersonalityType.Curious => GetRandom(curiousNames),
                PersonalityType.Shy => GetRandom(shyNames),
                _ => GetRandom(firstNameList)
            };
        }
        else
        {
            firstName = GetRandom(firstNameList);
        }

        // Optionally add last name
        if (Random.value < chanceToUseLastName && lastNameList.Count > 0)
        {
            lastName = separator + GetRandom(lastNameList);
        }

        return firstName + lastName;
    }

    private string GetRandom(List<string> list)
    {
        return list.Count > 0 ? list[Random.Range(0, list.Count)] : "Unnamed";
    }
}

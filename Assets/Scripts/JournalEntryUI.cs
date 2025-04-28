using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalEntryUI : MonoBehaviour
{
    [Header("References")]
    public TMP_Text nameText;
    public TMP_Text personalityText;
    public TMP_Text incomeText;
    public Image creatureImage;
    public Button sellButton;
    public Button favoriteButton;
    public Image favoriteIcon;

    [Header("Rename Settings")]
    public TMP_InputField nameInput;
    public Button renameButton;
    public float renameTransitionSpeed = 5f;

    private JournalEntry entry;
    private CreatureJournal journal;
    private bool isRenaming = false;

    void Awake()
    {
        // Ensure proper layout
        GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var vlg = GetComponent<VerticalLayoutGroup>();
        nameInput.gameObject.SetActive(false);
        renameButton.onClick.AddListener(StartRenaming);
        nameInput.onEndEdit.AddListener(FinishRenaming);
    }
    void Update()
    {
        // Handle ESC key to cancel renaming
        if (isRenaming && Input.GetKeyDown(KeyCode.Escape))
        {
            nameInput.text = entry.creatureName;
            FinishRenaming(entry.creatureName);
        }
    }

    public void Initialize(JournalEntry entry, CreatureJournal journal)
    {
        this.entry = entry;
        this.journal = journal;

        nameText.text = entry.creatureName;
        personalityText.text = entry.personality.ToString();
        incomeText.text = $"${entry.incomeValue:F1}/5s";
        creatureImage.sprite = entry.creatureIcon;
        favoriteIcon.gameObject.SetActive(entry.isFavorite);

        sellButton.onClick.AddListener(() => journal.SellCreature(entry));
        favoriteButton.onClick.AddListener(ToggleFavorite);
        isRenaming = false;
        nameInput.gameObject.SetActive(false);
    }
    void StartRenaming()
    {
        if (isRenaming) return;
        
        isRenaming = true;
        nameInput.text = entry.creatureName;
        nameInput.gameObject.SetActive(true);
        nameInput.Select();
        nameInput.ActivateInputField();
    }

    void FinishRenaming(string newName)
    {
        if (!isRenaming) return;
        
        entry.creatureName = string.IsNullOrWhiteSpace(newName) ? entry.creatureName : newName;
        nameText.text = entry.creatureName;
        nameInput.gameObject.SetActive(false);
        isRenaming = false;
    }

    void ToggleFavorite()
    {
        entry.isFavorite = !entry.isFavorite;
        favoriteIcon.gameObject.SetActive(entry.isFavorite);
        sellButton.interactable = !entry.isFavorite;
    }
}

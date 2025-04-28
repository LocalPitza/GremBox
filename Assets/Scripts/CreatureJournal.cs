using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureJournal : MonoBehaviour
{
    public static CreatureJournal Instance { get; private set; }
    
    [Header("References")]
    public NameGenerator nameGenerator;
    public GameObject journalUI;
    public Transform entryContainer;
    public JournalEntryUI entryPrefab;
    
    [Header("Grid Settings")]
    private GridLayoutGroup gridLayout;
    public int maxColumns = 3;
    public float cellSpacing = 20f;
    public Vector2 cellSize = new Vector2(300, 150);
    public ScrollRect scrollRect;
    
    [SerializeField] private List<JournalEntry> entries = new List<JournalEntry>();

    void Awake()
    {
        Instance = this;
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Safely get or add GridLayoutGroup
        gridLayout = entryContainer.GetComponent<GridLayoutGroup>();
        
        // Set scroll content with proper RectTransform
        if (scrollRect != null && entryContainer != null)
        {
            scrollRect.content = entryContainer.GetComponent<RectTransform>();
        }

        if (gridLayout == null)
        {
            gridLayout = entryContainer.gameObject.AddComponent<GridLayoutGroup>();
        }

        // Configure grid
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = maxColumns;
        gridLayout.cellSize = cellSize;
        gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);

        // Ensure ContentSizeFitter exists
        var sizeFitter = entryContainer.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = entryContainer.gameObject.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void UpdateJournalUI()
    {
        if (this == null || entryContainer == null) return;
        
        StartCoroutine(RefreshJournalGrid());
    }

    IEnumerator RefreshJournalGrid()
    {
        // Safely check for destroyed objects
        if (entryContainer == null) yield break;

        // Clean up null entries
        entries.RemoveAll(entry => entry.linkedCreature == null);

        // Clear existing UI safely
        foreach(Transform child in entryContainer)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
        
        yield return null;

        // Rebuild grid only if components exist
        if (gridLayout == null || entryContainer == null) yield break;

        // Calculate responsive cell size
        RectTransform containerRect = entryContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float cellWidth = (containerWidth - (cellSpacing * (maxColumns-1))) / maxColumns;
        gridLayout.cellSize = new Vector2(cellWidth, cellSize.y);

        // Create new entries
        foreach(var entry in entries)
        {
            if (entry.linkedCreature == null) continue;
            
            var entryUI = Instantiate(entryPrefab, entryContainer);
            if (entryUI != null)
                entryUI.Initialize(entry, this);
            
            yield return null;
        }
                    
        LayoutRebuilder.ForceRebuildLayoutImmediate(entryContainer.GetComponent<RectTransform>());

        // Snap scroll to top after rebuild
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1); // 1 = top
        }
        yield return null;
    }

    public void SellCreature(JournalEntry entry)
    {
        if(entry.isFavorite || entry.linkedCreature == null) return;
        
        EconomyManager.Instance.AddMoney(entry.incomeValue * 5);
        Destroy(entry.linkedCreature);
        entries.Remove(entry);
        UpdateJournalUI();
    }

    [ContextMenu("Debug Check Journal")]
    public void DebugCheckJournal()
    {
        Debug.Log($"Journal contains {entries.Count} entries");
        foreach (var entry in entries)
        {
            Debug.Log($"{entry.creatureName} - {(entry.linkedCreature != null ? "VALID" : "MISSING")}");
        }
    }

    public void RegisterCreature(GameObject creature)
    {
        StartCoroutine(RegisterCreatureWithDelay(creature, 0.1f));
    }

    private IEnumerator RegisterCreatureWithDelay(GameObject creature, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (creature == null) yield break;

        var needs = creature.GetComponent<CreatureNeeds>();
        var renderer = creature.GetComponent<SpriteRenderer>();

        if (needs == null || renderer == null)
        {
            Debug.LogError($"Creature missing required components: {creature.name}");
            yield break;
        }

        var newEntry = new JournalEntry 
        {
            creatureName = nameGenerator.GenerateName(needs.personality),
            personality = needs.personality,
            linkedCreature = creature,
            creatureIcon = renderer.sprite,
            isFavorite = false
        };

        entries.Add(newEntry);
        UpdateJournalUI();
    }
}

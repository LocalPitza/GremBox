using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
public class DecorationShop : MonoBehaviour
{
    [System.Serializable]
    public class DecorationItem
    {
        public string name;
        public int cost;
        public GameObject decorationObject;
        public Sprite icon;
        [TextArea] public string description;
        public bool isPurchased = false;
        public bool isEnabled = false;
    }

    [System.Serializable]
    public class DecorationCategory
    {
        public string categoryName;
        public List<DecorationItem> decorations;
    }

    [Header("UI References")]
    public TMP_Text decorationNameText;
    public TMP_Text decorationCostText;
    public TMP_Text decorationDescriptionText;
    public Image decorationIconImage;
    public Button buyButton;
    public Button enableButton;
    public Button disableButton;
    public TMP_Text statusText;
    public Transform categoryButtonsParent;
    public Transform contentParent;
    public GameObject categoryButtonPrefab;
    public GameObject decorationItemUIPrefab;

    [Header("Decoration Settings")]
    public List<DecorationCategory> decorationCategories;

    private int currentCategoryIndex = 0;
    private int currentDecorationIndex = 0;
    private List<GameObject> categoryButtons = new List<GameObject>();
    private List<GameObject> decorationItemUIElements = new List<GameObject>();
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = contentParent.parent.GetComponent<ScrollRect>();
        InitializeCategories();
        ShowCategory(0);
    }

    private void InitializeCategories()
    {
        // Clear existing buttons
        foreach (Transform child in categoryButtonsParent)
        {
            Destroy(child.gameObject);
        }
        categoryButtons.Clear();

        // Create category buttons
        for (int i = 0; i < decorationCategories.Count; i++)
        {
            int index = i; // Local copy for closure
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryButtonsParent);
            Button button = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            buttonText.text = decorationCategories[i].categoryName;

            button.onClick.AddListener(() => ShowCategory(index));
            categoryButtons.Add(buttonObj);
        }
    }

    private void ShowCategory(int categoryIndex)
    {
        currentCategoryIndex = categoryIndex;
        currentDecorationIndex = 0;

        // Clear existing decoration UI elements
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        decorationItemUIElements.Clear();

        // Create decoration UI elements for this category
        for (int i = 0; i < decorationCategories[categoryIndex].decorations.Count; i++)
        {
            int index = i; // Local copy for closure
            DecorationItem item = decorationCategories[categoryIndex].decorations[i];
            GameObject itemUI = Instantiate(decorationItemUIPrefab, contentParent);
            
            // Set UI elements
            Image icon = itemUI.transform.Find("Icon").GetComponent<Image>();
            TMP_Text nameText = itemUI.transform.Find("Name").GetComponent<TMP_Text>();
            TMP_Text costText = itemUI.transform.Find("Cost").GetComponent<TMP_Text>();
            Button selectButton = itemUI.GetComponent<Button>();

            icon.sprite = item.icon;
            nameText.text = item.name;
            costText.text = item.isPurchased ? "Owned" : $"${item.cost}";

            selectButton.onClick.AddListener(() => SelectDecoration(index));
            decorationItemUIElements.Add(itemUI);
        }

        // Select first decoration by default
        if (decorationCategories[categoryIndex].decorations.Count > 0)
        {
            SelectDecoration(0);
        }
    }

    private void SelectDecoration(int decorationIndex)
    {
        currentDecorationIndex = decorationIndex;
        DecorationItem selectedItem = decorationCategories[currentCategoryIndex].decorations[decorationIndex];

        // Update main display
        decorationNameText.text = selectedItem.name;
        decorationCostText.text = selectedItem.isPurchased ? "Owned" : $"Cost: ${selectedItem.cost}";
        decorationDescriptionText.text = selectedItem.description;
        decorationIconImage.sprite = selectedItem.icon;

        // Update button states
        buyButton.gameObject.SetActive(!selectedItem.isPurchased);
        enableButton.gameObject.SetActive(selectedItem.isPurchased && !selectedItem.isEnabled);
        disableButton.gameObject.SetActive(selectedItem.isPurchased && selectedItem.isEnabled);

        // Update status text
        if (!selectedItem.isPurchased)
        {
            statusText.text = "Not Purchased";
        }
        else
        {
            statusText.text = selectedItem.isEnabled ? "Enabled" : "Disabled";
        }

        // Scroll to selected item
        if (scrollRect != null && decorationItemUIElements.Count > decorationIndex)
        {
            Canvas.ForceUpdateCanvases();
            RectTransform contentRect = contentParent.GetComponent<RectTransform>();
            RectTransform targetRect = decorationItemUIElements[decorationIndex].GetComponent<RectTransform>();
            
            contentRect.anchoredPosition = new Vector2(
                contentRect.anchoredPosition.x,
                -targetRect.localPosition.y - (targetRect.rect.height / 2)
            );
        }
    }

    public void BuyCurrentDecoration()
    {
        DecorationItem item = decorationCategories[currentCategoryIndex].decorations[currentDecorationIndex];
        
        // Check if player has enough currency (implement your own currency system)
        if (HasEnoughCurrency(item.cost))
        {
            // Deduct currency
            DeductCurrency(item.cost);
            
            // Update item status
            item.isPurchased = true;
            item.isEnabled = true;
            
            // Activate decoration
            if (item.decorationObject != null)
            {
                item.decorationObject.SetActive(true);
            }
            
            // Update UI
            SelectDecoration(currentDecorationIndex);
            
            // Update the list item UI
            TMP_Text costText = decorationItemUIElements[currentDecorationIndex].transform.Find("Cost").GetComponent<TMP_Text>();
            costText.text = "Owned";
        }
        else
        {
            // Show not enough money message
            statusText.text = "Not enough money!";
        }
    }

    public void EnableCurrentDecoration()
    {
        DecorationItem item = decorationCategories[currentCategoryIndex].decorations[currentDecorationIndex];
        item.isEnabled = true;
        
        if (item.decorationObject != null)
        {
            item.decorationObject.SetActive(true);
        }
        
        SelectDecoration(currentDecorationIndex);
    }

    public void DisableCurrentDecoration()
    {
        DecorationItem item = decorationCategories[currentCategoryIndex].decorations[currentDecorationIndex];
        item.isEnabled = false;
        
        if (item.decorationObject != null)
        {
            item.decorationObject.SetActive(false);
        }
        
        SelectDecoration(currentDecorationIndex);
    }

    // Implement your own currency system
    private bool HasEnoughCurrency(int amount)
    {
        // Replace with your actual currency check
        return true;
    }

    private void DeductCurrency(int amount)
    {
        // Replace with your actual currency deduction
    }
}

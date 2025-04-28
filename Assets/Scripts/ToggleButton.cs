using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [System.Serializable]
    public class ToggleGroup
    {
        public Button toggleButton;
        public List<GameObject> targetObjects;
        public KeyCode keyboardShortcut = KeyCode.None;
    }

    public List<ToggleGroup> toggleGroups;

    void Start()
    {
        foreach (var group in toggleGroups)
        {
            if (group.toggleButton != null)
            {
                group.toggleButton.onClick.AddListener(() => ToggleObjects(group));
            }
        }
    }

    void Update()
    {
        // Check for ESC first
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllWindows();
            return;
        }

        // Check other keyboard shortcuts
        foreach (var group in toggleGroups)
        {
            if (group.keyboardShortcut != KeyCode.None && 
                Input.GetKeyDown(group.keyboardShortcut))
            {
                ToggleObjects(group);
            }
        }
    }

    void CloseAllWindows()
    {
        foreach (var group in toggleGroups)
        {
            // Only turn off objects that are currently active
            foreach (var obj in group.targetObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    void ToggleObjects(ToggleGroup group)
    {
        bool anyActive = AreAnyObjectsActive(group);
        
        foreach (var obj in group.targetObjects)
        {
            if (obj != null)
            {
                obj.SetActive(!anyActive);
            }
        }
    }

    bool AreAnyObjectsActive(ToggleGroup group)
    {
        foreach (var obj in group.targetObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}

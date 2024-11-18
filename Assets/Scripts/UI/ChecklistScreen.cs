using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class ChecklistScreen : MonoBehaviour
{
    //public bool PC = false;
    //public bool PAC = false;
    //public bool TK = false;
    //public bool FCK = false;
    //public bool PKC = false;
    //public bool GE = false;
    //public bool GP = false;
    //public bool SCK = false;
    //public bool DC = false;

    public GameObject checklistItemPrefab;
    public List<string> checklistNames = new List<string>(); // internal names
    public List<string> checklistTexts = new List<string>(); // external text

    // Dictionary to map task names to their indices in the checklistItems list
    public Dictionary<string, bool> taskNameToBool = new Dictionary<string, bool>();
    private Dictionary<string, GameObject> taskNameToGameObject= new Dictionary<string, GameObject>();

    public UnityEvent onChecklistComplete;

    void Start()
    {
        CreateChecklistItems();
    }

    public void CreateChecklistItems()
    {
        for (int i = 0; i < checklistNames.Count; i++)
        {
            GameObject item = Instantiate(checklistItemPrefab, this.transform);
            item.name = "ChecklistItem" + i;

            // Set the checklist text
            item.GetComponentInChildren<TextMeshProUGUI>().text = checklistTexts[i];

            // Add the item to the checklistItems list
            taskNameToGameObject[checklistNames[i]] = item;

            // Map the task name to its index in taskNameToIndex dictionary
            taskNameToBool[checklistNames[i]] = false;
        }
    }

    public void UpdateChecklistItem(string taskName, bool isComplete)
    {
        // Check if the task name exists in the dictionary
        if (!taskNameToBool.TryGetValue(taskName, out bool status))
        {
            Debug.LogWarning("Invalid task name.");
            return;
        }

        taskNameToBool[taskName] = isComplete;

        // Update the color based on completion status
        GameObject item = taskNameToGameObject[taskName];
        Color color = isComplete ? Color.green : Color.red;
        item.GetComponent<Image>().color = color;

        CheckComplete();
    }

    void CheckComplete()
    {
        foreach (var task in taskNameToBool)
        {
            if (!task.Value)
                return;
        }

        onChecklistComplete.Invoke();
    }
}

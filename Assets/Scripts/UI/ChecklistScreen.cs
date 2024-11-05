using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChecklistScreen : MonoBehaviour
{
    public GameObject checklistItemPrefab;
    public List<string> checklistTexts = new List<string>();
    private List<GameObject> checklistItems = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateChecklistItems(checklistTexts.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateChecklistItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(checklistItemPrefab, this.transform);
            item.name = "ChecklistItem" + i;
            if (i < checklistTexts.Count)
                item.GetComponentInChildren<Text>().text = checklistTexts[i];
            checklistItems.Add(item);
        }
    }

    public void UpdateChecklistItem(int taskNumber, bool isComplete)
    {
        if (taskNumber < 0 || taskNumber >= checklistItems.Count)
        {
            Debug.LogWarning("Invalid task number.");
            return;
        }

        // Change color based on completion status
        GameObject item = checklistItems[taskNumber];
        Color color = isComplete ? Color.green : Color.red;
        item.GetComponent<Image>().color = color; // Assumes checklist item has an Image component
    }
}

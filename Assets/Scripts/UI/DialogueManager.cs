using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;               // dialogue text to display
    public GameObject dialoguePanel;                   // dialogue panel UI
    public List<DialogueStep> dialogueSteps;           // full list of DialogueStep assets
    public Button nextButton;
    public Button backButton;

    private int currentStepIndex = 0;

    private void Start()
    {
        dialoguePanel.SetActive(false);   
        nextButton.onClick.AddListener(NextLine);
        backButton.onClick.AddListener(PreviousLine);
    }

    public void StartDialogue()
    {
        currentStepIndex = 0;
        dialoguePanel.SetActive(true);
        UpdateDialogue();
    }

    private void UpdateDialogue()
    {
        if (currentStepIndex < dialogueSteps.Count)
        {
            var step = dialogueSteps[currentStepIndex];
            dialogueText.text = step.dialogueText;
            ShowHints(step.hints);

            // Show or hide navigation buttons as needed
            backButton.gameObject.SetActive(currentStepIndex > 0);
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                currentStepIndex == dialogueSteps.Count - 1 ? "Done" : "Next";
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowHints(GameObject[] hints)
    {
        // disable all hints to clear previous ones
        foreach (DialogueStep step in dialogueSteps)
        {
            foreach (var hint in step.hints)
            {
                if (hint != null)
                    hint.SetActive(false);
            }
        }

        // only enable the hints for the current step
        foreach (var hint in hints)
        {
            if (hint != null)
                hint.SetActive(true);
        }
    }

    public void NextLine()
    {
        if (currentStepIndex < dialogueSteps.Count - 1)
        {
            currentStepIndex++;
            UpdateDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void PreviousLine()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateDialogue();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        ShowHints(new GameObject[0]); // hide all the hints when we're done
    }
}

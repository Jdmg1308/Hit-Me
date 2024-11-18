using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopGuideManager : MonoBehaviour
{
    public TextMeshProUGUI instructionText;          // text for instructions
    public GameObject guidePanel;                    // panel for instruction text
    public List<ShopGuideStep> guideSteps;           // list of guide steps
    public Button nextButton;                        // button to move on to next step
    private int currentStepIndex = 0;

    private bool hoverCompleted = false;
    private bool purchaseCompleted = false;
    private bool destroyCompleted = false;

    private void Start()
    {
        guidePanel.SetActive(false); // hide guide panel initially
        nextButton.gameObject.SetActive(false); // hide 'next' button initially
        nextButton.onClick.AddListener(NextStep);
    }

    public void StartGuide()
    {
        currentStepIndex = 0;
        guidePanel.SetActive(true);
        UpdateGuideStep();
    }

    private void UpdateGuideStep()
    {
        if (currentStepIndex < guideSteps.Count)
        {
            var step = guideSteps[currentStepIndex];
            instructionText.text = step.instructionText;
            ShowHints(step.hints);

            // hide the Next button if this step requires an action, until the action is completed
            nextButton.gameObject.SetActive(!step.requiresHover && !step.requiresPurchase && !step.requiresDestroy);
        }
        else
        {
            EndGuide();
        }
    }

    // show all of the hints related to the step
    private void ShowHints(GameObject[] hints)
    {
        foreach (var hint in hints)
        {
            if (hint != null)
                hint.SetActive(true);
        }
    }

    // hide all hints
    private void HideAllHints()
    {
        foreach (var step in guideSteps)
        {
            foreach (var hint in step.hints)
            {
                if (hint != null)
                    hint.SetActive(false);
            }
        }
    }

    public void NextStep()
    {
        currentStepIndex++;
        hoverCompleted = purchaseCompleted = destroyCompleted = false; // reset required action flags
        HideAllHints();  // clear previous step hints
        UpdateGuideStep();
    }

    // to be called when required object is hovered over
    public void OnHover()
    {
        if (guideSteps[currentStepIndex].requiresHover && !hoverCompleted)
        {
            hoverCompleted = true;
            ShowNextButtonIfNeeded();
        }
    }

    // to be called when required object is purchased
    public void OnPurchase()
    {
        if (guideSteps[currentStepIndex].requiresPurchase && !purchaseCompleted)
        {
            purchaseCompleted = true;
            ShowNextButtonIfNeeded();
        }
    }

    // to be called when required object is destroyed
    public void OnDestroy()
    {
        if (guideSteps[currentStepIndex].requiresDestroy && !destroyCompleted)
        {
            destroyCompleted = true;
            ShowNextButtonIfNeeded();
        }
    }

    private void ShowNextButtonIfNeeded()
    {
        // show the 'next' button if all required actions for the current step are completed
        if (hoverCompleted || purchaseCompleted || destroyCompleted)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    private void EndGuide()
    {
        guidePanel.SetActive(false);
        HideAllHints();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckSelection : MonoBehaviour
{
    public List<Card> deck1cards;
    public List<Card> deck2cards;
    public List<Card> deck3cards;
    private GameManager GM;
    private GameObject DeckSelectionScreen;
    private Button ContinueButton;
    private GameObject deckPanel;
    private List<Card> current;

    private void Awake()
    {
        AssignReferences();

        ContinueButton = DeckSelectionScreen.transform.Find("Continue")?.GetComponent<Button>();
        deckPanel = DeckSelectionScreen.transform.Find("DeckDisplayPanel")?.transform.gameObject;
        if (ContinueButton) ContinueButton.onClick.AddListener(ChooseAndContinue);


        SetUpChoices(1);
        SetUpButtons();
    }


    private void AssignReferences()
    {
        GM = GameManager.instance;

        DeckSelectionScreen = this.gameObject;

        if (DeckSelectionScreen)
        {
            //GM.AssignButton(MapScreen.transform, "Menu", GM.OpenMenu);
            //GM.AssignButton(MapScreen.transform, "Tutorial", PlayTutorial);
            //GM.AssignButton(MapScreen.transform, "Shop", GM.OpenShop);
            //GM.AssignButton(MapScreen.transform, "Level 1", PlayLvl1);
            //GM.AssignButton(MapScreen.transform, "Level 2", PlayLvl2);
            //GM.AssignButton(MapScreen.transform, "Level 3", PlayLvl3);
            //GM.AssignButton(MapScreen.transform, "Final Level", FinalLvl);
        }

    }


    /*
     * Sets up the cards in the shop to be random & configures the button sprites & listeners
     * to be called whenever a level is beaten.
     */
    public void SetUpButtons()
    {
        Button deck1 = DeckSelectionScreen.transform.Find("Deck1")?.gameObject.GetComponent<Button>();
        deck1.onClick.AddListener(() => SetUpChoices(1));

        Button deck2 = DeckSelectionScreen.transform.Find("Deck2")?.gameObject.GetComponent<Button>();
        deck2.onClick.AddListener(() => SetUpChoices(2));

        Button deck3 = DeckSelectionScreen.transform.Find("Deck3")?.gameObject.GetComponent<Button>();
        deck3.onClick.AddListener(() => SetUpChoices(3));
    }

    public void SetUpChoices(int deckNumber)
    {
        List<Card> deck;
        switch (deckNumber)
        {
            case 1:
                deck = deck1cards;
                break;
            case 2:
                deck = deck2cards;
                break;
            case 3:
                deck = deck3cards;
                break;
            default:
                deck = deck1cards;
                break;
        }

        current = deck;
        // Clear previous buttons
        foreach (Transform child in deckPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in deck)
        {
            GM.showCard(card, deckPanel);
        }
    }

    public void ChooseAndContinue()
    {
        // Choose
        Deck newDeck = new Deck();
        foreach (Card card in current)
        {
            GM.deckController.DeckAdd(card, newDeck);
        }
        GM.deckController.currentDeck = newDeck;

        // Continue
        GM.ContinueToRandomLevel();

    }
}

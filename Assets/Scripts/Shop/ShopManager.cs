using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : TheSceneManager
{
    public List<Card> allPurchasableCards;
    public GameObject deleteSign;
    public Button destroyButton;
    public GameObject Options; // Panel containing the player's current deck of cards
    public GameObject deckDisplayPanel; // Panel containing the player's current deck of cards
    private GameObject Canvas;
    private GameObject ShopScreen;
    public Button NextLevelButton; // Prefab to create buttons for each card in the deck
    public float dupPrice;
    public float destPrice;

    private GameManager GM;

    private void Awake()
    {
        // Buttons should be assigned in the Inspector, no need to assign them here unless necessary
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        Canvas = GameObject.FindGameObjectWithTag("Canvas");

        if (Canvas)
            ShopScreen = Canvas.transform.Find("Shop Screen")?.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpShop();
        destroyButton.onClick.AddListener(DisplayDeckForDestruction);
        NextLevelSetup();

    }

    /*
     * Sets up the cards in the shop to be random & configures the button sprites & listeners
     * to be called whenever a level is beaten.
     */
    public void SetUpShop()
    {
        int[] _randCards = new int[3];
        for (int i = 0; i < 3; i++)
        {
            _randCards[i] = Random.Range(0, allPurchasableCards.Count);
        }

        GameObject displayPanel = Options.transform.Find("ShopDisplayPanel").gameObject;
        // Using _randCards[i] instead of 0, 1, 2
        GameObject child0 = GM.showCard(allPurchasableCards[_randCards[0]], displayPanel);
        child0.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[0], child0));

        //Debug.Log("Gets HERE: " + child0.name);

        GameObject child1 = GM.showCard(allPurchasableCards[_randCards[1]], displayPanel);
        child1.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[1], child1));

        GameObject child2 = GM.showCard(allPurchasableCards[_randCards[2]], displayPanel);
        child2.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[2], child2));

        //card1Button.GetComponent<Image>().sprite = allPurchasableCards[_randCards[1]].cardImage;
        //card1Button.onClick.AddListener(() => PurchaseCard(_randCards[1], card1Button));

        //card2Button.GetComponent<Image>().sprite = allPurchasableCards[_randCards[2]].cardImage;
        //card2Button.onClick.AddListener(() => PurchaseCard(_randCards[2], card2Button));
    }

    private void PurchaseCard(int _cardIndex, GameObject buttonCard)
    {
        Debug.Log("Gets HERE: " + GM.money);
        Card selectedCard = allPurchasableCards[_cardIndex];
        if (GM.money < selectedCard.price)
        {
            // Feedback for insufficient funds (e.g., play a sound or display a message)
            Debug.Log("Player doesn't have enough money to buy this card!, MONEY: " + GM.money);
            return;
        }
        else
        {
            // Player buys the card
            GM.money -= selectedCard.price;
            GM.deckController.DeckAdd(selectedCard, GM.deckController.currentDeck);
            Destroy(buttonCard);
            GM.updateDeckPanel();
        }
    }

    void DisplayDeckForDestruction()
    {
        deleteSign.SetActive(true);
        deckDisplayPanel.GetComponent<Image>().color = Color.red;
        ShopDisplayDestroyableDeck(false);
    }

    void ShopDisplayDestroyableDeck(bool duplicateCard)
    {
        Options.SetActive(false);


        // Clear previous buttons
        foreach (Transform child in deckDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in GM.deckController.currentDeck)
        {
            GameObject cardButton = GM.showCard(card, deckDisplayPanel);
            cardButton.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                DestroyCard(card);
                Options.SetActive(true);
                deleteSign.SetActive(false);
                deckDisplayPanel.GetComponent<Image>().color = Color.white;
            });
        }
    }

    void DestroyCard(Card card)
    {
        if (GM.money < destPrice)
        {
            Debug.Log("Not enough money to destroy this card!");
            return;
        }
        else
        {
            GM.money -= destPrice;
            GM.deckController.DeckRemove(card, GM.deckController.currentDeck); // Correcting to remove the card
            GM.updateDeckPanel();
        }
    }

    public void NextLevelSetup()
    {
        //if (remainingLevels.Count == 0)
        //{
        //    // All levels have been played; reset the list to start over
        //    remainingLevels = new List<System.Action>(availableLevels);
        //}

        // Choose a random level from the remaining levels
        int randomIndex = Random.Range(0, 4);
        //System.Action nextLevel = remainingLevels[randomIndex];
        //remainingLevels.RemoveAt(randomIndex); // Remove chosen level so it won’t repeat

        //// Assign the chosen level to the button
        //GM.AssignButton(ShopScreen.transform, "NextLevel", nextLevel);

        switch (randomIndex)
        {
            case 0:
                GM.AssignButton(ShopScreen.transform, "NextLevel", PlayAlleyway1);
                break;
            case 1:
                GM.AssignButton(ShopScreen.transform, "NextLevel", PlayAlleyway2);
                break;
            case 2:
                GM.AssignButton(ShopScreen.transform, "NextLevel", PlayLvl2);
                break;
            case 3:
                GM.AssignButton(ShopScreen.transform, "NextLevel", PlayLvl3);
                break;

            default:
                Debug.LogWarning("HELL NAWWWW" + GM.LastScene);
                break;
        }
    }
}

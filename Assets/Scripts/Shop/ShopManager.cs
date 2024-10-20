using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : TheSceneManager
{
    public List<Card> allPurchasableCards;
    public Button duplicateButton;
    public Button destroyButton;
    public GameObject Options; // Panel containing the player's current deck of cards
    public GameObject deckDisplayPanel; // Panel containing the player's current deck of cards
    public Button exitButton; // Prefab to create buttons for each card in the deck
    public float dupPrice;
    public float destPrice;

    private GameManager GM;

    private void Awake()
    {
        // Buttons should be assigned in the Inspector, no need to assign them here unless necessary
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpShop();
        duplicateButton.onClick.AddListener(DisplayDeckForDuplication);
        destroyButton.onClick.AddListener(DisplayDeckForDestruction);
        exitButton.onClick.AddListener(OpenMap);
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
        child0.GetComponent<Button>().onClick.AddListener(() => PurchaseCard(_randCards[0], child0));

        GameObject child1 = GM.showCard(allPurchasableCards[_randCards[1]], displayPanel);
        child1.GetComponent<Button>().onClick.AddListener(() => PurchaseCard(_randCards[1], child1));

        GameObject child2 = GM.showCard(allPurchasableCards[_randCards[2]], displayPanel);
        child2.GetComponent<Button>().onClick.AddListener(() => PurchaseCard(_randCards[2], child2));

        //card1Button.GetComponent<Image>().sprite = allPurchasableCards[_randCards[1]].cardImage;
        //card1Button.onClick.AddListener(() => PurchaseCard(_randCards[1], card1Button));

        //card2Button.GetComponent<Image>().sprite = allPurchasableCards[_randCards[2]].cardImage;
        //card2Button.onClick.AddListener(() => PurchaseCard(_randCards[2], card2Button));
    }

    private void PurchaseCard(int _cardIndex, GameObject buttonCard)
    {
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
        }
    }

    void DisplayDeckForDuplication()
    {
        ShopDisplayDeck(true);
    }

    void DisplayDeckForDestruction()
    {
        ShopDisplayDeck(false);
    }

    void ShopDisplayDeck(bool duplicateCard)
    {
        deckDisplayPanel.SetActive(true);
        Options.SetActive(false);

        // Clear previous buttons
        foreach (Transform child in deckDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in GM.deckController.currentDeck)
        {
            GameObject cardButton = GM.showCard(card, deckDisplayPanel);
            cardButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (duplicateCard)
                {
                    DuplicateCard(card);
                }
                else
                {
                    DestroyCard(card);
                }
                deckDisplayPanel.SetActive(false); // Hide after selection
                Options.SetActive(true);
            });
        }
    }

    void DuplicateCard(Card card)
    {
        if (GM.money < dupPrice)
        {
            Debug.Log("Not enough money to duplicate this card!");
            return;
        }
        else
        {
            GM.money -= dupPrice;
            GM.deckController.DeckAdd(card, GM.deckController.currentDeck);
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
        }
    }
}

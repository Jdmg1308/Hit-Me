using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : TheSceneManager
{
    private List<Card> allPurchasableCards;
    public GameObject deleteSign;
    public Button destroyButton;
    public GameObject Options; // Panel containing the player's current deck of cards
    public GameObject deckDisplayPanel; // Panel containing the player's current deck of cards
    private GameObject Canvas;
    private GameObject ShopScreen;
    private Button NextLevelButton; // Prefab to create buttons for each card in the deck
    public float dupPrice;
    public float destPrice;

    private bool destroyMode = false;

    private GameManager GM;

    AudioManager audioManager;

    [SerializeField]
    private List<string> levelSceneNames;

    private void Awake()
    {
        // Buttons should be assigned in the Inspector, no need to assign them here unless necessary
        GM = GameManager.instance;
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        if (GM)
        {
            allPurchasableCards = GM.deckController.allCards;
        }

        if (Canvas)
        {
            ShopScreen = Canvas.transform.Find("Shop Screen")?.gameObject;
            NextLevelButton = ShopScreen.transform.Find("NextLevel")?.GetComponent<Button>();
            if (NextLevelButton) NextLevelButton.onClick.AddListener(ContinueToRandomLevel);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetUpShop();
        destroyButton.onClick.AddListener(DisplayDeckForDestruction);

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

        GameObject child1 = GM.showCard(allPurchasableCards[_randCards[1]], displayPanel);
        child1.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[1], child1));

        GameObject child2 = GM.showCard(allPurchasableCards[_randCards[2]], displayPanel);
        child2.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[2], child2));
    }

    private void PurchaseCard(int _cardIndex, GameObject buttonCard)
    {
        Debug.Log("Gets HERE: " + GM.Money);
        Card selectedCard = allPurchasableCards[_cardIndex];
        if (GM.Money < selectedCard.price)
        {
            // Feedback for insufficient funds (e.g., play a sound or display a message)
            Debug.Log("Player doesn't have enough money to buy this card!, MONEY: " + GM.Money);
            return;
        }
        else
        {
            // Player buys the card
            audioManager.PlaySFX(audioManager.buyCard);
            GM.Money -= (int) selectedCard.price;
            GM.deckController.DeckAdd(selectedCard, GM.deckController.currentDeck);
            Destroy(buttonCard);
            GM.updateDeckPanel();
        }
    }

    void DisplayDeckForDestruction()
    {
        if (!destroyMode)
        {
            destroyMode = true;
            deleteSign.SetActive(true);
            deckDisplayPanel.GetComponent<Image>().color = Color.red;
            ShopDisplayDestroyableDeck();
        } else
        {
            destroyMode = false;
            Options.SetActive(true);
            deleteSign.SetActive(false);
            deckDisplayPanel.GetComponent<Image>().color = Color.white;
        }
        
    }

    void ShopDisplayDestroyableDeck()
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
        if (GM.Money < destPrice)
        {
            Debug.Log("Not enough money to destroy this card!");
            return;
        }
        else
        {
            audioManager.PlaySFX(audioManager.cardDestroy);
            GM.Money -= (int) destPrice;
            GM.deckController.DeckRemove(card, GM.deckController.currentDeck); // Correcting to remove the card
            GM.updateDeckPanel();
        }
    }

    public void ContinueToRandomLevel()
    {
        if (levelSceneNames == null || levelSceneNames.Count == 0)
        {
            Debug.LogError("No levels specified in the levelSceneNames list!");
            return;
        }

        // Pick a random scene name from the list
        string randomScene = levelSceneNames[Random.Range(0, levelSceneNames.Count)];

        // Load the randomly chosen scene
        GM.LoadNextScene(randomScene);
    }
}

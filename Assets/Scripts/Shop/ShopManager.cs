using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    public TextMeshProUGUI uiText;

    private bool destroyMode = false;

    private GameManager GM;

    AudioManager audioManager;


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
            if (NextLevelButton) NextLevelButton.onClick.AddListener(GM.ContinueToRandomLevel);
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
            StartCoroutine(FadeText("Not enough money to buy this card!", 1f));
            return;
        }
        else
        {

            if (GM.deckController.currentDeck.deck.Count + 1 <= GM.deckController.deckLimit)
            {
                // Player buys the card
                audioManager.PlaySFX(audioManager.buyCard);
                GM.Money -= (int)selectedCard.price;
                GM.deckController.DeckAdd(selectedCard, GM.deckController.currentDeck);
                Destroy(buttonCard);
                GM.updateDeckPanel();
            }
            else
            {
                //shopScript = ShopManager.GetComponent<ShopManager>();
                StartCoroutine(FadeText("Deck limit reached! Max of 8 cards", 1f));
                Debug.Log("DECK LIMIT REACHED, 8 of cards:");

            }
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
            GM.updateDeckPanel();
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
            StartCoroutine(FadeText("Not enough money to destroy this card!", 1f));
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

    private IEnumerator FadeText(string message, float duration)
    {
        uiText.text = message;
        uiText.color = new Color(1, 0, 0, 1); // Fully opaque red
        uiText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        float fadeDuration = 1f; // Duration of the fade-out
        float elapsedTime = 0f;
        Color startColor = uiText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // Interpolate alpha
            uiText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        uiText.gameObject.SetActive(false); // Hide the text after fading
    }

}

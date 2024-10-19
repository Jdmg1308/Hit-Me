using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : TheSceneManager
{
    [Header("General")]
    public GameEnemyManager GameEnemyManager;
    public GameObject Player;
    public GameObject Canvas;
    public GameObject Camera;

    [Header("Art/Audio")]
    public AudioSource audioSource;
    public AudioClip KickAudio;
    public AudioClip MissAudio;
    public AudioClip CardPullAudio;
    public AudioClip GoodPullAudio;
    public AudioClip BadPullAudio;
    public AudioClip DeckShuffle;

    [Header("Cards")]
    public DeckController deckController;
    protected float cardCDTime = 5.0f, cardCDTimer = 0;
    protected bool cardIsOnCD = false;
    public Card statusCard;
    public bool statusApplied = false;
    public GameObject cardButtonPrefab; // Prefab to create buttons for each card in the deck
    protected Image UICard;
    protected Image CardCooldownImg;
    protected StatusEffectManager StatusEffectManager;
    protected bool deckShowing = false;
    [HideInInspector]
    public GameObject CardDisplay;
    protected GameObject deckDisplayPanel;

    [Header("Health")]
    public int healthCurrent;       // Current health of the player
    public int healthMax = 100;     // Maximum health of the player (dynamically changed by cards)
    public int baseHealth = 100;    // base maximum HP of player 
    [HideInInspector]
    public Slider healthBar;       // UI Slider for health bar

    [Header("Money")]
    public float money = 500;

    [Header("Mobile")]
    public bool mobile;
    [HideInInspector]
    public GameObject iOSPanel;

    [HideInInspector]
    public Player p;
    [HideInInspector]
    public PlayerController playerController;

    public TextMeshProUGUI money_text;
    protected TextMeshProUGUI quota_text;

    protected GameObject PlayScreen;
    protected GameObject DifficultyScreen;
    protected GameObject PauseScreen;
    protected GameObject DeathScreen;
    protected GameObject WinScreen;
    protected GameObject IBuild;

    protected bool paused = false;

    [Header("FX")]
    // hit fx
    public bool InHitStop = false;
    public AnimationCurve Curve;
    // hurt fx
    public float totalHurtFlashTime;
    public AnimationCurve hurtFlashCurve;
    private Image hurtFlashImage;


    private static GameManager instance;

    public bool hasWon = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Add listener for scene change
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("GameManager instance is not assigned.");
        }
        return instance;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignReferences(); // Reassign UI references
    }

    public void AssignButton(Transform screen, string buttonName, UnityEngine.Events.UnityAction action)
    {
        Button button = screen.Find(buttonName)?.GetComponent<Button>();
        if (button)
        {
            // Debug.Log($"{buttonName} button found and assigned for {screen.name}");
            button.onClick.AddListener(action);
        }
    }

    void AssignReferences()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        Player = GameObject.FindGameObjectWithTag("Player");
        Camera = GameObject.FindGameObjectWithTag("MainCamera");

        if (Player)
        {
            playerController = Player.GetComponent<PlayerController>();
            resetPlayer();
        }

        if (Canvas)
        {
            PlayScreen = Canvas.transform.Find("Play Screen")?.gameObject;
            DifficultyScreen = Canvas.transform.Find("Difficulty Screen")?.gameObject;
            PauseScreen = Canvas.transform.Find("Pause Screen")?.gameObject;
            DeathScreen = Canvas.transform.Find("Death Screen")?.gameObject;
            WinScreen = Canvas.transform.Find("Win Screen")?.gameObject;
            IBuild = Canvas.transform.Find("InclusivityBuildPanel")?.gameObject;
        }

        if (PlayScreen)
        {
            Debug.Log("PLAY SCREEN" + PlayScreen);
            money_text = PlayScreen.transform.Find("Money")?.gameObject.GetComponent<TextMeshProUGUI>();
            CardDisplay = PlayScreen.transform.Find("CardDisplay")?.gameObject;
            if (CardDisplay)
            {
                UICard = CardDisplay.GetComponent<Image>();
                CardCooldownImg = CardDisplay.transform.Find("Cooldown")?.gameObject.GetComponent<Image>();
                StatusEffectManager = CardDisplay.GetComponent<StatusEffectManager>();
            }
            healthBar = PlayScreen.GetComponentInChildren<Slider>();
            hurtFlashImage = PlayScreen.transform.Find("HurtFlash")?.gameObject.GetComponent<Image>();

            money_text.text = " " + money.ToString();
        }

        if (DeathScreen || WinScreen || PauseScreen)
        {
            if (DeathScreen)
            {
                AssignButton(DeathScreen.transform, "Map", OpenMap);
                AssignButton(DeathScreen.transform, "Restart", Restart);
            }

            if (WinScreen)
            {
                AssignButton(WinScreen.transform, "Map", OpenMap);
            }

            if (PauseScreen)
            {
                AssignButton(PauseScreen.transform, "Map", OpenMap);
                AssignButton(PauseScreen.transform, "Resume", Pause);

                deckDisplayPanel = PauseScreen.transform.Find("DeckDisplayPanel")?.gameObject;
                AssignButton(PauseScreen.transform, "DeckButton", currentCardDeck);
            }
        }

        if (DifficultyScreen) {
            DifficultyScreen.transform.Find("Easy").GetComponent<Button>().onClick.AddListener(() => DifficultyChoice(1));
            DifficultyScreen.transform.Find("Medium").GetComponent<Button>().onClick.AddListener(() => DifficultyChoice(2));
            DifficultyScreen.transform.Find("Hard").GetComponent<Button>().onClick.AddListener(() => DifficultyChoice(3));
        }

        if (IBuild)
        {
            iOSPanel = IBuild.transform.Find("iOS Panel")?.gameObject;
        }

        audioSource = GetComponent<AudioSource>();
        GameEnemyManager = GetComponentInChildren<GameEnemyManager>();
        GameEnemyManager.currentWave = 0;
    }

    void Update()
    {
        if (GameEnemyManager.currentWave > GameEnemyManager.waveConfigurations.Count && !hasWon)
        {
            Win();
        }

        if (cardIsOnCD && UICard)
        {
            ApplyCooldown();
        }

        updateWager();
    }

    public void currentCardDeck()
    {
        if (deckShowing)
        {
            // close deck
            deckDisplayPanel.SetActive(false);
            deckShowing = false;
        } else
        {
            // open deck
            deckDisplayPanel.SetActive(true);
            displayDeck();
            deckShowing = true;
        }
    }

    public GameObject showCard(Card card, GameObject parent)
    {
        GameObject cardButton = Instantiate(cardButtonPrefab, parent.transform);
        cardButton.GetComponent<Image>().sprite = card.cardImage;
        cardButton.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardName;
        cardButton.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
        cardButton.transform.Find("Image").gameObject.GetComponent<Image>().sprite = card.effectImage;

        return cardButton;
    }

    public void displayDeck()
    {
        // Clear previous buttons
        foreach (Transform child in deckDisplayPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Card card in deckController.currentDeck)
        {
            showCard(card, deckDisplayPanel);
        }
    }

    public void useCard()
    {
        if (cardIsOnCD)
        { //don't do anything if the card is on CD
            return;
        }
        else
        {
            cardIsOnCD = true;
            cardCDTimer = cardCDTime;
            Card card = deckController.infinDrawCard(deckController.currentDeck);
            StartCoroutine(playCardSound(card));
            card.use(this);
            if(card.cardType == CardType.StatusEffect)
            {
                statusCard = card;
                statusApplied = true;
            }
            // CooldownImg.sprite = card.cardImage;

            // Get the RectTransform component of the prefab
            GameObject obj = showCard(card, PlayScreen);
            RectTransform rectTransform = obj.GetComponent<RectTransform>();

            //// Set the anchor to the top right corner
            rectTransform.anchorMin = new Vector2(1, 0.725f);  // Top-right corner
            rectTransform.anchorMax = new Vector2(1, 0.725f);  // Top-right corner
            rectTransform.pivot = new Vector2(1, 1);

            //// Adjust the position (0,0 will be the top right corner)
            ////rectTransform.anchoredPosition = new Vector2(193.55f, 266.95f);
            //obj.transform.localScale = obj.transform.localScale * 0.7f;
            StatusEffectManager.AddStatusEffect(card);
            updateHealth();
        }
    }

    public void ApplyCooldown()
    {
        cardCDTimer -= Time.deltaTime;

        if (cardCDTimer < 0)
        {
            //call the use function to unapply the status effect if it exists
            if (statusCard)
            {
                statusCard.use(this);
                StatusEffectManager.RemovePermanentStatusEffect(statusCard);
                statusCard = null;
                statusApplied = false;
                updateHealth();
            }
            cardIsOnCD = false;
            cardCDTimer = 0;
            UICard.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            audioSource.clip = DeckShuffle;
            audioSource.Play();
        }
        else
        {
            UICard.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.RoundToInt(cardCDTimer).ToString();

            Debug.Log("COOLDOWN FILL: " + CardCooldownImg.fillAmount);
            CardCooldownImg.fillAmount = cardCDTimer / cardCDTime;
        }
    }

    IEnumerator playCardSound(Card card)
    {
        audioSource.clip = CardPullAudio;
        audioSource.Play();
        yield return new WaitForSeconds(CardPullAudio.length);
        if (card.cardType == CardType.EnemyBuff)
        {
            audioSource.clip = BadPullAudio;
        }
        else
        {
            audioSource.clip = GoodPullAudio;
        }
        audioSource.Play();
    }

    // method to be called on level load, resets the players stats to base
    public void resetPlayer()
    {
        hasWon = false;
        GameEnemyManager.shouldSpawn = true;
        playerController.resetPlayerDamage();
        healthMax = baseHealth;     
        healthCurrent = healthMax;
    }

    public void freeze(bool status)
    {
        if (status)
        {
            playerController.SetControls(false);
            Time.timeScale = 0f;
        } 
        else
        {
            playerController.SetControls(true);
            Time.timeScale = 1f;
        }   
    }

    //method to update UI for health + check death condition
    //if the player dies, returns true, otherwise false 
    public void updateHealth()
    {
        if (healthCurrent > healthMax) healthCurrent = healthMax;
        healthBar.value = healthCurrent;
        if (healthCurrent <= 0) 
        {
            Death();
        }
    }

    public void updateWager()
    {
        if (money_text != null)
        {
            money_text.text = " " + money.ToString();
        }
    }

    public void Difficulty()
    {
        freeze(true);
        PlayScreen.SetActive(false);
        DifficultyScreen.SetActive(true);
    }

    public void DifficultyChoice(int value)
    {
        freeze(false);
        // money = value;
        GameEnemyManager.SetDifficulty(value);
        PlayScreen.SetActive(true);
        DifficultyScreen.SetActive(false);
    }

    public void Pause()
    {
        if (!paused)
        {
            // pause
            freeze(true);
            paused = true;
            PlayScreen.SetActive(false);
            PauseScreen.SetActive(true);
        }
        else
        {
            // unpause
            freeze(false);
            paused = false;
            PlayScreen.SetActive(true);
            PauseScreen.SetActive(false);
        }
    }

    public void Death()
    {
        playerController.SetControls(false);
        // animate death here
        DeathScreen.SetActive(true);
        TextMeshProUGUI ScoreText = DeathScreen.GetComponentInChildren<TextMeshProUGUI>();
        ScoreText.text = "Final Payout: " + money.ToString();
    }
    
    public void Win()
    {
        playerController.SetControls(false);
        // animate win here (gangnam style)
        WinScreen.SetActive(true);
        TextMeshProUGUI ScoreText = WinScreen.GetComponentInChildren<TextMeshProUGUI>();
        ScoreText.text = "Final Payout: " + money.ToString();
        hasWon = true;
    }

    #region FX
    // hit stop (scaling) + screen shake (if strong enough)
    public IEnumerator HitStop(float totalTime) {
        if (!InHitStop) {
            Time.timeScale = 0.01f; // FREEEZE TIME!
            InHitStop = true;
            yield return new WaitForSecondsRealtime(totalTime);
            Time.timeScale = 1.0f; // unfreeze time :(
            InHitStop = false;
        }
    }

    public IEnumerator ScreenShake(float totalTime, float shakeMultiplier) {
        Vector3 startPos = Camera.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < totalTime) {
            elapsedTime += Time.unscaledDeltaTime;
            float strength = Curve.Evaluate(elapsedTime / totalTime);
            Camera.transform.position = startPos + UnityEngine.Random.insideUnitSphere * strength * shakeMultiplier;
            yield return null;
        }

        Camera.transform.position = startPos;
    }

    // in future can prob do the outline ring like in CoD or smth, and can have its min alpha increase as player hp lowers
    public IEnumerator HurtFlash() {
        float elapsedTime = 0f;
        while (elapsedTime < totalHurtFlashTime) {
            float strength = hurtFlashCurve.Evaluate(elapsedTime / totalHurtFlashTime);
            elapsedTime += Time.deltaTime;
            Color c = hurtFlashImage.color;
            c.a = strength;
            hurtFlashImage.color = c;
            yield return null;
        }
    }
    #endregion

    public new void OpenMap()
    {
        GameEnemyManager.shouldSpawn = false; // THIS
        SceneManager.LoadScene("MAP");
    }
}
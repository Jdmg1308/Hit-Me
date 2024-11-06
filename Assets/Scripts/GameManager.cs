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

    public bool CheatWin = false;
    public string LastScene = "TUTORIAL";

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
    public int deckLimit;
    protected Image UICard;
    protected Image CardCooldownImg;
    protected StatusEffectManager StatusEffectManager;
    //protected bool deckShowing = false;
    [HideInInspector]
    public GameObject CardUIDeck;
    protected GameObject deckDisplayPanel;
    protected GameObject latestCard = null;
    protected GameObject cardDescriptor;
    protected GameObject DrawnCard;
    private int NofCardsUsed = 0;

    [Header("Health")]
    public int healthCurrent;       // Current health of the player
    public int healthMax = 100;     // Maximum health of the player (dynamically changed by cards)
    public int baseHealth = 100;    // base maximum HP of player 
   // [HideInInspector]
    public Slider healthBar;       // UI Slider for health bar

    [Header("Money")]
    public float money = 500;
    public float level_gathered = 0;

    [Header("Progress Meter")]
    public Slider progressBar;      // UI slidere for health bar 
    public int points;              // progress bar points 
    public bool milestone1;
    public int milestone1Points;
    public bool milestone2;
    public int milestone2Points;
    public bool milestone3;
    public int milestone3Points;

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
    protected GameObject ChecklistScreen;

    [HideInInspector]
    public ChecklistScreen ChecklistScript;

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

    AudioManager audioManager;

    #region Setup and update data
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
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
            Debug.LogError("GameManager instance is not assigned.");
        return instance;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignReferences(); // Reassign UI references
        string levelName = scene.name;

        Debug.Log("Loaded scene: " + levelName);

        if (audioManager != null)
        {
            audioManager.PlayLevelMusic(levelName);
        }
        else
        {
            Debug.LogWarning("AudioManager not found!");
        }
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
            ChecklistScreen = Canvas.transform.Find("Checklist Screen")?.gameObject;
        }

        if (PlayScreen)
        {
            money_text = PlayScreen.transform.Find("Money")?.gameObject.GetComponent<TextMeshProUGUI>();
            CardUIDeck = PlayScreen.transform.Find("CardUIDeck")?.gameObject;
            if (CardUIDeck)
            {
                UICard = CardUIDeck.GetComponent<Image>();
                CardCooldownImg = CardUIDeck.transform.Find("Cooldown")?.gameObject.GetComponent<Image>();
                StatusEffectManager = CardUIDeck.GetComponent<StatusEffectManager>();

                StartCoroutine(IdleToShinyCardSequence(CardUIDeck.GetComponent<Animator>()));
            }

            DrawnCard = PlayScreen.transform.Find("DrawnCard")?.gameObject;

            healthBar = PlayScreen.GetComponentInChildren<Slider>();

            //resets for progress meter
            //TODO: assign slider for progress bar
            milestone1 = false;
            milestone2 = false;
            milestone3 = false;
            points = 0;

            hurtFlashImage = PlayScreen.transform.Find("HurtFlash")?.gameObject.GetComponent<Image>();

            deckDisplayPanel = PlayScreen.transform.Find("DeckDisplayPanel")?.gameObject;

            cardDescriptor = PlayScreen.transform.Find("CardDescriptor")?.gameObject;

            money_text.text = " " + money.ToString();
        }

        if (DeathScreen || WinScreen || PauseScreen)
        {
            if (DeathScreen)
            {
                AssignButton(PauseScreen.transform, "Menu", OpenMenu);
                AssignButton(DeathScreen.transform, "Restart", Restart);
            }

            if (WinScreen)
                AssignButton(WinScreen.transform, "Shop", OpenShop);

            if (PauseScreen)
            {
                //AssignButton(PauseScreen.transform, "Menu", OpenShop);
                AssignButton(PauseScreen.transform, "Resume", Pause);

                deckDisplayPanel = PauseScreen.transform.Find("DeckDisplayPanel")?.gameObject;
            }
        }

        if (ChecklistScreen)
        {
            ChecklistScript = ChecklistScreen.GetComponent<ChecklistScreen>();
        }

        if (IBuild)
            iOSPanel = IBuild.transform.Find("iOS Panel")?.gameObject;

        if (deckDisplayPanel)
            updateDeckPanel();

        audioSource = GetComponent<AudioSource>();
        GameEnemyManager = GetComponentInChildren<GameEnemyManager>();
        GameEnemyManager.ResetWaves();
    }

    

    void Update()
    {
        if ((GameEnemyManager.currentWave > GameEnemyManager.waveConfigurations.Count && !hasWon) || CheatWin)
            Win();

        if (cardIsOnCD && UICard)
            ApplyCooldown();

        updateWager();
    }
    #endregion

    #region Cards
    public GameObject showCard(Card card, GameObject parent)
    {
        GameObject cardButton = Instantiate(cardButtonPrefab, parent.transform);
        cardButton.GetComponent<Image>().sprite = card.cardImage;
        cardButton.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardName;
        cardButton.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = card.cardDescription;
        cardButton.transform.Find("Image").gameObject.GetComponent<Image>().sprite = card.effectImage;

        return cardButton;
    }

    public void updateDeckPanel()
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
        NofCardsUsed++;
        if (NofCardsUsed >= 3)
            ChecklistScript.UpdateChecklistItem("DC", true);
        if (cardIsOnCD)
        {
            //don't do anything if the card is on CD
            return;
        }
        else
        {
            cardIsOnCD = true;
            cardCDTimer = cardCDTime;

            Card card = deckController.infinDrawCard(deckController.currentDeck);
            StartCoroutine(DrawCardSequence(DrawnCard.GetComponent<Animator>(), card));
            //TODO: updatePoints(card point value);
            updatePoints(10);
            updateHealth();
        }
    }

    private IEnumerator DrawCardSequence(Animator cardAnimator, Card card)
    {
        if (cardAnimator != null)
        {
            // delete previous card if any
            if (latestCard != null)
                Destroy(latestCard);

            // Play the "drawCard" animation once
            if (cardAnimator != null)
            {
                cardAnimator.Play("DrawCard");
                yield return new WaitForSeconds(cardAnimator.GetCurrentAnimatorStateInfo(0).length);
            }

            // Then play the "flipCard" animation
            Debug.Log("Texture Name: " + card.cardImage.texture.name); // Log the texture name for debugging
                                                                       // Check if the animator still exists to avoid null reference errors
            if (cardAnimator != null)
            {
                if (card.cardImage.texture.name == "goodCard")
                    cardAnimator.Play("FlipCardBlue");
                else
                    cardAnimator.Play("FlipCardRed");
                yield return new WaitForSeconds(cardAnimator.GetCurrentAnimatorStateInfo(0).length);
            }

            // PlayAudio
            StartCoroutine(playCardSound(card));

            // Show what the card actually is
            latestCard = showCard(card, DrawnCard);
            RectTransform latestRect = latestCard.GetComponent<RectTransform>();
            latestRect.anchorMin = Vector2.zero;
            latestRect.anchorMax = Vector2.one;
            latestRect.offsetMin = Vector2.zero;
            latestRect.offsetMax = Vector2.zero;

            // Set effect of card
            card.use(this);
            if (card.cardType == CardType.StatusEffect)
            {
                statusCard = card;
                statusApplied = true;
                updateHealth();
            }

            StatusEffectManager.AddStatusEffect(card);

            // Show Card Descriptor
            ShowCardDescriptor(card);

            // Return to idle state
            if (cardAnimator != null)
                cardAnimator.Play("NoCardDrawn");
        }
    }

    public void ShowCardDescriptor(Card card)
    {
        // animate descriptor
        Animator animator = cardDescriptor.GetComponent<Animator>();
        TextMeshProUGUI description = cardDescriptor.GetComponentInChildren<TextMeshProUGUI>();
        description.text = card.cardName;
        PlayAnimationOnce(animator, "LookAtMe");
    }

    public void ApplyCooldown()
    {
        // Update cooldown timer
        cardCDTimer -= Time.deltaTime;

        // Check if cooldown timer is below 0
        if (cardCDTimer < 0.01)
            cardCDTimer = 0; // Reset to zero if below 0

        if (cardCDTimer == 0)
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
            StartCoroutine(IdleToShinyCardSequence(CardUIDeck.GetComponent<Animator>()));
        }
        else
        {
            UICard.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.RoundToInt(cardCDTimer).ToString();
            CardUIDeck.GetComponent<Animator>().Play("IdleDeck");
        }
        CardCooldownImg.fillAmount = cardCDTimer / cardCDTime;
    }

    private IEnumerator IdleToShinyCardSequence(Animator drawingDeckAnimator)
    {
        Scene startingScene = SceneManager.GetActiveScene();
        // Loop for idle -> shiny -> idle while not on cooldown
        while (!cardIsOnCD && !hasWon && startingScene == SceneManager.GetActiveScene())
        {
            // Check if the animator still exists to avoid null reference errors
            if (drawingDeckAnimator == null)
                yield break; // Exit the coroutine if the Animator is missing

            // Play the "Shiny" animation once
            drawingDeckAnimator.Play("Shiny");
            yield return new WaitForSeconds(drawingDeckAnimator.GetCurrentAnimatorStateInfo(0).length);

            // Check if the animator still exists to avoid null reference errors
            if (drawingDeckAnimator == null)
                yield break; // Exit the coroutine if the Animator is missing

            // Play the "Idle" animation for 5 seconds
            drawingDeckAnimator.Play("IdleDeck");
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator playCardSound(Card card)
    {
        audioSource.clip = CardPullAudio;
        audioSource.Play();
        yield return new WaitForSeconds(CardPullAudio.length);
        if (card.cardType == CardType.EnemyBuff)
            audioSource.clip = BadPullAudio;
        else
            audioSource.clip = GoodPullAudio;
        audioSource.Play();
    }

    private void PlayAnimationOnce(Animator animator, string animationStateName)
    {
        animator.enabled = true;
        // Play the specified animation state
        animator.Play(animationStateName, -1, 0f);
    }
    #endregion

    #region UI_elements and screens
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
            Death();
    }

    public void updateWager()
    {
        if (money_text != null)
            money_text.text = " " + (money + level_gathered).ToString();
    }

    public void updatePoints(int numPoints)
    {
        //TODO: reset points on new level load 
        points += numPoints;
        if (points >= milestone1Points && !milestone1)
        {
            //reached milestone 1 for the first time 
            //TODO: do the rewards, play audio or something idk
            Debug.Log("milestone 1 reached");
            milestone1 = true;
        }
        else if (points >= milestone2Points && !milestone2)
        {
            Debug.Log("mileestone 2 reached");
            milestone2 = true;
        }
        else if (points >= milestone3Points && !milestone3)
        {
            Debug.Log("milestone 3 reached");
            milestone3 = true;
        }
        //TODO: uncomment once we get the bar 
        Debug.Log("points: " + points);
        //progressBar.value = points;
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
        updateDeckPanel();
        if (!paused)
        {
            // pause
            freeze(true);
            paused = true;
            PauseScreen.SetActive(true);
        }
        else
        {
            // unpause
            freeze(false);
            paused = false;
            PauseScreen.SetActive(false);
        }
    }

    public void Death()
    {
        playerController.SetControls(false);
        // animate death here
        hasWon = true;
        DeathScreen.SetActive(true);
        TextMeshProUGUI ScoreText = DeathScreen.GetComponentInChildren<TextMeshProUGUI>();
        ScoreText.text = "Final Payout: " + money.ToString();
    }

    public void Win()
    {
        playerController.SetControls(false);
        // animate win here (gangnam style)
        WinScreen.SetActive(true);
        CheatWin = false;
        //TextMeshProUGUI ScoreText = WinScreen.GetComponentInChildren<TextMeshProUGUI>();
        //ScoreText.text = "Final Payout: " + money.ToString();
        hasWon = true;
    }
    #endregion

    #region FX
    // hit stop (scaling) + screen shake (if strong enough)
    public IEnumerator HitStop(float totalTime)
    {
        if (!InHitStop)
        {
            Time.timeScale = 0.01f; // FREEEZE TIME!
            InHitStop = true;
            yield return new WaitForSecondsRealtime(totalTime);
            Time.timeScale = 1.0f; // unfreeze time :(
            InHitStop = false;
        }
    }

    public IEnumerator ScreenShake(float totalTime, float shakeMultiplier)
    {
        Vector3 startPos = Camera.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float strength = Curve.Evaluate(elapsedTime / totalTime);
            Camera.transform.position = startPos + UnityEngine.Random.insideUnitSphere * strength * shakeMultiplier;
            yield return null;
        }

        Camera.transform.position = startPos;
    }

    // in future can prob do the outline ring like in CoD or smth, and can have its min alpha increase as player hp lowers
    public IEnumerator HurtFlash()
    {
        float elapsedTime = 0f;
        while (elapsedTime < totalHurtFlashTime)
        {
            float strength = hurtFlashCurve.Evaluate(elapsedTime / totalHurtFlashTime);
            elapsedTime += Time.deltaTime;
            Color c = hurtFlashImage.color;
            c.a = strength;
            hurtFlashImage.color = c;
            yield return null;
        }
    }
    #endregion

    public new void OpenShop()
    {
        LastScene = SceneManager.GetActiveScene().name;
        GameEnemyManager.shouldSpawn = false; // THIS
        SceneManager.LoadScene("SHOP");
    }
}
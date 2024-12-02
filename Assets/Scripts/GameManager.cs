using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("General")]
    public GameEnemyManager GameEnemyManager;
    public GameObject Player;
    public GameObject Canvas;
    public GameObject Camera;

    public GameObject nextLevelLoad;

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
    protected float cardCDTime = 10.0f, cardCDTimer = 0;
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
    private int BlueCardsDrawn = 0;
    private int RedCardsDrawn = 0;
    private int GreenCardsDrawn = 0;

    [Header("Health")]
    public int healthCurrent;       // Current health of the player
    public int healthMax = 100;     // Maximum health of the player (dynamically changed by cards)
    public int baseHealth = 100;    // base maximum HP of player 
   // [HideInInspector]
    public Slider healthBar;       // UI Slider for health bar
    public Color healthBarDefaultColor;

    [Header("Progress Meter")]
    private Animator pointmeterAnimator;
    private TextMeshProUGUI statsText;
    public int EnemiesKilled = 0;
    public int pointsPerKill;
    public int progressMax = 100;
    public TextMeshProUGUI points_text;
    public Slider progressBar;      // UI slidere for health bar 
    public bool milestone1;
    public int milestone1Points;
    public bool milestone2;
    public int milestone2Points;
    public bool milestone3;
    public int milestone3Points;
    public int CountFPS = 30;
    public float Duration = 1f;
    public int _points = 0;
    public int Points
    {
        get
        {
            return _points;
        }
        set
        {
            StartCoroutine(PlayAnimationOnce(pointmeterAnimator, "PointMeterGoesUp"));
            UpdateText(value, _points, points_text);
            _points = value;
        }
    }
    Coroutine updatePointsRoutine;

    [Header("Money")]
    public float pointsToMoneyConversionRate = 0.5f;
    public TextMeshProUGUI money_text;
    public int _money = 500;
    public int Money
    {
        get
        {
            return _money;
        }
        set
        {
            UpdateText(value, _money, money_text);
            _money = value;
        }
    }

    private Coroutine CountingCoroutine;

    [Header("Mobile")]
    public bool mobile;
    [HideInInspector]
    public GameObject iOSPanel;

    [HideInInspector]
    public Player p;
    [HideInInspector]
    public PlayerController playerController;

    protected GameObject PlayScreen;
    protected GameObject DifficultyScreen;
    protected GameObject PauseScreen;
    protected GameObject DeathScreen;
    protected GameObject WinScreen;
    protected GameObject IBuild;
    protected GameObject ChecklistScreen;

    [HideInInspector]
    public ChecklistScreen ChecklistScript;

    [SerializeField]
    private List<string> levelSceneNames;

    protected bool paused = false;

    [Header("FX")]
    private Image DarkOverlayImage;
    // hit fx
    public bool InHitStop = false;
    public AnimationCurve Curve;
    // hurt fx
    public float totalHurtFlashTime;
    public AnimationCurve hurtFlashCurve;
    private Image hurtFlashImage;
    // point fx
    public GameObject pointIndicationPrefab;

    public static GameManager instance;

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
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
            Debug.LogError("GameManager instance is not assigned.");
        return instance;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (updatePointsRoutine != null)
        {
            StopCoroutine(updatePointsRoutine);
            updatePointsRoutine = null;
        }
        
        AssignReferences(); // Reassign UI references
        string levelName = scene.name;

        Debug.Log("Loaded scene: " + levelName);

        if (audioManager != null)
            audioManager.PlayLevelMusic(levelName);
        else
            Debug.LogWarning("AudioManager not found!");
    }

    public void AssignButton(Transform screen, string buttonName, UnityEngine.Events.UnityAction action)
    {
        Button button = screen.Find(buttonName)?.GetComponent<Button>();
        if (button) button.onClick.AddListener(action);
    }

    void AssignReferences()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
        Player = GameObject.FindGameObjectWithTag("Player");
        Camera = GameObject.FindGameObjectWithTag("MainCamera");

        nextLevelLoad = GameObject.FindGameObjectWithTag("nextLevelLoad");

        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

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

            DarkOverlayImage = Canvas.transform.Find("DarkOverlayImage")?.gameObject.GetComponent<Image>();
        }

        if (PlayScreen)
        {
            // in the shop
            money_text = PlayScreen.transform.Find("Money")?.gameObject.GetComponent<TextMeshProUGUI>();
            if (money_text)
                money_text.text = " " + Money.ToString();


            CardUIDeck = PlayScreen.transform.Find("CardUIDeck")?.gameObject;
            if (CardUIDeck)
            {
                UICard = CardUIDeck.GetComponent<Image>();
                CardCooldownImg = CardUIDeck.transform.Find("Cooldown")?.gameObject.GetComponent<Image>();
                StatusEffectManager = CardUIDeck.GetComponent<StatusEffectManager>();

                StartCoroutine(IdleToShinyCardSequence(CardUIDeck.GetComponent<Animator>()));
            }

            DrawnCard = PlayScreen.transform.Find("DrawnCard")?.gameObject;
            progressBar = PlayScreen.transform.Find("PointMeter")?.gameObject.GetComponent<Slider>();

            points_text = PlayScreen.transform.Find("PointMeter")?.Find("Points")?.gameObject.GetComponent<TextMeshProUGUI>();
            pointmeterAnimator = PlayScreen.transform.Find("PointMeter")?.gameObject.GetComponent<Animator>();
            if (points_text)
                points_text.text = " " + Points.ToString();

            healthBar = PlayScreen.GetComponentInChildren<Slider>();
            healthBarDefaultColor = healthBar.fillRect.GetComponent<Image>().color;

            //resets for progress meter
            //TODO: assign slider for progress bar
            milestone1 = false;
            milestone2 = false;
            milestone3 = false;
            Points = 0;

            hurtFlashImage = PlayScreen.transform.Find("HurtFlash")?.gameObject.GetComponent<Image>();
            deckDisplayPanel = PlayScreen.transform.Find("DeckDisplayPanel")?.gameObject;
            cardDescriptor = PlayScreen.transform.Find("CardDescriptor")?.gameObject;

            //availableLevels = new List<System.Action> { PlayLvl1, PlayLvl2, PlayLvl3, PlayLvl4 };
            //remainingLevels = new List<System.Action>(availableLevels);
        }

        if (DeathScreen || WinScreen || PauseScreen)
        {
            if (DeathScreen)
            {
                DeathScreen.transform.Find("Menu")?.GetComponent<Button>().onClick.AddListener(() => LoadNextScene("MENU")); 
                DeathScreen.transform.Find("Restart")?.GetComponent<Button>().onClick.AddListener(() => LoadNextScene(SceneManager.GetActiveScene().name));
            }

            if (WinScreen) 
            {

                Button button = WinScreen.transform.Find("Next")?.GetComponent<Button>();
                if (nextLevelLoad)
                {
                    if (button)
                        button.onClick.AddListener(() => LoadNextScene(nextLevelLoad.name));
                }
                else 
                {
                    button.onClick.AddListener(() => LoadNextScene("SHOP"));
                }
                statsText = WinScreen.transform.Find("Stats")?.GetComponentInChildren<TextMeshProUGUI>();
            } 

            if (PauseScreen)
            {
                //AssignButton(PauseScreen.transform, "Menu", OpenShop);
                PauseScreen.transform.Find("Menu")?.GetComponent<Button>().onClick.AddListener(() => LoadNextScene("MENU")); 
                AssignButton(PauseScreen.transform, "Resume", Pause);
                deckDisplayPanel = PauseScreen.transform.Find("DeckDisplayPanel")?.gameObject;

                money_text = PauseScreen.transform.Find("Money")?.gameObject.GetComponent<TextMeshProUGUI>();
                money_text.text = " " + Money.ToString();
            }
        }

        if (ChecklistScreen)
            ChecklistScript = ChecklistScreen.GetComponent<ChecklistScreen>();

        if (IBuild)
            iOSPanel = IBuild.transform.Find("iOS Panel")?.gameObject;

        if (deckDisplayPanel)
            updateDeckPanel();

        audioSource = GetComponent<AudioSource>();
        GameEnemyManager = GetComponentInChildren<GameEnemyManager>();
        GameEnemyManager.ResetWaves();

        BlueCardsDrawn = 0 ; RedCardsDrawn = 0; GreenCardsDrawn = 0;
        EnemiesKilled = 0;
}

    void Update()
    {
        if (!hasWon && CheatWin)
            Win();

        if (cardIsOnCD && UICard)
            ApplyCooldown();
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
        cardButton.transform.Find("Points").gameObject.GetComponent<TextMeshProUGUI>().text = card.points.ToString();

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
        if (CardUIDeck && CardUIDeck.activeSelf)
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
                updateHealth();
            }
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
                switch (card.color)
                {
                    case ColorType.Blue:
                        cardAnimator.Play("FlipCardBlue");
                        break;
                    case ColorType.Red:
                        cardAnimator.Play("FlipCardRed");
                        break;
                    case ColorType.Green:
                        cardAnimator.Play("FlipCardGreen");
                        break;
                    default:
                        break;
                }

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

            yield return StartCoroutine(ShowCardDescriptor(card));

            // Return to idle state
            if (cardAnimator != null)
                cardAnimator.Play("NoCardDrawn");
        }
    }

    public IEnumerator ShowCardDescriptor(Card card)
    {
        // animate descriptor
        Animator animator = cardDescriptor.GetComponent<Animator>();
        TextMeshProUGUI description = cardDescriptor.GetComponentInChildren<TextMeshProUGUI>();
        description.text = card.cardName;
        description.color = Color.white;
        Image bg = cardDescriptor.GetComponent<Image>();

        switch (card.color)
        {
            case ColorType.Blue:
                BlueCardsDrawn++;
                bg.color = Color.blue;
                break;
            case ColorType.Red:
                RedCardsDrawn++;
                bg.color = Color.red;
                break;
            case ColorType.Green:
                GreenCardsDrawn++;
                bg.color = Color.green;
                break;
            default:
                break;
        }

        updatePoints(card.points);
        PointIndication(card.points, Player.transform.position);
        yield return StartCoroutine(PlayAnimationOnce(animator, "LookAtMe"));
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

    private IEnumerator PlayAnimationOnce(Animator animator, string animationStateName)
    {
        if (animator)
        {
            Debug.Log("play Animation once for: " + animator);
            animator.enabled = true;

            // Play the animation
            animator.Play(animationStateName, -1, 0f);

            // Wait a frame for the animator to update
            yield return null;

            // Get the length of the current animation
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
        }
    }
    #endregion

    #region UI_elements and screens
    // method to be called on level load, resets the players stats to base
    public void resetPlayer()
    {
        hasWon = false;
        // GameEnemyManager.shouldSpawn = true;
        playerController.resetPlayerDamage();
        healthMax = baseHealth;
        healthCurrent = healthMax;

    }

    public void freeze(bool status)
    {
        if (status)
        {
            playerController?.SetControls(false);
            Time.timeScale = 0f;
        }
        else
        {
            playerController?.SetControls(true);
            Time.timeScale = 1f;
        }
    }

    //method to update UI for health + check death condition
    //if the player dies, returns true, otherwise false 
    public void updateHealth()
    {
        if (healthCurrent > healthMax) healthCurrent = healthMax;
        healthBar.value = healthCurrent;

        // Update health bar color based on health percentage
        Color healthColor = healthBarDefaultColor; // Default: Healthy
        if (healthBar.value <= 33f)
            healthColor = Color.red; // Low health
        else if (healthBar.value <= 66f)
            healthColor = Color.yellow; // Medium health

        healthBar.fillRect.GetComponent<Image>().color = healthColor;

        if (healthCurrent <= 0)
            Death();
    }

    public void updatePoints(int numPoints)
    {
        //TODO: reset points on new level load 
        Points += numPoints;

        updatePointsRoutine = StartCoroutine(updatePointsSlider());

        if (Points >= milestone1Points && !milestone1)
        {
            //reached milestone 1 for the first time 
            //TODO: do the rewards, play audio or something idk
            MilestoneAnimation("milestone 1 reached \n bonus cash");
            milestone1 = true;
        }
        else if (Points >= milestone2Points && !milestone2)
        {
            MilestoneAnimation("milestone 2 reached \n healing");
            milestone2 = true;
        }
        else if (Points >= milestone3Points && !milestone3)
        {
            MilestoneAnimation("milestone 3 reached \n extra damage");
            milestone3 = true;
        }
        //TODO: uncomment once we get the bar 
        Debug.Log("points: " + Points);
        //progressBar.value = points;
        if (updatePointsRoutine != null)
            StopCoroutine(updatePointsRoutine);
        updatePointsRoutine = StartCoroutine(updatePointsSlider());
    }

    private void UpdateText(int newValue, int prevValue, TextMeshProUGUI text)
    {
        if (CountingCoroutine != null)
            StopCoroutine(CountingCoroutine);

        Debug.Log(newValue + " text " + text + " prev " + prevValue);

        CountingCoroutine = StartCoroutine(CountText(newValue, prevValue, text));
    }

    private IEnumerator updatePointsSlider()
    {
        int newValue = Points;
        int prevValue = (int) progressBar.value;
        WaitForSeconds wait = new WaitForSeconds(1f / CountFPS);
        int stepAmount;

        stepAmount = Mathf.CeilToInt((newValue - prevValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0

        while (prevValue != newValue)
        {
            prevValue += stepAmount;

            // Clamp value to ensure it doesn't overshoot the target
            if ((stepAmount > 0 && prevValue > newValue) || (stepAmount < 0 && prevValue < newValue))
                prevValue = newValue;

            progressBar.value = prevValue;
            yield return wait;
        }
    }

    // The coroutine that counts up to the new money value in increments
    private IEnumerator CountText(int newValue, int prevValue, TextMeshProUGUI text)
    {
        Debug.Log(prevValue);
        WaitForSeconds wait = new WaitForSeconds(1f / CountFPS);
        int stepAmount;

        if (newValue - prevValue < 0)
            stepAmount = Mathf.FloorToInt((newValue - prevValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
        else
            stepAmount = Mathf.CeilToInt((newValue - prevValue) / (CountFPS * Duration)); // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0

        while (prevValue != newValue)
        {
            prevValue += stepAmount;

            // Clamp value to ensure it doesn't overshoot the target
            if ((stepAmount > 0 && prevValue > newValue) || (stepAmount < 0 && prevValue < newValue))
                prevValue = newValue;

            text.SetText(prevValue.ToString());
            yield return wait;
        }
    }

    public void MilestoneAnimation(string text)
    {
        // animate descriptor
        cardDescriptor.GetComponent<Image>().color = Color.yellow;
        Animator animator = cardDescriptor.GetComponent<Animator>();
        TextMeshProUGUI description = cardDescriptor.GetComponentInChildren<TextMeshProUGUI>();
        description.text = text;
        description.color = Color.yellow;
        audioSource.clip = GoodPullAudio;
        audioSource.Play();
        StartCoroutine(PlayAnimationOnce(animator, "LookAtMe"));
    }

    public void WaveStartingAnimation(string text)
    {
        // animate descriptor
        cardDescriptor.GetComponent<Image>().color = Color.gray;
        Animator animator = cardDescriptor.GetComponent<Animator>();
        TextMeshProUGUI description = cardDescriptor.GetComponentInChildren<TextMeshProUGUI>();
        description.text = text;
        description.color = Color.white;
        StartCoroutine(PlayAnimationOnce(animator, "LookAtMe"));
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
        ScoreText.text = "Final Payout: " + Money.ToString();
        Money = 500;   
    }

    public void Win()
    {
        // playerController.SetControls(false);

        statsText.text = "CARDS DRAWN: \nBlue: " + BlueCardsDrawn + " Red: " + RedCardsDrawn + " Green: "  + GreenCardsDrawn + " \nENEMIES KILLED: " + EnemiesKilled;

        money_text = WinScreen.transform.Find("Money")?.gameObject.GetComponent<TextMeshProUGUI>();
        if (money_text)
           money_text.text = " " + Money.ToString();

        // animate win here (gangnam style)
        WinScreen.SetActive(true);

        // Add points To Money
        Debug.Log("IF YOU FUCKING DARE SAY NULL " + money_text);
        Money += Points; // pointsToMoneyConversionRate (int) (Points)

        // Make Points zero
        Points = 0;
        if (updatePointsRoutine != null)
            StopCoroutine(updatePointsRoutine);
        updatePointsRoutine = StartCoroutine(updatePointsSlider());


        CheatWin = false;
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

    public void PointIndication(int points, Vector3 position)
    {
        Vector3 offset = position + new Vector3(0, 3, 0);
        GameObject indicator = Instantiate(pointIndicationPrefab, offset, Quaternion.identity);
        TextMeshPro textDisplay = indicator.GetComponent<TextMeshPro>();
        float xyScale = .15f;
        if (textDisplay != null)
        {
            textDisplay.text = "+" + points.ToString();
            if (points <= 50)
            {
                textDisplay.color = Color.white;
                xyScale = .15f;
            }
            else if (points <= 100)
            {
                textDisplay.color = Color.white;
                xyScale = .2f;
            }
            else
            {
                textDisplay.color = Color.yellow;
                xyScale = .25f;
            }
        }
        indicator.transform.localScale = new Vector3(xyScale, xyScale, 1);
        
    }
    #endregion

    public void LoadNextScene(string name)
    {
        if (updatePointsRoutine != null)
        {
            StopCoroutine(updatePointsRoutine);
            updatePointsRoutine = null;
        }
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
            CountingCoroutine = null;
        }
        StopAllCoroutines();
        if (name == "SHOP")
            GameEnemyManager.shouldSpawn = false;
        else
            GameEnemyManager.shouldSpawn = true;

        LastScene = SceneManager.GetActiveScene().name;

        // Pause game and start fade
        StartCoroutine(FadeAndLoadScene(name));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        freeze(true);
        // Ensure the fade overlay is visible
        Debug.Log("BRUHHHHHHHHHHHHH" + DarkOverlayImage);
        if (DarkOverlayImage != null)
        {
            DarkOverlayImage.gameObject.SetActive(true);
            // Fade to black
            yield return StartCoroutine(FadeScreen(0f, 1f, 0.5f));
        }

        // Actually load the next scene
        SceneManager.LoadScene(sceneName);
        freeze(false);
    }

    private IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color overlayColor = DarkOverlayImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            overlayColor.a = newAlpha;
            DarkOverlayImage.color = overlayColor; // Apply interpolated alpha
            yield return null;
        }

        // Ensure the final alpha value is set
        overlayColor.a = endAlpha;
        DarkOverlayImage.color = overlayColor;
    }

    public void ContinueToRandomLevel()
    {
        if (levelSceneNames == null || levelSceneNames.Count == 0)
        {
            Debug.LogError("No levels specified in the levelSceneNames list!");
            return;
        }

        // Pick a random scene name from the list
        string randomScene = levelSceneNames[UnityEngine.Random.Range(0, levelSceneNames.Count)];

        // Load the randomly chosen scene
        LoadNextScene(randomScene);
    }
}
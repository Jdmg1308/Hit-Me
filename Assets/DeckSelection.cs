using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckSelection : ShopManager
{

    private GameManager GM;
    private GameObject DeckSelectionScreen;
    private Button ContinueButton;

    private void Awake()
    {
        AssignReferences();

        ContinueButton = DeckSelectionScreen.transform.Find("Continue")?.GetComponent<Button>();
        if (ContinueButton) ContinueButton.onClick.AddListener(ContinueToRandomLevel);
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

    void Start()
    {
        SetUpChoices();

    }

    /*
     * Sets up the cards in the shop to be random & configures the button sprites & listeners
     * to be called whenever a level is beaten.
     */
    public void SetUpChoices()
    {
        //int[] _randCards = new int[3];
        //for (int i = 0; i < 3; i++)
        //{
        //    _randCards[i] = Random.Range(0, allPurchasableCards.Count);
        //}

        //GameObject displayPanel = Options.transform.Find("ShopDisplayPanel").gameObject;
        //// Using _randCards[i] instead of 0, 1, 2
        //GameObject child0 = GM.showCard(allPurchasableCards[_randCards[0]], displayPanel);
        //child0.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[0], child0));

        //GameObject child1 = GM.showCard(allPurchasableCards[_randCards[1]], displayPanel);
        //child1.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[1], child1));

        //GameObject child2 = GM.showCard(allPurchasableCards[_randCards[2]], displayPanel);
        //child2.GetComponentInChildren<Button>().onClick.AddListener(() => PurchaseCard(_randCards[2], child2));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

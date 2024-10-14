using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusEffectManager : MonoBehaviour
{
    public RectTransform canvasRectTransform;
    public GameObject statusEffectPrefab;
    public float spacing = 10;
    public GameManager GM;

    private List<GameObject> statusEffects = new List<GameObject>();

    private void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    public void AddStatusEffect(Card card)
    {
        Sprite EffectImage = card.effectImage;
        // Create a new status effect image from the prefab
        GameObject newStatusEffect = Instantiate(statusEffectPrefab, canvasRectTransform);
        newStatusEffect.GetComponent<Image>().sprite = EffectImage;
        
        RectTransform rectTransform = newStatusEffect.GetComponent<RectTransform>();


        float totalWidth = rectTransform.rect.width + spacing;
        float xOffset = -(statusEffects.Count * totalWidth);

        rectTransform.anchorMax = new Vector2(1, 1); // Top-right corner
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(xOffset - 330, -48); // Adjust -200 for padding from top edge

        // Add the new image to the list
        statusEffects.Add(newStatusEffect);

        GameObject targetObject;

        if (card.cardType == CardType.Multiplier || card.cardType == CardType.Heal || card.cardType == CardType.PlayerBuff || card.cardType == CardType.Additor) 
        {
            targetObject = GM.Player;
        } else if (card.cardType == CardType.EnemyBuff || card.cardType == CardType.StatusEffect)
        {
            targetObject = GM.GameEnemyManager.spawnedEnemies[0];
        } else
        {
            Debug.LogError("Card has no recognizable set type L bozo");
            targetObject = null;
        }

        // Create a new status effect image for the target object (e.g., player or enemy)
        GameObject statusEffectOnTarget = Instantiate(statusEffectPrefab, targetObject.transform);
        statusEffectOnTarget.GetComponent<Image>().sprite = EffectImage;

        // Adjust position for the target object (e.g., on top of the player's or enemy's head)
        RectTransform targetRectTransform = statusEffectOnTarget.GetComponent<RectTransform>();
        targetRectTransform.anchorMax = new Vector2(0.5f, 1); // Center-top
        targetRectTransform.anchorMin = new Vector2(0.5f, 1);
        targetRectTransform.pivot = new Vector2(0.5f, 1);
        targetRectTransform.anchoredPosition = new Vector2(0, 50); // Adjust for the position above the target

        // Optionally, you can destroy the status effect after some time
        StartCoroutine(RemoveStatusEffectAfterTime(newStatusEffect, statusEffectOnTarget, 5f)); // Remove after 5 seconds
    }

    private IEnumerator RemoveStatusEffectAfterTime(GameObject statusEffectUI, GameObject statusEffectOnTarget, float duration)
    {
        yield return new WaitForSeconds(duration);

        // Call your method to remove the power-up here (e.g., RemovePowerUp())
        // RemovePowerUp();

        // Remove the status effect from the UI and the target object
        RemoveStatusEffect(statusEffectUI);
        Destroy(statusEffectOnTarget); // Remove from the target object
    }


    public void RemoveStatusEffect(GameObject statusEffect)
    {
        statusEffects.Remove(statusEffect);
        Destroy(statusEffect);

        // Re-arrange the positions of the remaining status effects
        for (int i = 0; i < statusEffects.Count; i++)
        {
            float xOffset = -(i * (spacing));
            RectTransform rectTransform = statusEffects[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xOffset - 330, -48); // Adjust for padding
        }
    }
}

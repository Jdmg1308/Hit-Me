using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusEffectManager : MonoBehaviour
{
    public RectTransform canvasRectTransform;
    public GameObject PermStatusEffectPrefab;
    public GameObject TempStatusEffectPrefab;
    public GameObject TempStatusEffectPrefabForEnemy;
    public float spacing = 10;
    public GameManager GM;

    private List<GameObject> PermanentStatusEffects = new List<GameObject>();

    private void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    public void AddEffectPermanently(Card card)
    {
        // Debug.LogError("Card is " + card.name);
        Sprite EffectImage = card.effectImage;
        GameObject permanentStatusEffect = Instantiate(PermStatusEffectPrefab, canvasRectTransform);
        permanentStatusEffect.GetComponent<Image>().sprite = EffectImage;

        RectTransform rectTransform = permanentStatusEffect.GetComponent<RectTransform>();

        float totalWidth = rectTransform.rect.width + spacing;
        float xOffset = -(PermanentStatusEffects.Count * totalWidth);
        rectTransform.anchorMax = new Vector2(1, 1); // Top-right corner
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(xOffset - 330, -48); // Adjust -200 for padding from top edge

        // Add the new image to the list
        PermanentStatusEffects.Add(permanentStatusEffect);
    }

    public void AboveEffectTemporarily(GameObject target, Card card, bool enemy)
    {
        Sprite EffectImage = card.effectImage;
        GameObject TempStatusEffect;
        if (enemy)
        {
            TempStatusEffect = Instantiate(TempStatusEffectPrefabForEnemy, target.transform);
        } else
        {
            TempStatusEffect = Instantiate(TempStatusEffectPrefab, target.transform);
        }
        TempStatusEffect.GetComponent<SpriteRenderer>().sprite = EffectImage;

        StartCoroutine(RemoveAboveEffectAfterTime(TempStatusEffect, 5f)); // Remove after card.time seconds
    }

    public void AddToEachEnemy(Card card)
    {
        List<Enemy> enemies = GM.GameEnemyManager.spawnedEnemies;
        foreach (Enemy enemy in enemies) {
            AboveEffectTemporarily(enemy.gameObject, card, true);
        }
    }

    public void AddStatusEffect(Card card)
    {
        if (card.cardType == CardType.EnemyBuff || card.cardType == CardType.EnemyDebuff)
        {
            AddToEachEnemy(card);
        } 
        else
        {
            AboveEffectTemporarily(GM.Player, card, false);
        }
        // AddEffectPermanently(card);

    }

    private IEnumerator RemoveAboveEffectAfterTime(GameObject statusEffect, float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(statusEffect);
    }

    public void RemovePermanentStatusEffect(Card card)
    {
        //for (int j = PermanentStatusEffects.Count - 1; j >= 0; j--)
        //{
        //    GameObject effect = PermanentStatusEffects[j];
        //    // Get the Image component from the GameObject
        //    Image imageComponent = effect.GetComponent<Image>();

        //    // Check if the GameObject has an Image component and the sprite matches
        //    if (imageComponent != null && imageComponent.sprite == card.effectImage)
        //    {
        //        PermanentStatusEffects.Remove(effect);
        //        Destroy(effect);

        //        // Re-arrange the positions of the remaining status effects
        //        for (int i = 0; i < PermanentStatusEffects.Count; i++)
        //        {
        //            float xOffset = -(i * (spacing));
        //            RectTransform rectTransform = PermanentStatusEffects[i].GetComponent<RectTransform>();
        //            rectTransform.anchoredPosition = new Vector2(xOffset - 330, -48); // Adjust for padding
        //        }
        //    }
        //}
    }
}

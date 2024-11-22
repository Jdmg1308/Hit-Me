using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tank Card Data", menuName = "ScriptableObjects/Card/Tank")]
public class Tank : Card
{
    public override CardType cardType { get { return CardType.StatusEffect; } }

    public GameObject tankPrefab; // Reference to the prefab in the editor
    private GameObject tankObject; // To keep track of the spawned object

    /*
     * Spawns an object below the player when applied,
     * and destroys the object when the effect is removed.
     */
    public override void use(GameManager GM)
    {
        if (!GM.statusApplied)
        {
            // Apply the status effect
            Debug.Log("Spawning object below the player");

            // Get the player's transform
            Transform playerTransform = GM.playerController.transform;

            // Calculate the position slightly below the player
            Vector3 spawnPosition = playerTransform.position;

            // Instantiate the object
            tankObject = Instantiate(tankPrefab, spawnPosition, Quaternion.identity, playerTransform);

            tankObject.GetComponent<SpriteRenderer>().sortingOrder = 2;

        }
        else
        {
            // Remove the status effect
            Debug.Log("Destroying the spawned object");

            if (tankObject != null)
            {
                Destroy(tankObject);
                tankObject = null;
            }
        }
    }
}

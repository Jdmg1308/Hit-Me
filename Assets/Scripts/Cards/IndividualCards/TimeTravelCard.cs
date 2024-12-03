using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TimeTravel Card Data", menuName = "ScriptableObjects/Card/TimeTravel")]
public class TimeTravelCard : Card
{
    public override CardType cardType { get { return CardType.StatusEffect; } }

    // A stack to store the player's positions over time
    private Stack<Vector3> positionHistory = new Stack<Vector3>();
    private List<GameObject> spriteMarkers = new List<GameObject>();
    private float recordInterval = 0.01f; // How often to record positions
    private Coroutine recordRoutine;
    public Color TimeTravelColor;
    public float rewindSpeedMultiplier = 2f; // Higher = faster rewind
    private int playerHealthBeforeRewind;

    public override void use(GameManager GM)
    {
        if (!GM.statusApplied)
        {
            // Apply status effect: Start recording
            Debug.Log("Applying TimeTravel effect");
            positionHistory.Clear(); // Clear old history
            recordRoutine = GM.StartCoroutine(RecordPlayerState(GM));
        }
        else
        {
            Debug.Log("Un-applying TimeTravel effect");

            // Unapply status effect: Move player through time
            if (recordRoutine != null)
                GM.StopCoroutine(recordRoutine);

            GM.StartCoroutine(RewindPlayerState(GM));
        }
    }

    private IEnumerator RecordPlayerState(GameManager GM)
    {
        playerHealthBeforeRewind = GM.healthCurrent;
        SpriteRenderer playerSpriteRenderer = GM.playerController.GetComponent<SpriteRenderer>();

        while (true)
        {
            Vector3 currentPosition = GM.playerController.transform.position;
            positionHistory.Push(currentPosition);

            // Create a time marker at the current position
            if (playerSpriteRenderer != null)
            {
                // Create a new GameObject for the time marker
                GameObject marker = new GameObject("TimeMarker");
                marker.transform.position = currentPosition;

                // Add a SpriteRenderer and copy the player's current sprite
                SpriteRenderer markerSpriteRenderer = marker.AddComponent<SpriteRenderer>();
                markerSpriteRenderer.gameObject.transform.localScale = GM.playerController.transform.localScale;
                markerSpriteRenderer.gameObject.transform.rotation = GM.playerController.transform.rotation;
                markerSpriteRenderer.sprite = playerSpriteRenderer.sprite;
                markerSpriteRenderer.color = TimeTravelColor; // Optional: Make the marker semi-transparent
                markerSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1; // Ensure it appears behind the player visually

                spriteMarkers.Add(marker);
            }


            yield return new WaitForSeconds(recordInterval);
        }
    }

    private IEnumerator RewindPlayerState(GameManager GM)
    {
        GM.playerController.p.anim.SetBool("midJump", true);
        BoxCollider2D playerCollider = GM.playerController.gameObject.GetComponent<BoxCollider2D>();
        Rigidbody2D playerRigidbody = GM.playerController.GetComponent<Rigidbody2D>();
        if (playerCollider != null)
            playerCollider.enabled = false;

        if (playerRigidbody != null)
            playerRigidbody.bodyType = RigidbodyType2D.Static;

        GM.playerController.SetControls(false);
        while (positionHistory.Count > 0)
        {
            Vector3 targetPosition = positionHistory.Pop();

            float elapsedTime = 0f;
            Vector3 startPosition = GM.playerController.transform.position;

            // Lerp to the target position
            while (elapsedTime < recordInterval)
            {
                elapsedTime += Time.deltaTime * rewindSpeedMultiplier;
                GM.playerController.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / recordInterval);
                yield return null;
            }

            GameObject marker = spriteMarkers[spriteMarkers.Count - 1];
            spriteMarkers.RemoveAt(spriteMarkers.Count - 1);
            Destroy(marker);

            // Snap to the final target position for this step
            GM.playerController.transform.position = targetPosition;
        }

        GM.healthCurrent = playerHealthBeforeRewind;

        GM.playerController.p.anim.SetBool("midJump", false);
        GM.playerController.SetControls(true);
        if (playerCollider != null)
            playerCollider.enabled = true;
        if (playerRigidbody != null)
            playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}

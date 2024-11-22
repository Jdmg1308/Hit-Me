using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TimeTravel Card Data", menuName = "ScriptableObjects/Card/TimeTravel")]
public class TimeTravelCard : Card
{
    public override CardType cardType { get { return CardType.StatusEffect; } }

    // A stack to store the player's positions over time
    private Stack<Vector3> positionHistory = new Stack<Vector3>();
    private float recordInterval = 0.5f; // How often to record positions
    private Coroutine recordRoutine;

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
        while (true)
        {
            positionHistory.Push(GM.playerController.transform.position);
            yield return new WaitForSeconds(recordInterval);
        }
    }

    private IEnumerator RewindPlayerState(GameManager GM)
    {
        GM.playerController.p.anim.SetBool("midJump", true);
        GM.playerController.SetControls(false);
        while (positionHistory.Count > 0)
        {
            Vector3 targetPosition = positionHistory.Pop();
            float elapsedTime = 0f;
            Vector3 startPosition = GM.playerController.transform.position;

            // Lerp to the target position
            while (elapsedTime < recordInterval)
            {
                elapsedTime += Time.deltaTime;
                GM.playerController.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / recordInterval);
                yield return null;
            }

            // Snap to the final target position for this step
            GM.playerController.transform.position = targetPosition;
        }
        GM.playerController.p.anim.SetBool("midJump", false);
        GM.playerController.SetControls(true);
    }
}

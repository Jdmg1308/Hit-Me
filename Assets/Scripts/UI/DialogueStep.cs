using UnityEngine;

[CreateAssetMenu(fileName = "DialogueStep", menuName = "Dialogue/Dialogue Step")]
public class DialogueStep : ScriptableObject
{
    [TextArea]
    public string dialogueText;      // text line 
    public GameObject[] hints;       // array of hint GameObjects (arrows, boxes, etc.)
}

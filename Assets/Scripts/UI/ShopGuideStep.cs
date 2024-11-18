using UnityEngine;

[CreateAssetMenu(fileName = "ShopGuideStep", menuName = "Shop Guide Step")]
public class ShopGuideStep : ScriptableObject
{
    [TextArea]
    public string instructionText;            // instruction text for this step
    public GameObject[] hints;                // hint GameObjects like boxes or arrows pointing to things
    public bool requiresHover;                // if this step requires a hover action
    public bool requiresPurchase;             // if this step requires a purchase action
    public bool requiresDestroy;              // if this step requires a destroy action
}

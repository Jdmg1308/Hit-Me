using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour
{
    private Vector3 originalScale;
    public Vector3 scaleLarge = new Vector3(2f, 2f, 2f);  // Double size
    private RectTransform OGsizeRectTransform;
    private Vector3 OGsizeWorldPos;
    private RectTransform parentCanvasRect;
    private GameObject OGsize;
    private Canvas overlayCanvas;

    void Start()
    {
        // Store the original scale of the card
        originalScale = transform.localScale;

        // Get the RectTransform of the parent Canvas
        parentCanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        OGsize = transform.Find("OGsize").gameObject;

        overlayCanvas = transform.GetComponent<Canvas>();
    }

    // When the pointer enters the card
    public void OnPointerEnter(PointerEventData eventData)
    {
        IncreaseSize();
    }

    // When the pointer exits the card
    public void OnPointerExit(PointerEventData eventData)
    {
        DecreaseSize();
    }

    public void IncreaseSize()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        transform.localScale = scaleLarge;
        OGsize.transform.localScale = new Vector3(1/scaleLarge.x, 1 / scaleLarge.y, 1 / scaleLarge.z);

        //OGsizeWorldPos = OGsize.GetComponent<RectTransform>().position;
        //OGsize.GetComponent<RectTransform>().position = OGsizeWorldPos;

        overlayCanvas.sortingOrder = 2; // Ensure this is above the main canvas sorting order
    }

    public void DecreaseSize()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.localScale = originalScale;
        OGsize.transform.localScale = new Vector3(OGsize.transform.localScale.x * scaleLarge.x, OGsize.transform.localScale.y * scaleLarge.y, OGsize.transform.localScale.z * scaleLarge.z);
        overlayCanvas.sortingOrder = 1;
    }
}

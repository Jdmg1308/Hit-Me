using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour
{
    private Vector3 originalScale;
    public Vector3 scaleLarge = new Vector3(2f, 2f, 2f);  // Double size
    private RectTransform rectTransform;
    private RectTransform parentCanvasRect;
    private GameObject OGsize;
    private Canvas overlayCanvas;

    void Start()
    {
        // Store the original scale of the card
        originalScale = transform.localScale;

        // Get the RectTransform component of the card (for UI elements)
        rectTransform = GetComponent<RectTransform>();

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
        //Canvas latestCardCanvas = transform.gameObject.GetComponent<Canvas>();
        //if (latestCardCanvas == null)
        //{
        //    // If there's no Canvas attached, add one
        //    latestCardCanvas = transform.gameObject.AddComponent<Canvas>();
        //}

        //// Ensure the canvas renders on top
        //latestCardCanvas.overrideSorting = true;
        //latestCardCanvas.sortingOrder = 100;  // Set a high sorting order value
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        transform.localScale = scaleLarge;
        OGsize.transform.localScale = new Vector3(1/scaleLarge.x, 1 / scaleLarge.y, 1 / scaleLarge.z);
        //overlayCanvas.overrideSorting = true;
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

using System.Collections;
using System.Collections.Generic;
using Assets.AssetStoreUI.Scripts;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class AssetGazeInput : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable
{

    private GameObject rootObject;
    private bool gazeFocus = false;

    private bool selected = false;

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log("InputDown");

        eventData.Use();
    }

    public void OnInputUp(InputEventData eventData)
    {
        Debug.Log("InputUp");

        eventData.Use();
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Debug.Log("InputClicked");
        selected = !selected;

        eventData.Use();
    }

    public void OnFocusEnter()
    {
        if (selected) return;
        gazeFocus = true;
        Utility.outlineRecursive(gameObject.transform, true);
        Debug.Log("FocusEnter");
    }

    public void OnFocusExit()
    {
        if (selected) return;
        gazeFocus = false;
        Utility.outlineRecursive(gameObject.transform, false);
        Debug.Log("FocusExit");
    }
}

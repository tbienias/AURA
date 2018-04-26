using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SaveLoadBehavior : MonoBehaviour
{
    [Header("Set behavior at runtime")]

    [Tooltip("Is this GameObject savable?")]
    public bool IsSavable = true;

    [HideInInspector]
    public string AssetBundleName = null;
}

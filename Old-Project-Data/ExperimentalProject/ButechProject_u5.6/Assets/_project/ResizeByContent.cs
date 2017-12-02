using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResizeByContent : MonoBehaviour {

    RectTransform rt;
    Text txt;

    public float Margin = 72;

    // Use this for initialization
    void Start () {
        rt = GetComponent<RectTransform>();
        txt = GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        
        rt.sizeDelta = new Vector2(txt.preferredWidth + 2*Margin, txt.preferredHeight + 2*Margin);
    }
}

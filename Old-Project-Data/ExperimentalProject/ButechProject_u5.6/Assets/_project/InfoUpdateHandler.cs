using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent (typeof (Text)) ]
public class InfoUpdateHandler : MonoBehaviour {

    private Text info;
    private PositionControler poscon;
    private VerticalRepositioningHandler sot;
    // Use this for initialization
    void Start()
    {
        info = this.GetComponent<Text>();
        poscon = this.GetComponentInParent<PositionControler>();
        sot = this.GetComponentInParent<VerticalRepositioningHandler>();
        if (poscon == null || sot == null)
        {
            throw new System.Exception("InfoUpdateHandler.cs one or more required Component not found");
           
        }
    }
	// Update is called once per frame
	void Update () {
        if (poscon != null && sot != null)
        {
            var name = sot.StageObjectName;
            var pos = poscon.CurrentPosition;
            var speed = poscon.IsMoving ? poscon.MovementSpeed : 0f;
            
            info.text = string.Format("<b>Name:</b> {0:s}\n<b>Position:</b> {1:s}", name, pos);
            info.text += string.Format("\n<b>Geschwindigkeit:</b> {0,2:f} m/s", speed);

        }
        
	}
}

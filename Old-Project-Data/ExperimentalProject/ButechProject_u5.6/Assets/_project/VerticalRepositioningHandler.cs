using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PositionControler))]
public class VerticalRepositioningHandler :   MonoBehaviour {

    public string StageObjectName { get { return string.Format("Laststange {0:d}", engineID); } }
    public uint engineID;
    private PositionControler poscon;


    public void MoveTo(float position)
    {
        poscon.SetTargetPosition(y: position);
    }

    public void Move(float delta)
    {
        poscon.ModifyTargetPosition(dy: delta);
    }

    // Use this for initialization
    void Start () {
        poscon = this.GetComponent<PositionControler>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

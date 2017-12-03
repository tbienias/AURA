using HoloToolkit.Unity;
using kadmium_sacn_core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageEventManager : Singleton<StageEventManager> {
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}


    public void OnMessageReceived(SACNPacket obj)
    {
        var p = obj.Data.ToInt16Array();
        foreach(var tmp in GetComponentsInChildren<VerticalRepositioningHandler>())
        {
            try {
                // get desired position (in mm) from packet,
                // convert to m and apply              
                var pos = p[tmp.engineID - 1] / 1000f;
                tmp.MoveTo(pos);
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogException(ex);
            }
        }
           
    }


}

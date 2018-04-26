using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class XBOXCustomHandler : XboxControllerHandlerBase
{
    private Canvas uiCanvas;
    private GameObject debugWindow;

    private float deleteTimer;
    private const float maxTime = 3;
    private bool dPadPressed = false;

    private bool showInfo = false;

    void Start()
    {
        //base.Start();
        uiCanvas = GameObject.Find("UiCanvas").GetComponent<Canvas>();
        debugWindow = GameObject.Find("Console");
    }
    public override void OnXboxInputUpdate(XboxControllerEventData eventData)
    {
        GazeInputHandler gih = GazeInputHandler.Instance;

        /////////////////////
        // SELECTTION MODE //
        /////////////////////

        if (gih.isObjectSelected())
        {
            Vector3 v3 = new Vector3(eventData.XboxLeftStickHorizontalAxis, eventData.XboxSharedTriggerAxis,
                eventData.XboxLeftStickVerticalAxis);
            gih.manipulateObject(v3);
        }

        ///////////////////
        // Front Buttons //
        ///////////////////

        // select / grab object
        if (eventData.XboxA_Down)
        {
            if (!gih.isObjectSelected())
            {
                gih.selectObject();
            }
            else
            {
                gih.grabObject();
            }
        }

        if (gih.isObjectSelected())
        {
            // delete timer
            if (eventData.XboxB_Down)
            {
                deleteTimer = Time.time;
            }

            if (eventData.XboxB_Pressed)
            {
                if (Time.time - deleteTimer > maxTime)
                {
                    gih.deleteObject();
                }
                else
                {
                    gih.updateDeleteProgress((Time.time - deleteTimer) / maxTime);
                }
            }

            // drop object / delete object
            if (eventData.XboxB_Up)
            {
                gih.dropObject();
                deleteTimer = float.PositiveInfinity;
            }
        }

        // addiotional info
        if (eventData.XboxY_Down)
        {
            gih.applyTexture();
        }

        // Open menu
        if (eventData.XboxX_Down)
        {
            gih.repositionCanvas(uiCanvas);
        }

        ///////////////
        // Triggers //
        //////////////

        if (eventData.XboxSharedTriggerAxis != 0)
        {
            
        }
        
        // Move Object further away or closer
//        if (eventData.XboxRightTriggerAxis != 0.0)
//        {
//            gih.pushbackObj(eventData.XboxRightTriggerAxis);
//        }
//
//        if (eventData.XboxLeftTriggerAxis != 0.0)
//        {
//
//            gih.pullbackObj(eventData.XboxLeftTriggerAxis);
//        }

        ////////////////
        // Left Stick //
        ////////////////

        //ROTATE STUFF
        if (eventData.XboxLeftStickVerticalAxis != 0)
        {
            gih.rotateX(eventData.XboxLeftStickVerticalAxis);
        }
        else
        {
            gih.rotateX(0f);
        }
        if (eventData.XboxLeftStickHorizontalAxis != 0)
        {
            gih.rotateY(eventData.XboxLeftStickHorizontalAxis * -1);
        }
        else
        {
            gih.rotateY(0f);
        }

        /////////////////
        // Right Stick //
        /////////////////

        if (eventData.XboxRightStickVerticalAxis != 0)
        {
            
        }
        else
        {
            
        }
        if (eventData.XboxRightStickHorizontalAxis != 0)
        {
            
        }
        else
        {
            
        }

        //////////////
        // DPAD HACK//
        //////////////
#if !UNITY_EDITOR   
        // right
        if (eventData.XboxRightTriggerAxis == 0 && !dPadPressed )
        {
            if (gih.isObjectSelected())
            {
                Debug.Log("DPAD right");
                showInfo = !showInfo;
                GameObject.Find("ObjectUi").transform.Find("InfoView").gameObject.SetActive(showInfo);
                
                dPadPressed = true;
            }
        }

        // left
        else if (eventData.XboxRightTriggerAxis == 1 && !dPadPressed)
        {
            Debug.Log("DPAD left");
            dPadPressed = true;
            QRCodeScript qrscript = GameObject.Find("QRZXingObject").GetComponent<QRCodeScript>();
            qrscript.OnScan();
            Debug.Log("Scan started");
        }

        // up
        else if (eventData.XboxRightTriggerAxis < 0 && eventData.XboxRightTriggerAxis > -1 && !dPadPressed)
        {
            Debug.Log("DPAD up");
            dPadPressed = true;
        }

        // down
        else if (eventData.XboxRightTriggerAxis > 0 && eventData.XboxRightTriggerAxis < 1 && !dPadPressed)
        {
            Debug.Log("DPAD down");
            dPadPressed = true;
            gih.applyTexture();
        }
        else
        {
            dPadPressed = false;

        }

#endif

        /////////////
        // Bumpers //
        /////////////
        if (eventData.XboxLeftBumper_Down)
        {
            gih.changeSelectMode(-1);
        }

        if (eventData.XboxRightBumper_Down)
        {
            gih.changeSelectMode(1);
        }


        /* OLD Settings
        if (eventData.XboxLeftBumper_Pressed)
        {
            gih.rotateZ(1f);
        }

        if (eventData.XboxRightBumper_Pressed)
        {
            gih.rotateZ(-1f);
        }

        if (eventData.XboxLeftBumper_Pressed == false && eventData.XboxRightBumper_Pressed == false)
        {
            gih.rotateZ(0f);
        }
        */
        ////////////////////
        // Start / Select //
        ///////////////////

        if (eventData.XboxMenu_Down)
        {
          
        }

        if (eventData.XboxView_Down)
        {
            Debug.Log("Sel");
            gih.repositionDebugWindow(debugWindow);
        }

    }
}

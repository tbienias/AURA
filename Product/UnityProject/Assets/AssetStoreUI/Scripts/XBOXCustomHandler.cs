using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class XBOXCustomHandler : XboxControllerHandlerBase
{

    public override void OnXboxAxisUpdate(XboxControllerEventData eventData)
    {
        GazeInput GI = GazeInput.Instance;

        if (eventData.XboxA_Down)
        {
            GI.toggleGrab();
        }

        if (eventData.XboxRightTriggerAxis != 0.0)
        {
            GI.pushbackObj(eventData.XboxRightTriggerAxis);
        }
        if (eventData.XboxLeftTriggerAxis != 0.0)
        {

            GI.pullbackObj(eventData.XboxLeftTriggerAxis);
        }


        //ROTATE STUFF
        if (eventData.XboxLeftStickVerticalAxis != 0)
        {
            GI.rotateX(eventData.XboxLeftStickVerticalAxis);
        }
        else
        {
            GI.rotateX(0f);
        }
        if (eventData.XboxLeftStickHorizontalAxis != 0)
        {
            GI.rotateY(eventData.XboxLeftStickHorizontalAxis * -1);
        }
        else
        {
            GI.rotateY(0f);
        }

        if (eventData.XboxLeftBumper_Pressed)
        {
            GI.rotateZ(1f);
        }

        if (eventData.XboxRightBumper_Pressed)
        {
            GI.rotateZ(-1f);
        }
        if (eventData.XboxLeftBumper_Pressed == false && eventData.XboxRightBumper_Pressed == false)
        {
            GI.rotateZ(0f);
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GazeInput
{
    private static GazeInput instance;

    private GazeInput() { }

    public static GazeInput Instance
    {
        get { return instance ?? (instance = new GazeInput()); }
    }

    private Camera camera;
    private GameObject objectToMove = null;

    private bool dragging = false;
    private float rayCastMaxDistance = 1000;
    private float objectDistance = 2;
    private float objectDistancemod = 0.0001f;
    private float canvasDistance = 2;


    public void grabObject()
    {
         if (!dragging)
        {
            RaycastHit hit;
            Debug.Log("GRAB DETECTED" + dragging);

            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
            {
                

                objectToMove = hit.transform.gameObject;
                Debug.Log("Grabbing Obj" + objectToMove);
                dragging = true;
            }
            else
            {
                Debug.Log("No GameObj found");
            }

        }
    }

    public void moveObject()
    {
        if (dragging && objectToMove != null)
        {
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            Vector3 rayPoint = ray.GetPoint(objectDistance);

            objectToMove.transform.position = rayPoint;
        }
    }

    public void dropObject()
    {
        Debug.Log("Dropping object: " + objectToMove);
        dragging = false;
        objectToMove = null;
    }

    public void repositionCanvas(Canvas canvas)
    {
        canvas.transform.position = (camera.transform.position + Camera.main.transform.forward * canvasDistance);
    }

    public void canvasRotation(Canvas canvas)
    {
        canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - camera.transform.position);
    }

    public void debugWindowRotation(GameObject debugWindow)
    {
        debugWindow.transform.rotation = Quaternion.LookRotation(debugWindow.transform.position - camera.transform.position);
    }

    public void repositionDebugWindow(GameObject debugWindow)
    {
        debugWindow.transform.position = (camera.transform.position + Camera.main.transform.forward * canvasDistance);
    }

    public void setCamera(Camera cam)
    {
        camera = cam;
    }
    public void dragPlease(GameObject obj)
    {
        if (!dragging)
        {

            objectToMove = obj;
            dragging = true;
           
        }
    }
    public void toggleGrab()
    {
        if (!dragging)
        {

            grabObject();

        }
        else
        {
            dropObject();
        }
    }

    public void pushbackObj(float rate)
    {
        objectDistance = objectDistance + ((float)Math.Pow(rate+1, 10)) * objectDistancemod;
    }
    public void pullbackObj(float rate)
    {
        objectDistance = objectDistance - ((float)Math.Pow(rate+1, 10)) * objectDistancemod;
    }
}

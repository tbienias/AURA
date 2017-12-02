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

    private float rotatex = 0.0f;
    private float rotatey = 0.0f;
    private float rotatez = 0.0f;

    private float rotatemod = 1f;

    public void grabObject()
    {
         if (!dragging)
        {
            RaycastHit hit;
            Debug.Log("GRAB DETECTED" + dragging);

            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
            {

                objectDistance = hit.distance;
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

            objectToMove.transform.Rotate(new Vector3(rotatex, rotatey, rotatez), Space.World);
            
        }
    }

    public void dropObject()
    {
        Debug.Log("Dropping object: " + objectToMove);
        dragging = false;
        objectToMove = null;
        objectDistance = 2;
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
        float newdis = objectDistance + ((float)Math.Pow(rate + 1, 10)) * objectDistancemod;
        if (newdis >= 10)
        {
            objectDistance = 10;
        }
        else
        {
            objectDistance = newdis;
        }
    }
    public void pullbackObj(float rate)
    {
      
        objectDistance = objectDistance - ((float)Math.Pow(rate + 1, 10)) * objectDistancemod;
        if (objectDistance < 0.5f)
        {
            objectDistance = 0.5f;
        }
    }

    public void rotateX(float rate)
    {
        rotatex = rate * rotatemod;
        
    }
    public void rotateY(float rate)
    {
        rotatey = rate * rotatemod;
       
    }
    public void rotateZ(float rate)
    {
        rotatez = rate * rotatemod;

    }
}

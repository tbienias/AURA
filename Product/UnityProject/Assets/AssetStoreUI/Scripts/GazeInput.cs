using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float canvasDistance = 2;

    public void grabObject()
    {
        if (!dragging)
        {
            RaycastHit hit;

            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
            {
                Debug.Log("Grabbing Obj" + objectToMove);

                objectToMove = hit.transform.gameObject;
                dragging = true;
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

}

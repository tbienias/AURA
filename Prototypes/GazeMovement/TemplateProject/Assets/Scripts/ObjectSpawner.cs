using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    private static float distance = 2;

    public static void spawnCube()
    {
        GameObject objToSpawn = GameObject.Find("Cube");
        GameObject cam = GameObject.Find("MixedRealityCamera");

        Instantiate(objToSpawn, (cam.transform.position + Camera.main.transform.forward * distance), cam.transform.rotation);
        Debug.Log("Spawned cube at: " + cam.transform.position);
    }

    public void testCube()
    {
        GameObject objToSpawn = GameObject.Find("Cube");
        GameObject cam = GameObject.Find("MixedRealityCamera");
        Instantiate(objToSpawn, (cam.transform.position), cam.transform.rotation);
        Debug.Log("Spawned cube at: " + cam.transform.position);
    }
}

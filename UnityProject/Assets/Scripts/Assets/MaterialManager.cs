using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialManager {

    private Camera camera;
    private static MaterialManager instance;
    private GameObject objectHit = null;

    private float rayCastMaxDistance = 1000;
    private float objectDistance = 2;

    // Use this for initialization
    private MaterialManager() {
        camera = GameObject.Find("MixedRealityCamera").GetComponent<Camera>();
    }

    public static MaterialManager Instance
    {
        get { return instance ?? (instance = new MaterialManager()); }
    }

    public void AddMaterial(GameObject objectToTexture, String resource) { 
    
        Debug.Log("Objecr to texture: " + objectToTexture);
        // Get the Renderer and let him render a new material
        Renderer renderer = objectToTexture.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material newMaterial = Resources.Load(resource) as Material;
            if (newMaterial != null)
            {
                renderer.material = newMaterial;
            } else
            {
                Debug.Log("Nothing textured!");
            }                 
        }
        
       
        
    }
   
}

using System;
using UnityEngine;

namespace Assets.AssetStoreUI.Scripts
{
    public class Utility : MonoBehaviour {

        public static void boxColliderRecursive(Transform root, bool add)
        {
            BoxCollider bc = root.GetComponent<BoxCollider>();
            if (add)
            {
                if (bc == null)
                {
                    root.gameObject.AddComponent<BoxCollider>();
                
                }
            }
            else
            {
                if (bc != null)
                {
                    Destroy(bc);
                }
            }

            foreach (Transform child in root)
            {
                boxColliderRecursive(child, add);
            }
        }

        public static void meshColliderRecursive(Transform root, bool add)
        {
            MeshCollider mc = root.GetComponent<MeshCollider>();
            if (add)
            {
                if (mc == null)
                {
                    root.gameObject.AddComponent<MeshCollider>();
                }
            }
            else
            {
                if (mc != null)
                {
                    Destroy(mc);
                }
            }

            foreach (Transform child in root)
            {
                meshColliderRecursive(child, add);
            }
        }

        public static void outlineRecursive(Transform root, bool add)
        {
            cakeslice.Outline outline = root.gameObject.GetComponent<cakeslice.Outline>();
            if (add)
            {
                if (outline == null)
                {
                    root.gameObject.AddComponent<cakeslice.Outline>().color = 0;
                }
            }
            else
            {
                if (root.gameObject.GetComponent<cakeslice.Outline>() != null)
                {
                    Destroy(outline);
                }
            }

            foreach (Transform child in root)
            {
                outlineRecursive(child, add);
            }
        }

        public static void meshRendererRecursive(Transform root, bool add)
        {
            MeshRenderer mr = root.gameObject.GetComponent<MeshRenderer>();
            if (add)
            {
                if (mr == null)
                {
                    root.gameObject.AddComponent<MeshRenderer>();
                }
            }
            else
            {
                if (root.gameObject.GetComponent<cakeslice.Outline>() != null)
                {
                    Destroy(mr);
                }
            }

            foreach (Transform child in root)
            {
                meshRendererRecursive(child, add);
            }
        }

        public static GameObject getRootObject(GameObject child)
        {
            GameObject result = child;
            while (result.transform.parent != null)
            {
                result = result.transform.parent.gameObject;
            }

            //Debug.Log("traversing");

            return result;
        }
    }
}

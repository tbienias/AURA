using System.Collections;
using UnityEngine;

public class AssetManager : MonoBehaviour
{

    void Start()
    {
        string url = "https://code.dedyn.io/AssetBundles/crate";
        WWW www = new WWW(url);
        StartCoroutine(WaitForReq(www));
    }

    IEnumerator WaitForReq(WWW www)
    {
        yield return www;
        AssetBundle bundle = www.assetBundle;
        if (www.error == null)
        {
            GameObject barrel = (GameObject)bundle.LoadAsset("Crate_Sealed_01");
            Vector3 pos = new Vector3(0, 0, 2);
            Quaternion rot = new Quaternion(45.0f, 45.0f, 45.0f, 0.0f);
            Instantiate(barrel, pos, rot);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}

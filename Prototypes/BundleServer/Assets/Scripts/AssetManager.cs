using System.Collections;
using UnityEngine;

[System.Serializable]
public class AssetBundleListing
{
    public AssetBundleItem[] AssetBundleList;    
}

[System.Serializable]
public class AssetBundleItem
{
    public string BundleName;
    public string[] Assets;
}

public class AssetManager : MonoBehaviour
{
    private AssetBundleListing mAvailableBundles;

    void Start()
    {
        AssetBundleItem[] items = new AssetBundleItem[2];
        items[0] = new AssetBundleItem() { BundleName = "Test1", Assets = new string[2] };
        items[1] = new AssetBundleItem() { BundleName = "Test2" };

        string yay = JsonUtility.ToJson(items[0]);

        // Get all possible AssetBundles from the Server
        WWW www = new WWW("127.0.0.1");
        this.StartCoroutine(this.WaitForAssetList(www));

        www = new WWW("127.0.0.1/barrel");
        this.StartCoroutine(this.WaitForReq(www));
    }

    IEnumerator WaitForAssetList(WWW src)
    {
        yield return src;

        this.mAvailableBundles = JsonUtility.FromJson<AssetBundleListing>(src.text);

        foreach ( AssetBundleItem item in this.mAvailableBundles.AssetBundleList )
        {
            string bundleInfo = item.BundleName + " contains Assets:\n";
            foreach (string assetName in item.Assets)
            {
                bundleInfo += "\t" + assetName + "\n";
            }
            Debug.Log(bundleInfo);
        }
    }

    IEnumerator WaitForReq(WWW www)
    {
        yield return www;
        AssetBundle bundle = www.assetBundle;

        foreach (string assetName in bundle.GetAllAssetNames())
            Debug.Log(assetName);

        if (www.error == null)
        {
            GameObject barrel = (GameObject)bundle.LoadAsset("Barrel_Sealed_01");

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

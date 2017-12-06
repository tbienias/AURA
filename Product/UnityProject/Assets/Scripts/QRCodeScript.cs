using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QRCodeScript : MonoBehaviour
{
    public Transform textMeshObject;

    private void Start()
    {
        this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
        this.OnReset();
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(6);
        this.textMesh.text = "";
    }

    public void OnScan()
    {
        this.textMesh.text = "scanning for 30s";
        StartCoroutine(WaitCoroutine());

#if !UNITY_EDITOR
    MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
        result =>
        {
          UnityEngine.WSA.Application.InvokeOnAppThread(() =>
          {
            this.textMesh.text = result ?? "not found";
          }, 
          false);
        },
        TimeSpan.FromSeconds(30));
#endif
    }
    
    public void OnReset()
    {
        this.textMesh.text = "say scan to start";
        StartCoroutine(WaitCoroutine());
    }
    TextMesh textMesh;
}
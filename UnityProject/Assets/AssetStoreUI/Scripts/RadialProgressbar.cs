using UnityEngine;
using UnityEngine.UI;


public class RadialProgressbar : MonoBehaviour
{
    private Image _image;

    public float Progress
    {
        set { _image.fillAmount = Mathf.Clamp01(value); }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
}

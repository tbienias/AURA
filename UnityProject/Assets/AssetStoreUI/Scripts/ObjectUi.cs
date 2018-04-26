using UnityEngine;
using UnityEngine.UI;


public class ObjectUi : MonoBehaviour
{
    [SerializeField]
    private RadialProgressbar _radialProgressbar;

    [SerializeField]
    private GameObject infoView;

    private GameObject _target;
    private Camera _camera;
    private Text infoViewText;

    public RadialProgressbar Progressbar
    {
        get { return _radialProgressbar; }
    }

    public void updateInfo(string text)
    {
        infoViewText.text = text;
    }

    public void Activate(GameObject target)
    {
        Progressbar.Progress = 0f;
        _target = target;
        _radialProgressbar.gameObject.SetActive(true);

//        for (int i = 0; i < transform.childCount; i++)
//        {
//            transform.GetChild(i).gameObject.SetActive(true);
//        }
    }

    public void Deactivate()
    {
        _target = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _camera = Camera.main;
        infoViewText = infoView.GetComponent<Text>();

        Deactivate();
    }

    public void LateUpdate()
    {
        if (_target != null)
        {
            if (transform.position - _camera.transform.position != Vector3.zero)
            {
                transform.position = _target.transform.position;
                //transform.LookAt(_camera.transform.position - transform.position);
                transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
            }
        }
    }
}

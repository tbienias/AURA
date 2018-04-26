using System;
using Assets.AssetStoreUI.Scripts;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.SpatialMapping;


[DisallowMultipleComponent]

public class GazeInputHandler : MonoBehaviour
{
    private static GazeInputHandler _instance;
    
    
    private Canvas uiCanvas;
    private Canvas selCanvas;
    private Canvas objCanvas;

    private GameObject debugWindow;

    private Camera camera;
    private GameObject objectToMove = null;
    private ObjectUi _objectUi;
    private MaterialManager materialManager;

    private bool dragging = false;
    private float rayCastMaxDistance = 1000;
    private float objectDistance = 2;
    private float objectDistancemod = 0.0001f;
    private float canvasDistance = 2;

    private float rotatex = 0.0f;
    private float rotatey = 0.0f;
    private float rotatez = 0.0f;

    private float rotatemod = 2.5f;


    // SELECT MODE VARIABLES
    // Select MODES : 0=DotMode, 1=Translate, 2=Rotate, 3=Scale, 4=Texture
    private int selectMode = 0;

    enum Mode
    {
        DOTMODE,
        TRANSLATE,
        ROTATE,
        SCALE,
        TEXTURE1,
        TEXTURE2
    };

    //private Mode selectMode;

    public static GazeInputHandler Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            GazeInputHandler spawner = FindObjectOfType<GazeInputHandler>() ??
                                    new GameObject(typeof(GazeInputHandler) + "(Singleton)").AddComponent<GazeInputHandler>();
            return _instance ?? (_instance = spawner);
        }
    }

    private GameObject ObjectToMove
    {
        get { return objectToMove; }
        set
        {
            objectToMove = value;
            if (value != null)
            {
                _objectUi.Activate(value);
            }
            else
            {
                _objectUi.Deactivate();
            }
        }
    }

    public bool isObjectSelected()
    {
        return ObjectToMove != null;
    }

    public void selectObject()
    {
        RaycastHit hit;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
        {
            // get raycast object
            objectDistance = hit.distance;
            ObjectToMove = hit.transform.gameObject;

            // get root object
            objectToMove = Utility.getRootObject(objectToMove);

            // outline
            Utility.outlineRecursive(objectToMove.transform, true);

            selCanvas.gameObject.SetActive(true);
            selectMode = 0;

            // show infoView
            string text = "InfoView:\n\n";
            text += "Position: " + objectToMove.transform.position;
            _objectUi.updateInfo(text);
        }
        else
        {
            Debug.Log("No GameObj found");
        }
    }

    public void grabObject(GameObject obj)
    {
        if (!dragging)
        {
            ObjectToMove = obj;
            dragging = true;
        }
    }

    public void grabObject()
    {
        if (!dragging)
        {
            dragging = true;

        }
    }


    public void grabObjectDirectly()
    {
        selectObject();
        grabObject();
    }

    public void moveObject()
    {
       
        if (dragging && ObjectToMove != null)
        {
          
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);
            Vector3 rayPoint = ray.GetPoint(objectDistance);

            ObjectToMove.transform.position = rayPoint;
            
            ObjectToMove.transform.Rotate(new Vector3(rotatex, rotatey, rotatez), Space.World);

        }
    }

    public void dropObject()
    {
        // First, check if a object is selected
        if (ObjectToMove != null)
        {
            Debug.Log("Dropping object: " + ObjectToMove);

            // remove outline
            Utility.outlineRecursive(objectToMove.transform, false);

            dragging = false;
            ObjectToMove = null;

            selCanvas.gameObject.SetActive(false);
            changeSelectMode(0);
            selectMode = 0;

        }
    }

    public void updateDeleteProgress(float progress)
    {
        _objectUi.Progressbar.Progress = progress;
    }

    public void deleteObject()
    {
        if (ObjectToMove != null)
        {
            Debug.Log("Deleting object: " + ObjectToMove);
            Utility.outlineRecursive(objectToMove.transform, false);

            Destroy(ObjectToMove);
            ObjectToMove = null;
            dragging = false;
        }
    }

    public void repositionCanvas(Canvas canvas)
    {
        RaycastHit hit;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        if (Physics.Raycast(ray, out hit, rayCastMaxDistance))
        {
            if (uiCanvas.name == hit.transform.gameObject.name)
            {
                Debug.Log("Hiding UI: " + uiCanvas.name);
                uiCanvas.gameObject.SetActive(false);
            }
        }
        else
        {
            uiCanvas.gameObject.SetActive(true);
            canvas.transform.position = (camera.transform.position + Camera.main.transform.forward * canvasDistance);
        }

    }

    public void repositionDebugWindow(GameObject debugWindow)
    {
        debugWindow.transform.position = (camera.transform.position + Camera.main.transform.forward * canvasDistance);
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

    // Change Select Mode
    public void changeSelectMode(int direction)
    {
        // Set Picture to not Selected

        Image image = null;
        switch (selectMode)
        {
            case 0:
                image = selCanvas.transform.Find("DotImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_light");
                Debug.Log(image.sprite);
                break;
            case 1:
                image = selCanvas.transform.Find("TranslateImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Translate_light");
                Debug.Log(image.sprite);
                break;
            case 2:
                image = selCanvas.transform.Find("RotateImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Rotate_light");
                Debug.Log(image.sprite);
                break;
            case 3:
                image = selCanvas.transform.Find("ScaleImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Scale_light");
                Debug.Log(image.sprite);
                break;
            case 4:
                image = selCanvas.transform.Find("TextureImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_light");
                Debug.Log(image.sprite);
                break;
            case 5:
                image = selCanvas.transform.Find("TextureImage2").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_light");
                Debug.Log(image.sprite);
                break;
            default:
                Debug.Log("Switch Case out of Bounds");
                break;
        }


        selectMode = (selectMode + direction + 6) % 6;

        image = null;
        switch (selectMode)
        {
            case 0:
                image = selCanvas.transform.Find("DotImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_sel");
                Debug.Log(image.sprite);
                break;
            case 1:
                image = selCanvas.transform.Find("TranslateImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Translate_sel");
                Debug.Log(image.sprite);
                break;
            case 2:
                image = selCanvas.transform.Find("RotateImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Rotate_sel");
                Debug.Log(image.sprite);
                break;
            case 3:
                image = selCanvas.transform.Find("ScaleImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Scale_sel");
                Debug.Log(image.sprite);
                break;
            case 4:
                image = selCanvas.transform.Find("TextureImage").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_sel");
                Debug.Log(image.sprite);
                break;
            case 5:
                image = selCanvas.transform.Find("TextureImage2").GetComponent<Image>();
                image.sprite = Resources.Load<Sprite>("UIPngs/Nothing_sel");
                Debug.Log(image.sprite);
                break;
            default:
                Debug.Log("Switch Case out of Bounds");
                break;
        }
    }

    public void manipulateObject(Vector3 mov)
    {
        switch (selectMode)
        {
            case 0:
                break;
            case 1:
                Vector3 v3 = new Vector3(mov.x * 0.01f, mov.y * 0.01f, mov.z * 0.01f);
                objectToMove.transform.position = objectToMove.transform.position + v3;
                break;
            case 2:
                objectToMove.transform.Rotate(mov, Space.World);
                break;
            case 3:
                Vector3 v3scale = new Vector3(mov.x * 0.1f, mov.y * 0.1f, mov.z * 0.1f);
                objectToMove.transform.localScale = objectToMove.transform.localScale + v3scale;
                break;
            case 4:
                // Texture mode
                break;
            case 5:
                // Texture mode
                break;
            default:
                Debug.Log("select Mode out of Bounds");
                break;

        }

    }

    public void applyTexture()
    {

        switch (selectMode)
        {
            case 4:
                materialManager.AddMaterial(objectToMove, "Wood");
                break;
            case 5:
                materialManager.AddMaterial(objectToMove, "Metal");
                break;
            default:
                Debug.Log("No texture selected");
                break;
        }
    }

    // Use this for initialization
    void Start()
    {
        uiCanvas = GameObject.Find("UiCanvas").GetComponent<Canvas>();

        objCanvas = GameObject.Find("ObjectUi").GetComponent<Canvas>();
        GameObject.Find("ObjectUi").transform.Find("InfoView").gameObject.SetActive(false);

        selCanvas = GameObject.Find("ObjectUi").transform.Find("SelectCanvas").GetComponent<Canvas>();
        selCanvas.gameObject.SetActive(false);
        _objectUi = objCanvas.GetComponent<ObjectUi>();

        debugWindow = GameObject.Find("Console");
        camera = Camera.main;

        materialManager = MaterialManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        moveObject();
    }
}

using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using System.Collections.Generic;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity;

namespace Buetech
{

    public class StageManager : Singleton<StageManager>
    {

        [Tooltip("Supply a friendly name for the anchor as the key name for the WorldAnchorStore.")]
        public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";

        /// <summary>
        /// Manages persisted anchors.
        /// </summary>
        private WorldAnchorManager anchorManager;
        private TextToSpeechManager textToSpeechManager;

        private GameObject vuforiaTarget;
        private GameObject vuforiaCamera;
        private GameObject introPanel;

        // Use this for initialization
        void Start()
        {
            vuforiaTarget = GameObject.FindGameObjectWithTag("VuforiaTarget");
            if (vuforiaTarget == null)
            {
                Debug.LogError("This script expects that you have a VuforiaTarget component in your scene.");
            }

            vuforiaCamera = GameObject.FindGameObjectWithTag("VuforiaCamera");
            if (vuforiaCamera == null)
            {
                Debug.LogError("This script expects that you have a VuforiaCamera component in your scene.");
            }

            textToSpeechManager = GameObject.Find("AudioManager").GetComponent<TextToSpeechManager>(); 
            if (textToSpeechManager == null)
            {
                Debug.LogError("This script expects that you have a TextToSpeechManager component in your scene.");
            }

            introPanel = GameObject.Find("IntroPanel");
            if (introPanel == null)
            {
                Debug.LogError("This script expects that you have a IntroPanel in your scene.");
            }


            //GameObject.Find("AudioManager").GetComponent<TextToSpeechManager>();

            // Make sure we have all the components in the scene we need.
            anchorManager = WorldAnchorManager.Instance;
            if (anchorManager == null)
            {
                Debug.LogError("This script expects that you have a WorldAnchorManager component in your scene.");
            }
        }


        // Update is called once per frame
        void Update()
        {

        }

        public void OnTargetFound()
        {
            // apply transformation
            transform.position = vuforiaTarget.transform.position;
            transform.rotation = vuforiaTarget.transform.rotation;

            // store anchor
            anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);

            // provide feedback
            Debug.Log("Marker recognized. Calibration concluded.");
            textToSpeechManager.SpeakText("Marker recognized");


            StopCalibration();
        }

        public void StartCalibration()
        {
            // clear anchor
            anchorManager.RemoveAnchor(gameObject);

            // activate Vuforia tracking
            //vuforiaTarget.SetActive(true);
            vuforiaTarget.GetComponent<Vuforia.TrackableEventHandler>().enabled = true;
            vuforiaTarget.GetComponent<Vuforia.ImageTargetBehaviour>().enabled = true;
        }

        public void StopCalibration()
        {
            // disable Vuforia tracking
            //vuforiaTarget.SetActive(false);
            vuforiaTarget.GetComponent<Vuforia.TrackableEventHandler>().enabled = false;
            vuforiaTarget.GetComponent<Vuforia.ImageTargetBehaviour>().enabled = false;

            // hide the introductory instructions 
            //  those are only needed when the application starts up,
            //  thus there is no corresponding activation in StartCallibration
            introPanel.SetActive(false);
        }
    }
}
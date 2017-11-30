using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Hello.Scripts
{
	public class CustomUiCaster : MonoBehaviour, IInputClickHandler
	{
	    private bool clicked = false;
        private GraphicRaycaster _graphicRaycaster;

        public bool Active { get; set; }

		private void Awake()
		{
			_graphicRaycaster = GetComponent<GraphicRaycaster>();
		    Active = true;
		}

		private void Start()
		{
			InputManager.Instance.AddGlobalListener(gameObject);
		}

		public void OnInputClicked(InputClickedEventData eventData)
		{
		    clicked = !clicked;
		    if (clicked)
		    {
		        //Debug.Log(eventData.TapCount + " / " + eventData.PressType + " / " + eventData.SourceId + " / " + eventData.used);
		        PointerEventData pointerEventData = new PointerEventData(null) { position = new Vector3(Screen.width / 2f, Screen.height / 2f) };
		        List<RaycastResult> results = new List<RaycastResult>();
		        _graphicRaycaster.Raycast(pointerEventData, results);
		        if (results.Count > 0)
		        {
		            Button button = results[0].gameObject.GetComponent<Button>();
		            if (button != null)
		            {
		                button.onClick.Invoke();
		            }
		        }
            }
		}
    }
}

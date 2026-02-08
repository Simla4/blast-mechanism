using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.UI.Settings
{
	public class SettingsButtonControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		#region Variables
		[SerializeField] RectTransform settingsIcon;
		bool opened = false;
		float moveToY = 0;
		private float startY;

		#endregion
		#region Properties 
		RectTransform rect;
		RectTransform RectTransform { get { return (rect == null) ? rect = settingsIcon.GetComponent<RectTransform>() : rect; } }
		#endregion
		#region MonoBehaviour Methods
		void Start()
		{
			startY = RectTransform.anchoredPosition.y;
		}
		#endregion
		#region My Methods
		public void SettingsButton()
		{
			if (opened)
			{
				opened = false;

			}
			else
			{
				opened = true;

			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			RectTransform.anchoredPosition = new Vector3(0f, moveToY, 0);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			RectTransform.anchoredPosition = new Vector3(0, startY, 0);
		}
		#endregion
	}
}
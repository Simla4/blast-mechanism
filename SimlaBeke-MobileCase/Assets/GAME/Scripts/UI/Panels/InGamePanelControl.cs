using UnityEngine;
namespace Base.UI
{
    public class InGamePanelControl : MonoBehaviour
    {
       
        EasyPanel uiPanel;
        EasyPanel UiPanel { get { return (uiPanel == null) ? uiPanel = GetComponent<EasyPanel>() : uiPanel; } }
       
        private void OnEnable()
        {
            UiPanel.HidePanel();
        }
    }
}
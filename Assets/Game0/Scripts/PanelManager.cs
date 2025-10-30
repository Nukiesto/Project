using System;
using Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game0
{
    public class PanelManager : MonoBehaviour
    {
        [Serializable]
        public class Panel
        {
            public string name;
            public AnimationTrigger enterButtonTrigger;
            public Button enterButton;
            public Button contentButton;
            public GameObject panel;
            public bool isDisabled;
            public int panelCopy;
        }

        [SerializeField] private Panel[] panels;
        [SerializeField] private GameObject enterPanel;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private CanvasAnimationController contentAnimationController;
        [SerializeField] private TextMeshProUGUI titleText;
        
        private void Awake()
        {
            foreach (var panel in panels)
            {
                panel.contentButton.onClick.AddListener(() =>
                {
                    EnterPanel(panel);
                });
                panel.enterButton.onClick.AddListener(() =>
                {
                    contentAnimationController.ShowCanvas();
                    EnterPanel(panel);
                });
                panel.enterButtonTrigger.onEndAnimation.AddListener(() =>
                {
                    enterPanel.gameObject.SetActive(false);
                });
            }
        }

        private void EnterPanel(Panel panel)
        {
            if (panel.isDisabled)
                panel = panels[panel.panelCopy];
            titleText.text = panel.name;
            
            foreach (var panel1 in panels)
            {
                panel1.panel.SetActive(false);
                panel1.contentButton.gameObject.SetActive(!panel1.isDisabled);
            }
               
            panel.panel.SetActive(true);
            panel.contentButton.gameObject.SetActive(false);
        }
    }
}
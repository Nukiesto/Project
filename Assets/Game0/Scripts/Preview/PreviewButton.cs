using UnityEngine;
using UnityEngine.UI;

namespace Game0.Preview
{
    [RequireComponent(typeof(Button))]
    public class PreviewButton : MonoBehaviour
    {
        [SerializeField] private PreviewManager previewManager;
        [SerializeField] private Image icon;
        
        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                previewManager.Open(icon.sprite);
            });
        }
    }
}
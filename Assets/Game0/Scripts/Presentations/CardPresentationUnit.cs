using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game0.Presentations
{
    public class CardPresentationUnit : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        public void SetData(PresentationUnitData data, PresentationView presentationView)
        {
            title.text = data.name;
            icon.sprite = data.icon0;
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                presentationView.SetData(data);
                presentationView.gameObject.SetActive(true);
            });
        }
    }
}
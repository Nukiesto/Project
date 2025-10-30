using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game0.Cards
{
    public class LiteratureCard : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Button button;
        
        public void SetData(LiteratureUnitData data, LiteratureInfo info)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                info.SetData(data);
                info.gameObject.SetActive(true);
            });
            icon.sprite = data.icon;
            nameText.text = data.name;
            dateText.text = data.date;
            descText.text = data.quote1;
        }
    }
}

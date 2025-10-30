using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game0.Cards
{
    public class LiteratureInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI biographyText;
        [SerializeField] private TextMeshProUGUI quoteText0;
        [SerializeField] private TextMeshProUGUI quoteText1;
        [SerializeField] private TextMeshProUGUI compositionText;

        [SerializeField] private TextMeshProUGUI factTitle1;
        [SerializeField] private TextMeshProUGUI factDesc1;
        [SerializeField] private TextMeshProUGUI factTitle2;
        [SerializeField] private TextMeshProUGUI factDesc2;

        [SerializeField] private Image icon;
        [SerializeField] private Image quoteIcon0;
        [SerializeField] private Image quoteIcon1;
        [SerializeField] private Image[] photos;
        [SerializeField] private GameObject gray;

        private void OnEnable()
        {
            gray.SetActive(true);
        }

        private void OnDisable()
        {
            gray.SetActive(false);
        }

        public void SetData(LiteratureUnitData data)
        {
            nameText.text = data.name;
            dateText.text = data.date;
            biographyText.text = data.biography;
            
            icon.sprite = data.icon;
            quoteIcon0.sprite = data.quoteIcon0;
            quoteIcon1.sprite = data.quoteIcon1;

            quoteText0.text = data.quote0;
            quoteText1.text = data.quote1;

            compositionText.text = data.compositionText;

            factTitle1.text = data.fact1Title;
            factDesc1.text = data.fact1Desc;
            factTitle2.text = data.fact2Title;
            factDesc2.text = data.fact2Desc;

            foreach (var photo in photos)
                photo.gameObject.SetActive(false);
            for (var i = 0; i < data.photos.Length; i++)
            {
                if (data.photos[i] == null)
                    continue;
                photos[i].gameObject.SetActive(true);
                photos[i].sprite = data.photos[i];
            }
        }
    }
}
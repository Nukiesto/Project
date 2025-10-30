using TMPro;
using UnityEngine;

namespace Game0.Chronics
{
    public class ChronicView : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI year;
        public TextMeshProUGUI date;
        public TextMeshProUGUI city;
        public TextMeshProUGUI desc;
        public TextMeshProUGUI history;
        public TextMeshProUGUI personalityPrefab;
        public TextMeshProUGUI materialTitle0;
        public TextMeshProUGUI materialTitle1;
        public TextMeshProUGUI materialDesc0;
        public TextMeshProUGUI materialDesc1;
        public GameObject personalitiesUnit;
        public GameObject materialUnit1;
        public GameObject materialUnit2;
        public GameObject materialUnitGlobal;
        public GameObject materialUnitTextGlobal;

        public string author;
        
        public void SetData(ChronicUnitData unitData)
        {
            author = unitData.author;
            title.text = unitData.title;
            year.text = unitData.year;
            date.text = unitData.date;
            city.text = unitData.city;
            desc.text = unitData.desc;
            history.text = unitData.history;
            materialTitle0.text = unitData.materialTitle0;
            materialTitle1.text = unitData.materialTitle1;
            materialDesc0.text = unitData.materialDesc0;
            materialDesc1.text = unitData.materialDesc1;
            
            materialUnit1.SetActive(true);
            materialUnit2.SetActive(true);
            materialUnitGlobal.SetActive(true);
            materialUnitTextGlobal.SetActive(true);
            
            if (string.IsNullOrEmpty(unitData.materialTitle0))
                materialUnit1.SetActive(false);
            if (string.IsNullOrEmpty(unitData.materialTitle1))
                materialUnit2.SetActive(false);
            if (string.IsNullOrEmpty(unitData.materialTitle0) && string.IsNullOrEmpty(unitData.materialTitle1))
            {
                materialUnitGlobal.SetActive(false);
                materialUnitTextGlobal.SetActive(false);
            }
            var p = personalityPrefab.transform.parent;
            var count = unitData.personalities.Length-1;
            personalitiesUnit.SetActive(count > 0);
            personalityPrefab.text = unitData.personalities[0];
            personalityPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = unitData.personalities[0];
            
            for (var i = 0; i < count; i++)
            {
                var s= Instantiate(personalityPrefab, Vector3.zero, Quaternion.identity, p);
                var text = unitData.personalities[i + 1];
                s.text = text;
                s.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                s.transform.SetParent(p);
            }
        }
    }
}
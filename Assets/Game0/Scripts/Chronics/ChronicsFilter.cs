using System;
using System.Collections.Generic;
using System.Linq;
using Game0.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game0.Chronics
{
    [Serializable]
    public class Filter
    {
        public string query;
        public string textSet;
        public Image image;
        public TextMeshProUGUI text;
        
        public bool isAny;
        public Button buttonEvent;
        
        public void Create(Button prefab)
        {
            buttonEvent = Object.Instantiate(prefab, prefab.transform.parent, true);
            buttonEvent.transform.SetParent(prefab.transform.parent);
            buttonEvent.gameObject.SetActive(true);
            buttonEvent.transform.localScale = Vector3.one;
            text = buttonEvent.GetComponentInChildren<TextMeshProUGUI>();
            image = buttonEvent.GetComponentInChildren<Image>();

            text.text = textSet;
        }
        public bool IsMatch(ChronicView chronicView)
        {
            if (isAny)
                return true;
            return chronicView.author.Contains(query) || query == chronicView.author || query.Contains(chronicView.author);
            
            // var year = chronicView.year.text;
            // if (int.TryParse(year, out var yearInt))
            //     return yearInt >= dateFrom && yearInt <= dateTo;
            return false;
        }
    }
    public class ChronicsFilter : MonoBehaviour
    {
        //[SerializeField] private Filter[] filters;
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color selectedButtonColor;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color defaultButtonColor;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private int defaultFilter;
        [SerializeField] private Button buttonPrefab;
        
        private List<ChronicView> _views;
        private List<Filter> _filters = new();
        private void Awake()
        {
            if (_filters.Count > 0)
                Filter(_filters[defaultFilter]);
        }

        public void SetViews(IReadOnlyCollection<ChronicView> views, List<LiteratureUnitData> cards)
        {
            _views = views.ToList();
            _filters = cards.Where(s=>s.name!="").Select(s => new Filter
            {
                query = s.name.Substring(5,s.name.Length - 6),
                textSet = s.name
            }).ToList();
            foreach (var filter in _filters)
                filter.Create(buttonPrefab);
            foreach (var filter in _filters)
                filter.buttonEvent.onClick.AddListener(() => { Filter(filter); });
        }
        private void OnEnable()
        {
            Filter(_filters[defaultFilter]);
        }

        private void Filter(Filter filter)
        {
            foreach (var filter1 in _filters)
            {
                filter1.text.color = defaultTextColor;
                filter1.image.sprite = defaultSprite;
                filter1.image.color = defaultButtonColor;
            }
            filter.text.color = selectedTextColor;
            filter.image.color = selectedButtonColor;
            filter.image.sprite = null;

            foreach (var chronicView in _views)
                chronicView.gameObject.SetActive(filter.IsMatch(chronicView));
        }
    }
}
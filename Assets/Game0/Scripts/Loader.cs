using System.Collections.Generic;
using System.Linq;
using Game0.Cards;
using Game0.Chronics;
using Game0.Presentations;
using UnityEngine;

namespace Game0
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private LiteratureCard literatureCard;
        [SerializeField] private ChronicView chronicView;
        [SerializeField] private LiteratureInfo literatureInfo;
        [SerializeField] private PresentationView presentationView;
        [SerializeField] private CardPresentationUnit presentationCard;
        [SerializeField] private ChronicsFilter filter;
        
        private void Start()
        {
            var cardLoader = new CardsLoader();
            cardLoader.LoadAll();
            var cards = cardLoader.GetAllData().ToList();
            cards.Sort();
            foreach (var data in cards)
            {
                var s = Instantiate(literatureCard, literatureCard.transform.parent, true);
                s.transform.SetParent(literatureCard.transform.parent);
                s.SetData(data, literatureInfo);
                s.transform.localScale = new Vector3(1, 1, 1);
            }
            literatureCard.gameObject.SetActive(false);
            
            var chronicsLoader = new ChronicsLoader();
            chronicsLoader.LoadAll();
            var views = new List<ChronicView>();
            
            foreach (var data in chronicsLoader.GetAllData())
            {
                var s = Instantiate(chronicView, chronicView.transform.parent, true);
                s.transform.SetParent(chronicView.transform.parent);
                s.SetData(data);
                s.transform.localScale = new Vector3(1, 1, 1);
                s.gameObject.SetActive(true);
                views.Add(s);
            }
            chronicView.gameObject.SetActive(false);
            filter.SetViews(views, cards);
            
            var presentationLoader = new PresentationLoader();
            presentationLoader .LoadAll();
            var ss = presentationCard.transform.parent.GetChild(1);
            foreach (var data in presentationLoader.GetAllData())
            {
                var s = Instantiate(presentationCard, presentationCard.transform.parent, true);
                s.transform.SetParent(presentationCard.transform.parent);
                s.SetData(data, presentationView);
                s.transform.localScale = new Vector3(1, 1, 1);
                s.gameObject.SetActive(true);
            }
            ss.SetAsLastSibling();
            presentationCard.gameObject.SetActive(false);
        }
    }
}
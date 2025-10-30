using System;
using UnityEngine;

namespace Game0.Cards
{
    [Serializable]
    public class LiteratureUnitData : IComparable<LiteratureUnitData>
    {
        public string name = "А.С. Пушкин";
        public string date = "1799-1837";
        public string biography = "Русский поэт, драматург и прозаик, заложивший основы русского реалистического направления. Учился в Царскосельском лицее, где провел самые важные годы формирования как поэта.";
        
        public Sprite quoteIcon0;
        public Sprite quoteIcon1;
        public string quote0 = "Мой друг, отчизне посвятим души прекрасные порывы!";
        public string quote1 = "Мой друг, отчизне посвятим души прекрасные порывы!";
        public Sprite icon;
        public string fact1Title = "Лицейские годы";
        public string fact1Desc = "Провел 6 лет в Царскосельском лицее\n1811-1817";
        public string fact2Title = "Лицейские годы";
        public string fact2Desc = "Провел 6 лет в Царскосельском лицее\n1811-1817";
        public string compositionText = "Евгений Онегин\nРуслан и Людмила\nКапитанская дочка\nМедный всадник";
        public Sprite[] photos;

        public LiteratureUnitData(string name, string date, string biography, Sprite quoteIcon0, Sprite quoteIcon1, string quote0, string quote1, Sprite icon, string fact1Title, string fact1Desc, string fact2Title, string fact2Desc, string compositionText, Sprite[] photos)
        {
            this.name = name;
            this.date = date;
            this.biography = biography;
            this.quoteIcon0 = quoteIcon0;
            this.quoteIcon1 = quoteIcon1;
            this.quote0 = quote0;
            this.quote1 = quote1;
            this.icon = icon;
            this.fact1Title = fact1Title;
            this.fact1Desc = fact1Desc;
            this.fact2Title = fact2Title;
            this.fact2Desc = fact2Desc;
            this.compositionText = compositionText;
            this.photos = photos;
        }

        public int CompareTo(LiteratureUnitData other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return string.Compare(name, other.name, StringComparison.Ordinal);
        }
    }
}
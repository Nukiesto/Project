using System;

namespace Game0.Chronics
{
    [Serializable]
    public class ChronicUnitData
    {
        public string title;
        public string year;
        public string date;
        public string city;
        public string desc;
        public string history;
        public string[] personalities;
        public string materialTitle0;
        public string materialTitle1;
        public string materialDesc0;
        public string materialDesc1;
        public string author;
        
        public ChronicUnitData(string title, string year, string date, string city, string desc, string history, string[] personalities, string materialTitle0, string materialTitle1, string materialDesc0, string materialDesc1, string author)
        {
            this.title = title;
            this.year = year;
            this.date = date;
            this.city = city;
            this.desc = desc;
            this.history = history;
            this.personalities = personalities;
            this.materialTitle0 = materialTitle0;
            this.materialTitle1 = materialTitle1;
            this.materialDesc0 = materialDesc0;
            this.materialDesc1 = materialDesc1;
            this.author = author;
        }
    }
}

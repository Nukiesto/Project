using System;
using UnityEngine;

namespace Game0.Presentations
{
    [Serializable]
    public class PresentationUnitData
    {
        public string date;
        public string desc0;
        public string desc;
        public Sprite icon;
        public Sprite icon0;
        public string name;

        public PresentationUnitData(string date, string desc0, string desc, Sprite icon, string name, Sprite icon0)
        {
            this.date = date;
            this.desc0 = desc0;
            this.desc = desc;
            this.icon = icon;
            this.name = name;
            this.icon0 = icon0;
        }
    }
}
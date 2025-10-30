using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game0.Cards;
using UnityEngine;

namespace Game0.Presentations
{
    public class PresentationLoader
    {
        private string DataPath => Application.streamingAssetsPath;
        
        private readonly List<PresentationUnitData> _objects = new();
            
        public void LoadAll()
        {
            LoadAllData();
        }
        public IReadOnlyCollection<PresentationUnitData> GetAllData() => _objects;
        
        private void LoadAllData()
        {
            var excel = ExcelHelper.LoadExcel(DataPath + "/Админ панель.xlsm");
            var tableFirst = excel.Tables.FirstOrDefault(s=>s.TableName == "Презентации");
            if (tableFirst == null)
                return;
            for (var i = 2; i < tableFirst.NumberOfRows; i++)
            {
                var pathIcon = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 5);
                var pathIcon0 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 7);
                
                Sprite spriteIcon = LoadSprite(pathIcon);
                Sprite spriteIcon0 = LoadSprite(pathIcon0);
                if (string.IsNullOrWhiteSpace((string)tableFirst.GetValue(i, 2)))
                    continue;
                var unit = new PresentationUnitData(
                    (string)tableFirst.GetValue(i, 2),
                    (string)tableFirst.GetValue(i, 3),
                    (string)tableFirst.GetValue(i, 4),
                    spriteIcon,
                    (string)tableFirst.GetValue(i, 6),
                    spriteIcon0);
                _objects.Add(unit);
            }
        }

        private Sprite LoadSprite(string path)
        {
            if (File.Exists(path))
                return LoadNewSprite(path);
            return null;
        }
        
        public Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {      
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            Texture2D SpriteTexture = LoadTexture(filePath);
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), pixelsPerUnit, 0, spriteType);
 
            return NewSprite;
        }
 
        public Texture2D LoadTexture(string filePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails
 
            Texture2D Tex2D;
            byte[] FileData;
 
            if (File.Exists(filePath))
            {
                FileData = File.ReadAllBytes(filePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
    }
}
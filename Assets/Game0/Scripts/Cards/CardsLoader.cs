
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game0.Cards
{
    public class CardsLoader
    {
        private string DataPath => Application.streamingAssetsPath;
        
        private readonly List<LiteratureUnitData> _objects = new();
            
        public void LoadAll()
        {
            LoadAllData();
        }
        public IReadOnlyCollection<LiteratureUnitData> GetAllData() => _objects;
        
        private void LoadAllData()
        {
            var excel = ExcelHelper.LoadExcel(DataPath + "/Админ панель.xlsm");
            var tableFirst = excel.Tables.FirstOrDefault(s=>s.TableName == "Литераторы");
            if (tableFirst == null)
                return;
            for (var i = 2; i < tableFirst.NumberOfRows; i++)
            {
                var pathQuoteIcon0 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 14);
                var pathQuoteIcon1 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 15);
                var pathIcon = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 6);
                
                var pathPhoto0 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 16);
                var pathPhoto1 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 17);
                var pathPhoto2 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 18);
                var pathPhoto3 = Application.streamingAssetsPath +"/"+ (string) tableFirst.GetValue(i, 19);
                
                Sprite spriteQuoteIcon0 = LoadSprite(pathQuoteIcon0);
                Sprite spriteQuoteIcon1 = LoadSprite(pathQuoteIcon1);
                Sprite spriteIcon = LoadSprite(pathIcon);
                Sprite spritePhoto0 = LoadSprite(pathPhoto0);;
                Sprite spritePhoto1 = LoadSprite(pathPhoto1);
                Sprite spritePhoto2 = LoadSprite(pathPhoto2);
                Sprite spritePhoto3 = LoadSprite(pathPhoto3);
                if (string.IsNullOrWhiteSpace((string)tableFirst.GetValue(i, 2)))
                    continue;
                var unit = new LiteratureUnitData(
                    (string)tableFirst.GetValue(i, 2),
                    (string)tableFirst.GetValue(i, 3),
                    (string)tableFirst.GetValue(i, 4),
                    spriteQuoteIcon0,
                    spriteQuoteIcon1,
                    (string)tableFirst.GetValue(i, 5),
                    (string)tableFirst.GetValue(i, 8),
                    spriteIcon,
                    (string)tableFirst.GetValue(i, 9),
                    (string)tableFirst.GetValue(i, 10),
                    (string)tableFirst.GetValue(i, 11),
                    (string)tableFirst.GetValue(i, 12),
                    (string)tableFirst.GetValue(i, 13),
                    new Sprite[4]{spritePhoto0, spritePhoto1, spritePhoto2, spritePhoto3});
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
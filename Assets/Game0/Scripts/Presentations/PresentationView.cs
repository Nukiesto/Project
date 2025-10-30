using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game0.Presentations
{
    public class PresentationView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI desc0Text;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject gray;
        [SerializeField] private GameObject firstPanelText;
        [SerializeField] private GameObject secondPanelText;
        [SerializeField] private GameObject thirdPanelText;
        [SerializeField] private TextMeshProUGUI textUnit;
        [SerializeField] private Image imageUnit;
        
        public void SetData(PresentationUnitData data)
        {
            desc0Text.gameObject.SetActive(!string.IsNullOrEmpty(data.desc0));
            dateText.gameObject.SetActive(!string.IsNullOrEmpty(data.date));
            
            firstPanelText.gameObject.SetActive(!string.IsNullOrEmpty(data.desc0));
            
            dateText.text = data.date;
            desc0Text.text = data.desc0;
            descText.text = data.desc;
            
            icon.gameObject.SetActive(data.icon!=null);
            icon.sprite = data.icon;

            var desc = data.desc;
            var c = desc.Contains("image");
            secondPanelText.SetActive(!c);
            thirdPanelText.SetActive(c);
            if (c)
                SetImagedText(desc);
        }

        private void SetImagedText(string text)
        {
            //[image=Презентации\9 Мая\image 2.png]
            
            textUnit.gameObject.SetActive(false);
            imageUnit.gameObject.SetActive(false);

            for (var i = 0; i < thirdPanelText.transform.childCount; i++)
            {
                var child = thirdPanelText.transform.GetChild(i);
                if (child == textUnit.transform || child == imageUnit.transform)
                    continue;
                Destroy(child.gameObject);
            }
            var lines = text.Split('\r', '\n');
            var textCashed = new StringBuilder();
            
            foreach (var line in lines)
            {
                if (line.Contains("[image="))
                {
                    var newText = Instantiate(textUnit, textUnit.transform.parent, true);
                    newText.text = textCashed.ToString();
                    newText.gameObject.SetActive(true);

                    var imagePath = line.Replace("[image=", "");
                    imagePath = imagePath.Replace("]", "");
                    var path = Application.streamingAssetsPath + "/" + imagePath;
                    if (File.Exists(path))
                    {
                        var sprite = LoadSprite(path);
                        if (sprite != null)
                        {
                            var newImage = Instantiate(imageUnit, imageUnit.transform.parent, true);
                            newImage.sprite = sprite;
                            newImage.gameObject.SetActive(true);
                        }
                    }
                    textCashed.Clear();
                }
                else
                    textCashed.AppendLine(line);
            }
            
            var newText2 = Instantiate(textUnit, textUnit.transform.parent, true);
            newText2.text = textCashed.ToString();
            newText2.gameObject.SetActive(true);
        }
        private void OnEnable()
        {
            gray.SetActive(true);
        }

        private void OnDisable()
        {
            gray.SetActive(false);
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
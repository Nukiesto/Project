using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game0.Preview
{
    public class PreviewManager : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private Image icon;
        [SerializeField] private float durationFade;
        [SerializeField] private Button button;
        [SerializeField] private float fade;
        [SerializeField] private float maxW;
        [SerializeField] private float maxH;
        [SerializeField] private float xx;
        
        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                button.interactable = false;
                Close().Forget();
            });
        }

        public void Open(Sprite iconSet)
        {
            button.interactable = true;
            gameObject.SetActive(true);
            
            // Используем новый ImageScaler для правильного масштабирования с учетом аспекта
            float targetWidth = Mathf.Min(iconSet.textureRect.width * xx, maxW);
            ImageScaler.ScaleToWidth(icon, iconSet, targetWidth, maxH, true);
            
            Fade(icon, 0f, 1f).Forget();
        }
        private async UniTaskVoid Close()
        {
            Fade(icon, 1f, 0f).Forget();
            
            await UniTask.Delay(TimeSpan.FromSeconds(durationFade));
            gameObject.SetActive(false);
        }
        private async UniTaskVoid Fade(Image image, float from, float to)
        {
            var timer = 0f;
            var color = image.color;
            color.a = from;
            while (timer < durationFade)
            {
                timer += Time.deltaTime;
                await UniTask.Yield();
                color.a = Mathf.Lerp(from, to, timer / durationFade); 
                image.color = color;
            }
        }
    }
}
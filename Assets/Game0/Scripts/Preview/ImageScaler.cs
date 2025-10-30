using UnityEngine;
using UnityEngine.UI;

namespace Game0.Preview
{
    /// <summary>
    /// Класс для масштабирования изображений с учетом аспекта исходного спрайта
    /// </summary>
    public static class ImageScaler
    {
        /// <summary>
        /// Масштабирует изображение в заданные рамки с сохранением аспекта
        /// </summary>
        /// <param name="image">UI Image компонент</param>
        /// <param name="sprite">Спрайт для отображения</param>
        /// <param name="maxWidth">Максимальная ширина</param>
        /// <param name="maxHeight">Максимальная высота</param>
        /// <param name="clearPosition">Сбросить позицию в центр</param>
        public static void ScaleToFit(Image image, Sprite sprite, float maxWidth, float maxHeight, bool clearPosition = true)
        {
            if (image == null || sprite == null)
                return;

            image.enabled = true;
            image.sprite = sprite;
            image.SetNativeSize();

            if (clearPosition)
                image.rectTransform.anchoredPosition = Vector2.zero;

            Vector2 originalSize = image.rectTransform.sizeDelta;
            Vector2 scaledSize = CalculateScaledSize(originalSize, maxWidth, maxHeight);
            
            image.rectTransform.sizeDelta = scaledSize;
        }

        /// <summary>
        /// Масштабирует изображение по ширине с сохранением аспекта
        /// </summary>
        /// <param name="image">UI Image компонент</param>
        /// <param name="sprite">Спрайт для отображения</param>
        /// <param name="targetWidth">Целевая ширина</param>
        /// <param name="maxHeight">Максимальная высота (опционально)</param>
        /// <param name="clearPosition">Сбросить позицию в центр</param>
        public static void ScaleToWidth(Image image, Sprite sprite, float targetWidth, float maxHeight = float.MaxValue, bool clearPosition = true)
        {
            if (image == null || sprite == null)
                return;

            image.enabled = true;
            image.sprite = sprite;
            image.SetNativeSize();

            if (clearPosition)
                image.rectTransform.anchoredPosition = Vector2.zero;

            Vector2 originalSize = image.rectTransform.sizeDelta;
            Vector2 scaledSize = CalculateScaledSizeByWidth(originalSize, targetWidth, maxHeight);
            
            image.rectTransform.sizeDelta = scaledSize;
        }

        /// <summary>
        /// Масштабирует изображение по высоте с сохранением аспекта
        /// </summary>
        /// <param name="image">UI Image компонент</param>
        /// <param name="sprite">Спрайт для отображения</param>
        /// <param name="targetHeight">Целевая высота</param>
        /// <param name="maxWidth">Максимальная ширина (опционально)</param>
        /// <param name="clearPosition">Сбросить позицию в центр</param>
        public static void ScaleToHeight(Image image, Sprite sprite, float targetHeight, float maxWidth = float.MaxValue, bool clearPosition = true)
        {
            if (image == null || sprite == null)
                return;

            image.enabled = true;
            image.sprite = sprite;
            image.SetNativeSize();

            if (clearPosition)
                image.rectTransform.anchoredPosition = Vector2.zero;

            Vector2 originalSize = image.rectTransform.sizeDelta;
            Vector2 scaledSize = CalculateScaledSizeByHeight(originalSize, targetHeight, maxWidth);
            
            image.rectTransform.sizeDelta = scaledSize;
        }

        /// <summary>
        /// Вычисляет размеры для масштабирования в рамки с сохранением аспекта
        /// </summary>
        private static Vector2 CalculateScaledSize(Vector2 originalSize, float maxWidth, float maxHeight)
        {
            float aspectRatio = originalSize.x / originalSize.y;
            
            // Вычисляем размеры, которые поместятся в рамки
            float scaledWidth = maxWidth;
            float scaledHeight = scaledWidth / aspectRatio;
            
            // Если высота превышает максимальную, масштабируем по высоте
            if (scaledHeight > maxHeight)
            {
                scaledHeight = maxHeight;
                scaledWidth = scaledHeight * aspectRatio;
            }
            
            return new Vector2(scaledWidth, scaledHeight);
        }

        /// <summary>
        /// Вычисляет размеры для масштабирования по ширине
        /// </summary>
        private static Vector2 CalculateScaledSizeByWidth(Vector2 originalSize, float targetWidth, float maxHeight)
        {
            float aspectRatio = originalSize.x / originalSize.y;
            float scaledWidth = targetWidth;
            float scaledHeight = scaledWidth / aspectRatio;
            
            // Проверяем ограничение по высоте
            if (scaledHeight > maxHeight)
            {
                scaledHeight = maxHeight;
                scaledWidth = scaledHeight * aspectRatio;
            }
            
            return new Vector2(scaledWidth, scaledHeight);
        }

        /// <summary>
        /// Вычисляет размеры для масштабирования по высоте
        /// </summary>
        private static Vector2 CalculateScaledSizeByHeight(Vector2 originalSize, float targetHeight, float maxWidth)
        {
            float aspectRatio = originalSize.x / originalSize.y;
            float scaledHeight = targetHeight;
            float scaledWidth = scaledHeight * aspectRatio;
            
            // Проверяем ограничение по ширине
            if (scaledWidth > maxWidth)
            {
                scaledWidth = maxWidth;
                scaledHeight = scaledWidth / aspectRatio;
            }
            
            return new Vector2(scaledWidth, scaledHeight);
        }
    }
}


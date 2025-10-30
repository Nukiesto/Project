using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Main
{
    [RequireComponent(typeof(RectTransform))]
    public class UIAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Ease animationEase = Ease.OutBack;
        [SerializeField] private float fadeInDelay = 0f;
        [SerializeField] private float fadeOutDelay = 0f;
        
        [Header("Slide Animation")]
        [SerializeField] private SlideDirection slideDirection = SlideDirection.Bottom;
        [SerializeField] private float slideDistance = 100f;
        [SerializeField] private bool enableSlideAnimation = true;
        
        [Header("Fade Animation")]
        [SerializeField] private bool enableFadeAnimation = true;
        [SerializeField] private float minAlpha = 0f;
        [SerializeField] private float maxAlpha = 1f;
        
        [Header("Scale Animation")]
        [SerializeField] private bool enableScaleAnimation = false;
        [SerializeField] private float minScale = 0.8f;
        [SerializeField] private float maxScale = 1f;
        
        [Header("Auto Animation")]
        [SerializeField] private bool animateOnStart = false;
        [SerializeField] private bool animateOnEnable = false;
        
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Image image;
        private Vector2 originalPosition;
        private Vector3 originalScale;
        private float originalAlpha;
        private Sequence currentAnimation;
        private CancellationTokenSource cancellationTokenSource;
        
        public enum SlideDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            image = GetComponent<Image>();
            
            // Создаем CanvasGroup если его нет
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Сохраняем оригинальные значения
            originalPosition = rectTransform.anchoredPosition;
            originalScale = rectTransform.localScale;
            originalAlpha = canvasGroup.alpha;
            
            cancellationTokenSource = new CancellationTokenSource();
        }
        
        private void Start()
        {
            if (animateOnStart)
            {
                FadeInAsync().Forget();
            }
        }
        
        private void OnEnable()
        {
            if (animateOnEnable)
            {
                FadeInAsync().Forget();
            }
        }
        
        private void OnDestroy()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
        }
        
        /// <summary>
        /// Анимация появления элемента (FadeIn)
        /// </summary>
        public async UniTask FadeInAsync()
        {
            if (fadeInDelay > 0)
            {
                await UniTask.Delay((int)(fadeInDelay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
            
            // Останавливаем текущую анимацию
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            // Показываем объект
            gameObject.SetActive(true);
            
            // Создаем новую последовательность анимаций
            currentAnimation = DOTween.Sequence();
            
            // Начальное состояние
            if (enableSlideAnimation)
            {
                Vector2 startPosition = GetSlideStartPosition();
                rectTransform.anchoredPosition = startPosition;
            }
            
            if (enableFadeAnimation)
            {
                canvasGroup.alpha = minAlpha;
            }
            
            if (enableScaleAnimation)
            {
                rectTransform.localScale = Vector3.one * minScale;
            }
            
            // Анимация позиции
            if (enableSlideAnimation)
            {
                currentAnimation.Join(rectTransform.DOAnchorPos(originalPosition, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Анимация прозрачности
            if (enableFadeAnimation)
            {
                currentAnimation.Join(canvasGroup.DOFade(maxAlpha, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Анимация масштаба
            if (enableScaleAnimation)
            {
                currentAnimation.Join(rectTransform.DOScale(originalScale * maxScale, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Ждем завершения анимации
            await currentAnimation.AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Анимация исчезновения элемента (FadeOut)
        /// </summary>
        public async UniTask FadeOutAsync()
        {
            if (fadeOutDelay > 0)
            {
                await UniTask.Delay((int)(fadeOutDelay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
            
            // Останавливаем текущую анимацию
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            // Создаем новую последовательность анимаций
            currentAnimation = DOTween.Sequence();
            
            // Анимация позиции
            if (enableSlideAnimation)
            {
                Vector2 endPosition = GetSlideStartPosition();
                currentAnimation.Join(rectTransform.DOAnchorPos(endPosition, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Анимация прозрачности
            if (enableFadeAnimation)
            {
                currentAnimation.Join(canvasGroup.DOFade(minAlpha, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Анимация масштаба
            if (enableScaleAnimation)
            {
                currentAnimation.Join(rectTransform.DOScale(originalScale * minScale, animationDuration)
                    .SetEase(animationEase));
            }
            
            // Ждем завершения анимации
            await currentAnimation.AsyncWaitForCompletion().AsUniTask();
            
            // Скрываем объект
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Получает начальную позицию для анимации слайда
        /// </summary>
        private Vector2 GetSlideStartPosition()
        {
            Vector2 startPosition = originalPosition;
            
            switch (slideDirection)
            {
                case SlideDirection.Top:
                    startPosition.y += slideDistance;
                    break;
                case SlideDirection.Bottom:
                    startPosition.y -= slideDistance;
                    break;
                case SlideDirection.Left:
                    startPosition.x -= slideDistance;
                    break;
                case SlideDirection.Right:
                    startPosition.x += slideDistance;
                    break;
            }
            
            return startPosition;
        }
        
        /// <summary>
        /// Сброс к исходному состоянию
        /// </summary>
        public void ResetToOriginalState()
        {
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
            canvasGroup.alpha = originalAlpha;
        }
        
        /// <summary>
        /// Установка параметров анимации во время выполнения
        /// </summary>
        public void SetAnimationParameters(float duration, Ease ease, float slideDist = -1f)
        {
            animationDuration = duration;
            animationEase = ease;
            if (slideDist >= 0f)
            {
                slideDistance = slideDist;
            }
        }
        
        /// <summary>
        /// Включение/выключение типов анимации
        /// </summary>
        public void SetAnimationTypes(bool slide, bool fade, bool scale)
        {
            enableSlideAnimation = slide;
            enableFadeAnimation = fade;
            enableScaleAnimation = scale;
        }
        
        /// <summary>
        /// Установка направления слайда
        /// </summary>
        public void SetSlideDirection(SlideDirection direction)
        {
            slideDirection = direction;
        }
        
        /// <summary>
        /// Публичные методы для совместимости с событиями Unity
        /// </summary>
        public void FadeIn()
        {
            FadeInAsync().Forget();
        }
        
        public void FadeOut()
        {
            FadeOutAsync().Forget();
        }
    }
}

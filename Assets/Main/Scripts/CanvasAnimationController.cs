using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

namespace Main
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasAnimationController : MonoBehaviour
    {
        [System.Serializable]
        public class UIElementAnimation
        {
            [Header("Element Reference")]
            public RectTransform element;
            
            [Header("Animation Settings")]
            public float animationDuration = 0.5f;
            public Ease animationEase = Ease.OutBack;
            public float delay = 0f;
            
            [Header("Slide Animation")]
            public SlideDirection slideDirection = SlideDirection.Bottom;
            public float slideDistance = 100f;
            public bool enableSlideAnimation = true;
            
            [Header("Fade Animation")]
            public bool enableFadeAnimation = true;
            public float minAlpha = 0f;
            public float maxAlpha = 1f;
            
            [Header("Scale Animation")]
            public bool enableScaleAnimation = false;
            public float minScale = 0.8f;
            public float maxScale = 1f;
            
            [Header("Auto Animation")]
            public bool animateOnCanvasShow = true;
            public bool animateOnCanvasHide = true;
        }
        
        public enum SlideDirection
        {
            Top,
            Bottom,
            Left,
            Right
        }
        
        public enum AnimationMode
        {
            Sequential,    // Элементы анимируются по очереди
            Parallel,      // Все элементы анимируются одновременно
            Staggered     // Элементы анимируются с задержкой между ними
        }
        
        [Header("Canvas Animation Settings")]
        [SerializeField] private AnimationMode animationMode = AnimationMode.Staggered;
        [SerializeField] private float staggerDelay = 0.1f;
        [SerializeField] private bool autoAnimateOnStart = false;
        [SerializeField] private bool autoAnimateOnEnable = false;
        
        [Header("UI Elements")]
        [SerializeField] private List<UIElementAnimation> uiElements = new List<UIElementAnimation>();
        
        [Header("Animation Sequences")]
        [SerializeField] private bool createShowSequence = true;
        [SerializeField] private bool createHideSequence = true;
        
        [Header("Debug Controls")]
        [SerializeField] private bool showDebugInfo = true;
        
        [SerializeField] private Canvas canvas;
        private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
        private List<Vector2> originalPositions = new List<Vector2>();
        private List<Vector3> originalScales = new List<Vector3>();
        private List<float> originalAlphas = new List<float>();
        private Sequence currentAnimation;
        private CancellationTokenSource cancellationTokenSource;
        
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            cancellationTokenSource = new CancellationTokenSource();
            
            InitializeElements();
        }
        
        private void Start()
        {
            if (autoAnimateOnStart)
            {
                ShowCanvasAsync().Forget();
            }
        }
        
        private void OnEnable()
        {
            if (autoAnimateOnEnable)
            {
                ShowCanvasAsync().Forget();
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
        /// Инициализация элементов UI
        /// </summary>
        private void InitializeElements()
        {
            canvasGroups.Clear();
            originalPositions.Clear();
            originalScales.Clear();
            originalAlphas.Clear();
            
            foreach (var element in uiElements)
            {
                if (element.element == null) continue;
                
                // Получаем или создаем CanvasGroup
                var canvasGroup = element.element.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = element.element.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroups.Add(canvasGroup);
                
                // Сохраняем оригинальные значения
                originalPositions.Add(element.element.anchoredPosition);
                originalScales.Add(element.element.localScale);
                originalAlphas.Add(canvasGroup.alpha);
            }
        }
        
        /// <summary>
        /// Показать Canvas с анимацией всех элементов
        /// </summary>
        public async UniTask ShowCanvasAsync()
        {
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            canvas.enabled = true;
            gameObject.SetActive(true);
            
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    await ShowSequentialAsync();
                    break;
                case AnimationMode.Parallel:
                    await ShowParallelAsync();
                    break;
                case AnimationMode.Staggered:
                    await ShowStaggeredAsync();
                    break;
            }
        }
        
        /// <summary>
        /// Скрыть Canvas с анимацией всех элементов
        /// </summary>
        public async UniTask HideCanvasAsync()
        {
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    await HideSequentialAsync();
                    break;
                case AnimationMode.Parallel:
                    await HideParallelAsync();
                    break;
                case AnimationMode.Staggered:
                    await HideStaggeredAsync();
                    break;
            }
            
            canvas.enabled = false;
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Последовательная анимация показа
        /// </summary>
        private async UniTask ShowSequentialAsync()
        {
            currentAnimation = DOTween.Sequence();
            
            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasShow) continue;
                
                await AnimateElementShowAsync(element, i);
            }
        }
        
        /// <summary>
        /// Параллельная анимация показа
        /// </summary>
        private async UniTask ShowParallelAsync()
        {
            currentAnimation = DOTween.Sequence();
            var tasks = new List<UniTask>();
            
            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasShow) continue;
                
                tasks.Add(AnimateElementShowAsync(element, i));
            }
            
            await UniTask.WhenAll(tasks);
        }
        
        /// <summary>
        /// Ступенчатая анимация показа
        /// </summary>
        private async UniTask ShowStaggeredAsync()
        {
            currentAnimation = DOTween.Sequence();
            
            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasShow) continue;
                
                _ = AnimateElementShowAsync(element, i);
                await UniTask.Delay((int)(staggerDelay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
        }
        
        /// <summary>
        /// Последовательная анимация скрытия
        /// </summary>
        private async UniTask HideSequentialAsync()
        {
            currentAnimation = DOTween.Sequence();
            
            for (int i = uiElements.Count - 1; i >= 0; i--)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasHide) continue;
                
                await AnimateElementHideAsync(element, i);
            }
        }
        
        /// <summary>
        /// Параллельная анимация скрытия
        /// </summary>
        private async UniTask HideParallelAsync()
        {
            currentAnimation = DOTween.Sequence();
            var tasks = new List<UniTask>();
            
            for (int i = 0; i < uiElements.Count; i++)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasHide) continue;
                
                tasks.Add(AnimateElementHideAsync(element, i));
            }
            
            await UniTask.WhenAll(tasks);
        }
        
        /// <summary>
        /// Ступенчатая анимация скрытия
        /// </summary>
        private async UniTask HideStaggeredAsync()
        {
            currentAnimation = DOTween.Sequence();
            
            for (int i = uiElements.Count - 1; i >= 0; i--)
            {
                var element = uiElements[i];
                if (element.element == null || !element.animateOnCanvasHide) continue;
                
                _ = AnimateElementHideAsync(element, i);
                await UniTask.Delay((int)(staggerDelay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
        }
        
        /// <summary>
        /// Анимация показа отдельного элемента
        /// </summary>
        private async UniTask AnimateElementShowAsync(UIElementAnimation element, int index)
        {
            if (element.delay > 0)
            {
                await UniTask.Delay((int)(element.delay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
            
            //element.element.gameObject.SetActive(true);
            
            var sequence = DOTween.Sequence();
            
            // Начальное состояние
            if (element.enableSlideAnimation)
            {
                Vector2 startPosition = GetSlideStartPosition(element, originalPositions[index]);
                element.element.anchoredPosition = startPosition;
            }
            
            if (element.enableFadeAnimation)
            {
                canvasGroups[index].alpha = element.minAlpha;
            }
            
            if (element.enableScaleAnimation)
            {
                element.element.localScale = Vector3.one * element.minScale;
            }
            
            // Анимация позиции
            if (element.enableSlideAnimation)
            {
                sequence.Join(element.element.DOAnchorPos(originalPositions[index], element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            // Анимация прозрачности
            if (element.enableFadeAnimation)
            {
                sequence.Join(canvasGroups[index].DOFade(element.maxAlpha, element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            // Анимация масштаба
            if (element.enableScaleAnimation)
            {
                sequence.Join(element.element.DOScale(originalScales[index] * element.maxScale, element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            await sequence.AsyncWaitForCompletion();
        }
        
        /// <summary>
        /// Анимация скрытия отдельного элемента
        /// </summary>
        private async UniTask AnimateElementHideAsync(UIElementAnimation element, int index)
        {
            if (element.delay > 0)
            {
                await UniTask.Delay((int)(element.delay * 1000), cancellationToken: cancellationTokenSource.Token);
            }
            
            var sequence = DOTween.Sequence();
            
            // Анимация позиции
            if (element.enableSlideAnimation)
            {
                Vector2 endPosition = GetSlideStartPosition(element, originalPositions[index]);
                sequence.Join(element.element.DOAnchorPos(endPosition, element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            // Анимация прозрачности
            if (element.enableFadeAnimation)
            {
                sequence.Join(canvasGroups[index].DOFade(element.minAlpha, element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            // Анимация масштаба
            if (element.enableScaleAnimation)
            {
                sequence.Join(element.element.DOScale(originalScales[index] * element.minScale, element.animationDuration)
                    .SetEase(element.animationEase));
            }
            
            await sequence.AsyncWaitForCompletion();
            //element.element.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Получить начальную позицию для анимации слайда
        /// </summary>
        private Vector2 GetSlideStartPosition(UIElementAnimation element, Vector2 originalPosition)
        {
            Vector2 startPosition = originalPosition;
            
            switch (element.slideDirection)
            {
                case SlideDirection.Top:
                    startPosition.y += element.slideDistance;
                    break;
                case SlideDirection.Bottom:
                    startPosition.y -= element.slideDistance;
                    break;
                case SlideDirection.Left:
                    startPosition.x -= element.slideDistance;
                    break;
                case SlideDirection.Right:
                    startPosition.x += element.slideDistance;
                    break;
            }
            
            return startPosition;
        }
        
        /// <summary>
        /// Сброс всех элементов к исходному состоянию
        /// </summary>
        public void ResetAllElements()
        {
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
            
            for (int i = 0; i < uiElements.Count; i++)
            {
                if (uiElements[i].element == null) continue;
                
                uiElements[i].element.anchoredPosition = originalPositions[i];
                uiElements[i].element.localScale = originalScales[i];
                canvasGroups[i].alpha = originalAlphas[i];
            }
        }
        
        /// <summary>
        /// Добавить элемент для анимации
        /// </summary>
        public void AddElement(RectTransform element, UIElementAnimation animation = null)
        {
            if (animation == null)
            {
                animation = new UIElementAnimation();
            }
            
            animation.element = element;
            uiElements.Add(animation);
            
            // Переинициализируем элементы
            InitializeElements();
        }
        
        /// <summary>
        /// Удалить элемент из анимации
        /// </summary>
        public void RemoveElement(RectTransform element)
        {
            uiElements.RemoveAll(x => x.element == element);
            InitializeElements();
        }
        
        /// <summary>
        /// Установить режим анимации
        /// </summary>
        public void SetAnimationMode(AnimationMode mode)
        {
            animationMode = mode;
        }
        
        /// <summary>
        /// Установить задержку между элементами для ступенчатой анимации
        /// </summary>
        public void SetStaggerDelay(float delay)
        {
            staggerDelay = delay;
        }
        
        /// <summary>
        /// Публичные методы для совместимости с событиями Unity
        /// </summary>
        public void ShowCanvas()
        {
            ShowCanvasAsync().Forget();
        }
        
        public void HideCanvas()
        {
            HideCanvasAsync().Forget();
        }
        
        /// <summary>
        /// Получить количество элементов
        /// </summary>
        public int GetElementCount()
        {
            return uiElements.Count;
        }
        
        /// <summary>
        /// Получить элемент по индексу
        /// </summary>
        public UIElementAnimation GetElement(int index)
        {
            if (index >= 0 && index < uiElements.Count)
            {
                return uiElements[index];
            }
            return null;
        }
        
        /// <summary>
        /// Получить текущий режим анимации
        /// </summary>
        public AnimationMode GetCurrentAnimationMode()
        {
            return animationMode;
        }
        
        /// <summary>
        /// Получить текущую задержку ступенчатой анимации
        /// </summary>
        public float GetCurrentStaggerDelay()
        {
            return staggerDelay;
        }
        
        #region Debug Methods
        
        [Button("Show Canvas")]
        private void DebugShowCanvas()
        {
            if (Application.isPlaying)
            {
                ShowCanvasAsync().Forget();
                Debug.Log($"[CanvasAnimationController] Showing canvas with {GetElementCount()} elements");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Hide Canvas")]
        private void DebugHideCanvas()
        {
            if (Application.isPlaying)
            {
                HideCanvasAsync().Forget();
                Debug.Log($"[CanvasAnimationController] Hiding canvas with {GetElementCount()} elements");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Reset All Elements")]
        private void DebugResetElements()
        {
            if (Application.isPlaying)
            {
                ResetAllElements();
                Debug.Log("[CanvasAnimationController] All elements reset to original state");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Test Sequential Animation")]
        private void DebugTestSequential()
        {
            if (Application.isPlaying)
            {
                SetAnimationMode(AnimationMode.Sequential);
                ShowCanvasAsync().Forget();
                Debug.Log("[CanvasAnimationController] Testing Sequential animation mode");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Test Parallel Animation")]
        private void DebugTestParallel()
        {
            if (Application.isPlaying)
            {
                SetAnimationMode(AnimationMode.Parallel);
                ShowCanvasAsync().Forget();
                Debug.Log("[CanvasAnimationController] Testing Parallel animation mode");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Test Staggered Animation")]
        private void DebugTestStaggered()
        {
            if (Application.isPlaying)
            {
                SetAnimationMode(AnimationMode.Staggered);
                ShowCanvasAsync().Forget();
                Debug.Log("[CanvasAnimationController] Testing Staggered animation mode");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Print Debug Info")]
        private void DebugPrintInfo()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"[CanvasAnimationController] Debug Info:");
                Debug.Log($"- Canvas enabled: {canvas.enabled}");
                Debug.Log($"- Game object active: {gameObject.activeInHierarchy}");
                Debug.Log($"- Animation mode: {animationMode}");
                Debug.Log($"- Stagger delay: {staggerDelay}");
                Debug.Log($"- Elements count: {GetElementCount()}");
                
                for (int i = 0; i < uiElements.Count; i++)
                {
                    var element = uiElements[i];
                    if (element.element != null)
                    {
                        Debug.Log($"- Element {i}: {element.element.name}");
                        Debug.Log($"  - Slide animation: {element.enableSlideAnimation}");
                        Debug.Log($"  - Fade animation: {element.enableFadeAnimation}");
                        Debug.Log($"  - Scale animation: {element.enableScaleAnimation}");
                        Debug.Log($"  - Duration: {element.animationDuration}");
                        Debug.Log($"  - Delay: {element.delay}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        [Button("Validate Elements")]
        private void DebugValidateElements()
        {
            int validElements = 0;
            int invalidElements = 0;
            
            foreach (var element in uiElements)
            {
                if (element.element != null)
                {
                    validElements++;
                }
                else
                {
                    invalidElements++;
                }
            }
            
            Debug.Log($"[CanvasAnimationController] Element validation:");
            Debug.Log($"- Valid elements: {validElements}");
            Debug.Log($"- Invalid elements: {invalidElements}");
            Debug.Log($"- Total elements: {uiElements.Count}");
            
            if (invalidElements > 0)
            {
                Debug.LogWarning($"[CanvasAnimationController] Found {invalidElements} invalid elements! Please check the UI Elements list.");
            }
        }
        
        [Button("Quick Setup Test")]
        private void DebugQuickSetup()
        {
            if (Application.isPlaying)
            {
                // Быстрая настройка для тестирования
                SetAnimationMode(AnimationMode.Staggered);
                SetStaggerDelay(0.1f);
                
                // Настройка элементов по умолчанию если они не настроены
                foreach (var element in uiElements)
                {
                    if (element.element != null)
                    {
                        element.animationDuration = 0.5f;
                        element.animationEase = Ease.OutBack;
                        element.enableSlideAnimation = true;
                        element.enableFadeAnimation = true;
                        element.enableScaleAnimation = false;
                    }
                }
                
                Debug.Log("[CanvasAnimationController] Quick setup applied. Ready for testing!");
            }
            else
            {
                Debug.LogWarning("[CanvasAnimationController] Debug methods only work in Play mode!");
            }
        }
        
        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Main
{
    [RequireComponent(typeof(Button))]
    public class AnimationTrigger : MonoBehaviour
    {
        [Header("Animation Controller")]
        [SerializeField] private CanvasAnimationController animationController;
        
        [Header("Animation Settings")]
        [SerializeField] private AnimationType animationType = AnimationType.Show;
        [SerializeField] private CanvasAnimationController.AnimationMode customAnimationMode = CanvasAnimationController.AnimationMode.Staggered;
        [SerializeField] private float customStaggerDelay = 0.1f;
        
        [Header("Element Selection")]
        [SerializeField] private ElementSelectionMode selectionMode = ElementSelectionMode.ByIndices;
        
        [ShowIf("selectionMode", ElementSelectionMode.ByIndices)]
        [SerializeField] private int[] elementIndices = new int[0];
        
        [ShowIf("selectionMode", ElementSelectionMode.ByRange)]
        [SerializeField] private int startIndex = 0;
        [ShowIf("selectionMode", ElementSelectionMode.ByRange)]
        [SerializeField] private int endIndex = 0;
        
        [ShowIf("selectionMode", ElementSelectionMode.AllElements)]
        [SerializeField] private bool reverseOrder = false;
        
        [Header("Custom Animation")]
        [SerializeField] private bool useCustomSettings = false;
        [ShowIf("useCustomSettings")]
        [SerializeField] private float customDuration = 0.5f;
        [ShowIf("useCustomSettings")]
        [SerializeField] private DG.Tweening.Ease customEase = DG.Tweening.Ease.OutBack;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private Button button;
        public UnityEvent onEndAnimation;
        
        public enum AnimationType
        {
            Show,
            Hide,
            Toggle
        }
        
        public enum ElementSelectionMode
        {
            ByIndices,      // Выбор по конкретным индексам
            ByRange,        // Выбор по диапазону индексов
            AllElements     // Все элементы
        }
        
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(TriggerAnimation);
            
            // Автоматически находим контроллер если не задан
            if (animationController == null)
            {
                animationController = FindObjectOfType<CanvasAnimationController>();
                if (animationController == null)
                {
                    Debug.LogWarning("[AnimationTrigger] No CanvasAnimationController found! Please assign one manually.");
                }
            }
        }
        
        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(TriggerAnimation);
            }
        }
        
        /// <summary>
        /// Запуск анимации при нажатии кнопки
        /// </summary>
        [Button("Trigger Animation")]
        public void TriggerAnimation()
        {
            if (animationController == null)
            {
                Debug.LogError("[AnimationTrigger] No animation controller assigned!");
                return;
            }
            
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[AnimationTrigger] Animation can only be triggered in Play mode!");
                return;
            }
            
            _ = ExecuteAnimationAsync();
        }
        
        /// <summary>
        /// Асинхронное выполнение анимации
        /// </summary>
        private async UniTask ExecuteAnimationAsync()
        {
            try
            {
                // Получаем индексы элементов для анимации
                var indices = GetElementIndices();
                
                if (indices.Length == 0)
                {
                    Debug.LogWarning("[AnimationTrigger] No elements selected for animation!");
                    return;
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"[AnimationTrigger] Triggering {animationType} animation for {indices.Length} elements");
                }
                
                // Сохраняем текущие настройки контроллера
                var originalMode = animationController.GetCurrentAnimationMode();
                var originalDelay = animationController.GetCurrentStaggerDelay();
                
                // Применяем кастомные настройки если нужно
                if (useCustomSettings)
                {
                    animationController.SetAnimationMode(customAnimationMode);
                    animationController.SetStaggerDelay(customStaggerDelay);
                }
                
                // Выполняем анимацию в зависимости от типа
                switch (animationType)
                {
                    case AnimationType.Show:
                        await AnimateElementsShow(indices);
                        break;
                    case AnimationType.Hide:
                        await AnimateElementsHide(indices);
                        break;
                    case AnimationType.Toggle:
                        await AnimateElementsToggle(indices);
                        break;
                }
                
                // Восстанавливаем оригинальные настройки
                if (useCustomSettings)
                {
                    animationController.SetAnimationMode(originalMode);
                    animationController.SetStaggerDelay(originalDelay);
                }
                
                onEndAnimation.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AnimationTrigger] Error during animation: {e.Message}");
            }
        }
        
        /// <summary>
        /// Получение индексов элементов в зависимости от режима выбора
        /// </summary>
        private int[] GetElementIndices()
        {
            switch (selectionMode)
            {
                case ElementSelectionMode.ByIndices:
                    return elementIndices;
                    
                case ElementSelectionMode.ByRange:
                    var rangeIndices = new System.Collections.Generic.List<int>();
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        if (i >= 0 && i < animationController.GetElementCount())
                        {
                            rangeIndices.Add(i);
                        }
                    }
                    return rangeIndices.ToArray();
                    
                case ElementSelectionMode.AllElements:
                    var allIndices = new int[animationController.GetElementCount()];
                    for (int i = 0; i < allIndices.Length; i++)
                    {
                        allIndices[i] = reverseOrder ? allIndices.Length - 1 - i : i;
                    }
                    return allIndices;
                    
                default:
                    return new int[0];
            }
        }
        
        /// <summary>
        /// Анимация показа выбранных элементов
        /// </summary>
        private async UniTask AnimateElementsShow(int[] indices)
        {
            // Создаем временный контроллер только для выбранных элементов
            var tempController = CreateTemporaryController(indices);
            
            try
            {
                await tempController.ShowCanvasAsync();
            }
            finally
            {
                // Очищаем временный контроллер
                DestroyImmediate(tempController.gameObject);
            }
        }
        
        /// <summary>
        /// Анимация скрытия выбранных элементов
        /// </summary>
        private async UniTask AnimateElementsHide(int[] indices)
        {
            // Создаем временный контроллер только для выбранных элементов
            var tempController = CreateTemporaryController(indices);
            
            try
            {
                await tempController.HideCanvasAsync();
            }
            finally
            {
                // Очищаем временный контроллер
                DestroyImmediate(tempController.gameObject);
            }
        }
        
        /// <summary>
        /// Переключение анимации выбранных элементов
        /// </summary>
        private async UniTask AnimateElementsToggle(int[] indices)
        {
            // Проверяем, видимы ли элементы
            bool anyVisible = false;
            foreach (int index in indices)
            {
                var element = animationController.GetElement(index);
                if (element != null && element.element.gameObject.activeInHierarchy)
                {
                    anyVisible = true;
                    break;
                }
            }
            
            if (anyVisible)
            {
                await AnimateElementsHide(indices);
            }
            else
            {
                await AnimateElementsShow(indices);
            }
        }
        
        /// <summary>
        /// Создание временного контроллера для выбранных элементов
        /// </summary>
        private CanvasAnimationController CreateTemporaryController(int[] indices)
        {
            // Создаем временный GameObject
            var tempGO = new GameObject("TempAnimationController");
            tempGO.transform.SetParent(animationController.transform);
            
            // Добавляем временный контроллер
            var tempController = tempGO.AddComponent<CanvasAnimationController>();
            
            // Копируем настройки из основного контроллера
            tempController.SetAnimationMode(useCustomSettings ? customAnimationMode : animationController.GetCurrentAnimationMode());
            tempController.SetStaggerDelay(useCustomSettings ? customStaggerDelay : animationController.GetCurrentStaggerDelay());
            
            // Добавляем только выбранные элементы
            foreach (int index in indices)
            {
                var element = animationController.GetElement(index);
                if (element != null)
                {
                    // Создаем копию настроек элемента
                    var elementCopy = new CanvasAnimationController.UIElementAnimation
                    {
                        element = element.element,
                        animationDuration = useCustomSettings ? customDuration : element.animationDuration,
                        animationEase = useCustomSettings ? customEase : element.animationEase,
                        delay = element.delay,
                        slideDirection = element.slideDirection,
                        slideDistance = element.slideDistance,
                        enableSlideAnimation = element.enableSlideAnimation,
                        enableFadeAnimation = element.enableFadeAnimation,
                        minAlpha = element.minAlpha,
                        maxAlpha = element.maxAlpha,
                        enableScaleAnimation = element.enableScaleAnimation,
                        minScale = element.minScale,
                        maxScale = element.maxScale,
                        animateOnCanvasShow = element.animateOnCanvasShow,
                        animateOnCanvasHide = element.animateOnCanvasHide
                    };
                    
                    tempController.AddElement(element.element, elementCopy);
                }
            }
            
            return tempController;
        }
        
        /// <summary>
        /// Установка контроллера анимаций
        /// </summary>
        public void SetAnimationController(CanvasAnimationController controller)
        {
            animationController = controller;
        }
        
        /// <summary>
        /// Установка индексов элементов
        /// </summary>
        public void SetElementIndices(int[] indices)
        {
            elementIndices = indices;
        }
        
        /// <summary>
        /// Установка диапазона элементов
        /// </summary>
        public void SetElementRange(int start, int end)
        {
            startIndex = start;
            endIndex = end;
        }
        
        /// <summary>
        /// Установка типа анимации
        /// </summary>
        public void SetAnimationType(AnimationType type)
        {
            animationType = type;
        }
        
        /// <summary>
        /// Установка режима выбора элементов
        /// </summary>
        public void SetSelectionMode(ElementSelectionMode mode)
        {
            selectionMode = mode;
        }
        
        #region Debug Methods
        
        [Button("Print Current Settings")]
        private void DebugPrintSettings()
        {
            if (!Application.isPlaying) return;
            
            Debug.Log($"[AnimationTrigger] Current Settings:");
            Debug.Log($"- Animation Type: {animationType}");
            Debug.Log($"- Selection Mode: {selectionMode}");
            Debug.Log($"- Use Custom Settings: {useCustomSettings}");
            
            switch (selectionMode)
            {
                case ElementSelectionMode.ByIndices:
                    Debug.Log($"- Element Indices: [{string.Join(", ", elementIndices)}]");
                    break;
                case ElementSelectionMode.ByRange:
                    Debug.Log($"- Range: {startIndex} to {endIndex}");
                    break;
                case ElementSelectionMode.AllElements:
                    Debug.Log($"- Reverse Order: {reverseOrder}");
                    break;
            }
            
            if (useCustomSettings)
            {
                Debug.Log($"- Custom Duration: {customDuration}");
                Debug.Log($"- Custom Ease: {customEase}");
                Debug.Log($"- Custom Animation Mode: {customAnimationMode}");
                Debug.Log($"- Custom Stagger Delay: {customStaggerDelay}");
            }
            
            if (animationController != null)
            {
                Debug.Log($"- Controller Elements Count: {animationController.GetElementCount()}");
            }
            else
            {
                Debug.LogWarning("- No animation controller assigned!");
            }
        }
        
        [Button("Validate Settings")]
        private void DebugValidateSettings()
        {
            if (!Application.isPlaying) return;
            
            bool isValid = true;
            
            if (animationController == null)
            {
                Debug.LogError("[AnimationTrigger] No animation controller assigned!");
                isValid = false;
            }
            
            if (selectionMode == ElementSelectionMode.ByIndices)
            {
                foreach (int index in elementIndices)
                {
                    if (index < 0 || index >= animationController.GetElementCount())
                    {
                        Debug.LogError($"[AnimationTrigger] Invalid element index: {index}");
                        isValid = false;
                    }
                }
            }
            
            if (selectionMode == ElementSelectionMode.ByRange)
            {
                if (startIndex < 0 || endIndex < 0 || startIndex > endIndex)
                {
                    Debug.LogError($"[AnimationTrigger] Invalid range: {startIndex} to {endIndex}");
                    isValid = false;
                }
            }
            
            if (isValid)
            {
                Debug.Log("[AnimationTrigger] Settings are valid!");
            }
        }
        
        #endregion
    }
}


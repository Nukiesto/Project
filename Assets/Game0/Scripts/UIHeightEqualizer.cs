using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Скрипт для выравнивания объектов интерфейса по высоте.
/// Автоматически находит максимальную высоту среди всех объектов и устанавливает её для всех.
/// Отключает ContentSizeFitter после получения размеров.
/// </summary>
public class UIHeightEqualizer : MonoBehaviour
{
    [Header("Настройки выравнивания")]
    [Tooltip("Массив объектов интерфейса для выравнивания по высоте")]
    [SerializeField] private RectTransform[] uiObjects;
    
    [Tooltip("Выравнивать высоту при старте")]
    [SerializeField] private bool equalizeOnStart = true;
    
    [Tooltip("Выравнивать высоту при изменении размера объекта")]
    [SerializeField] private bool equalizeOnSizeChange = false;
    
    [Header("Дополнительные настройки")]
    [Tooltip("Минимальная высота для объектов (если 0, то не используется)")]
    [SerializeField] private float minHeight = 0f;
    
    [Tooltip("Максимальная высота для объектов (если 0, то не используется)")]
    [SerializeField] private float maxHeight = 0f;
    
    private float lastMaxHeight = 0f;
    private bool hasEqualized = false;
    private Dictionary<RectTransform, bool> originalContentSizeFitterStates = new Dictionary<RectTransform, bool>();
    
    void Start()
    {
        if (equalizeOnStart)
        {
            EqualizeHeight();
        }
    }
    
    void Update()
    {
        if (equalizeOnSizeChange)
        {
            CheckAndEqualizeHeight();
        }
    }
    
    /// <summary>
    /// Проверяет, изменилась ли максимальная высота и выравнивает при необходимости
    /// </summary>
    private void CheckAndEqualizeHeight()
    {
        float currentMaxHeight = GetMaxHeight();
        
        if (Mathf.Abs(currentMaxHeight - lastMaxHeight) > 0.01f)
        {
            EqualizeHeight();
            lastMaxHeight = currentMaxHeight;
        }
    }
    
    /// <summary>
    /// Выравнивает высоту всех объектов по максимальной высоте
    /// </summary>
    public void EqualizeHeight()
    {
        if (uiObjects == null || uiObjects.Length == 0)
        {
            Debug.LogWarning("UIHeightEqualizer: Массив объектов пуст!");
            return;
        }
        
        float maxHeight = GetMaxHeight();
        
        // Применяем ограничения по высоте
        if (this.minHeight > 0f && maxHeight < this.minHeight)
        {
            maxHeight = this.minHeight;
        }
        
        if (this.maxHeight > 0f && maxHeight > this.maxHeight)
        {
            maxHeight = this.maxHeight;
        }
        
        // Устанавливаем одинаковую высоту для всех объектов и отключаем ContentSizeFitter
        foreach (RectTransform rectTransform in uiObjects)
        {
            if (rectTransform != null)
            {
                Vector2 sizeDelta = rectTransform.sizeDelta;
                sizeDelta.y = maxHeight;
                rectTransform.sizeDelta = sizeDelta;
                
                // Отключаем ContentSizeFitter после установки размера
                DisableContentSizeFitter(rectTransform);
            }
        }
        
        lastMaxHeight = maxHeight;
        hasEqualized = true;
        
        Debug.Log($"UIHeightEqualizer: Высота всех объектов установлена на {maxHeight}");
    }
    
    /// <summary>
    /// Находит максимальную высоту среди всех объектов
    /// </summary>
    /// <returns>Максимальная высота</returns>
    private float GetMaxHeight()
    {
        float maxHeight = 0f;
        
        foreach (RectTransform rectTransform in uiObjects)
        {
            if (rectTransform != null)
            {
                float height = rectTransform.sizeDelta.y;
                if (height > maxHeight)
                {
                    maxHeight = height;
                }
            }
        }
        
        return maxHeight;
    }
    
    /// <summary>
    /// Отключает ContentSizeFitter на объекте и его дочерних элементах
    /// </summary>
    /// <param name="rectTransform">RectTransform объект</param>
    private void DisableContentSizeFitter(RectTransform rectTransform)
    {
        if (rectTransform == null) return;
        
        // Отключаем ContentSizeFitter на текущем объекте
        ContentSizeFitter contentSizeFitter = rectTransform.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            // Сохраняем исходное состояние
            if (!originalContentSizeFitterStates.ContainsKey(rectTransform))
            {
                originalContentSizeFitterStates[rectTransform] = contentSizeFitter.enabled;
            }
            
            contentSizeFitter.enabled = false;
            Debug.Log($"UIHeightEqualizer: ContentSizeFitter отключен на {rectTransform.name}");
        }
        
        // Отключаем ContentSizeFitter на всех дочерних объектах
        ContentSizeFitter[] childContentSizeFitters = rectTransform.GetComponentsInChildren<ContentSizeFitter>();
        foreach (ContentSizeFitter childFitter in childContentSizeFitters)
        {
            if (childFitter != contentSizeFitter) // Избегаем повторного отключения
            {
                // Сохраняем исходное состояние для дочерних объектов
                RectTransform childRectTransform = childFitter.GetComponent<RectTransform>();
                if (childRectTransform != null && !originalContentSizeFitterStates.ContainsKey(childRectTransform))
                {
                    originalContentSizeFitterStates[childRectTransform] = childFitter.enabled;
                }
                
                childFitter.enabled = false;
                Debug.Log($"UIHeightEqualizer: ContentSizeFitter отключен на дочернем объекте {childFitter.name}");
            }
        }
    }
    
    /// <summary>
    /// Устанавливает массив объектов для выравнивания
    /// </summary>
    /// <param name="objects">Массив RectTransform объектов</param>
    public void SetUIObjects(RectTransform[] objects)
    {
        uiObjects = objects;
    }
    
    /// <summary>
    /// Добавляет объект в массив для выравнивания
    /// </summary>
    /// <param name="uiObject">RectTransform объект</param>
    public void AddUIObject(RectTransform uiObject)
    {
        if (uiObject == null) return;
        
        // Создаем новый массив с дополнительным элементом
        RectTransform[] newArray = new RectTransform[uiObjects.Length + 1];
        
        // Копируем существующие элементы
        for (int i = 0; i < uiObjects.Length; i++)
        {
            newArray[i] = uiObjects[i];
        }
        
        // Добавляем новый элемент
        newArray[uiObjects.Length] = uiObject;
        
        uiObjects = newArray;
    }
    
    /// <summary>
    /// Удаляет объект из массива выравнивания
    /// </summary>
    /// <param name="uiObject">RectTransform объект для удаления</param>
    public void RemoveUIObject(RectTransform uiObject)
    {
        if (uiObject == null || uiObjects == null) return;
        
        // Находим индекс объекта
        int indexToRemove = -1;
        for (int i = 0; i < uiObjects.Length; i++)
        {
            if (uiObjects[i] == uiObject)
            {
                indexToRemove = i;
                break;
            }
        }
        
        if (indexToRemove == -1) return;
        
        // Создаем новый массив без удаляемого элемента
        RectTransform[] newArray = new RectTransform[uiObjects.Length - 1];
        
        for (int i = 0, j = 0; i < uiObjects.Length; i++)
        {
            if (i != indexToRemove)
            {
                newArray[j] = uiObjects[i];
                j++;
            }
        }
        
        uiObjects = newArray;
    }
    
    /// <summary>
    /// Очищает массив объектов
    /// </summary>
    public void ClearUIObjects()
    {
        uiObjects = new RectTransform[0];
    }
    
    /// <summary>
    /// Получает текущую максимальную высоту
    /// </summary>
    /// <returns>Текущая максимальная высота</returns>
    public float GetCurrentMaxHeight()
    {
        return GetMaxHeight();
    }
    
    /// <summary>
    /// Устанавливает минимальную высоту
    /// </summary>
    /// <param name="minHeight">Минимальная высота</param>
    public void SetMinHeight(float minHeight)
    {
        this.minHeight = minHeight;
    }
    
    /// <summary>
    /// Устанавливает максимальную высоту
    /// </summary>
    /// <param name="maxHeight">Максимальная высота</param>
    public void SetMaxHeight(float maxHeight)
    {
        this.maxHeight = maxHeight;
    }
    
    /// <summary>
    /// Проверяет, было ли выполнено выравнивание
    /// </summary>
    /// <returns>true если выравнивание было выполнено</returns>
    public bool HasEqualized()
    {
        return hasEqualized;
    }
    
    /// <summary>
    /// Принудительно отключает ContentSizeFitter на всех объектах
    /// </summary>
    public void ForceDisableContentSizeFitters()
    {
        if (uiObjects == null || uiObjects.Length == 0)
        {
            Debug.LogWarning("UIHeightEqualizer: Массив объектов пуст!");
            return;
        }
        
        foreach (RectTransform rectTransform in uiObjects)
        {
            if (rectTransform != null)
            {
                DisableContentSizeFitter(rectTransform);
            }
        }
        
        Debug.Log("UIHeightEqualizer: ContentSizeFitter отключен на всех объектах");
    }
    
    /// <summary>
    /// Включает ContentSizeFitter обратно на всех объектах
    /// </summary>
    public void RestoreContentSizeFitters()
    {
        if (uiObjects == null || uiObjects.Length == 0)
        {
            Debug.LogWarning("UIHeightEqualizer: Массив объектов пуст!");
            return;
        }
        
        foreach (RectTransform rectTransform in uiObjects)
        {
            if (rectTransform != null)
            {
                RestoreContentSizeFitter(rectTransform);
            }
        }
        
        Debug.Log("UIHeightEqualizer: ContentSizeFitter восстановлен на всех объектах");
    }
    
    /// <summary>
    /// Восстанавливает ContentSizeFitter на объекте и его дочерних элементах
    /// </summary>
    /// <param name="rectTransform">RectTransform объект</param>
    private void RestoreContentSizeFitter(RectTransform rectTransform)
    {
        if (rectTransform == null) return;
        
        // Восстанавливаем ContentSizeFitter на текущем объекте
        ContentSizeFitter contentSizeFitter = rectTransform.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null && originalContentSizeFitterStates.ContainsKey(rectTransform))
        {
            contentSizeFitter.enabled = originalContentSizeFitterStates[rectTransform];
            Debug.Log($"UIHeightEqualizer: ContentSizeFitter восстановлен на {rectTransform.name} (состояние: {contentSizeFitter.enabled})");
        }
        
        // Восстанавливаем ContentSizeFitter на всех дочерних объектах
        ContentSizeFitter[] childContentSizeFitters = rectTransform.GetComponentsInChildren<ContentSizeFitter>();
        foreach (ContentSizeFitter childFitter in childContentSizeFitters)
        {
            if (childFitter != contentSizeFitter) // Избегаем повторного восстановления
            {
                RectTransform childRectTransform = childFitter.GetComponent<RectTransform>();
                if (childRectTransform != null && originalContentSizeFitterStates.ContainsKey(childRectTransform))
                {
                    childFitter.enabled = originalContentSizeFitterStates[childRectTransform];
                    Debug.Log($"UIHeightEqualizer: ContentSizeFitter восстановлен на дочернем объекте {childFitter.name} (состояние: {childFitter.enabled})");
                }
            }
        }
    }
    
    /// <summary>
    /// Очищает сохраненные состояния ContentSizeFitter
    /// </summary>
    public void ClearContentSizeFitterStates()
    {
        originalContentSizeFitterStates.Clear();
        Debug.Log("UIHeightEqualizer: Сохраненные состояния ContentSizeFitter очищены");
    }
    
    void OnDisable()
    {
        // Восстанавливаем ContentSizeFitter при выключении объекта
        RestoreContentSizeFitters();
    }
    
    void OnDestroy()
    {
        // Восстанавливаем ContentSizeFitter при уничтожении объекта
        RestoreContentSizeFitters();
    }
}

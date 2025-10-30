using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Selectable))]
public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float scaleDownDuration = 0.1f;
    [SerializeField] private float scaleUpDuration = 0.1f;
    [SerializeField] private float scaleDownValue = 0.95f;
    [SerializeField] private float scaleUpValue = 1f;
    [SerializeField] private Ease scaleDownEase = Ease.OutQuad;
    [SerializeField] private Ease scaleUpEase = Ease.OutBack;
    
    [Header("Optional Effects")]
    [SerializeField] private bool enableColorChange = false;
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private float colorChangeDuration = 0.1f;
    
    private Selectable selectable;
    private Image selectableImage;
    private Vector3 originalScale;
    private Color originalColor;
    private Sequence currentAnimation;
    
    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        selectableImage = GetComponent<Image>();
        
        // Сохраняем оригинальные значения
        originalScale = transform.localScale;
        if (selectableImage != null)
        {
            originalColor = selectableImage.color;
        }
    }
    
    private void OnDestroy()
    {
        // Очищаем анимации при уничтожении объекта
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        AnimateButtonPress();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        AnimateButtonRelease();
    }
    
    private void AnimateButtonPress()
    {
        // Останавливаем текущую анимацию
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        // Создаем новую последовательность анимаций
        currentAnimation = DOTween.Sequence();
        
        // Анимация масштабирования при нажатии
        currentAnimation.Append(transform.DOScale(originalScale * scaleDownValue, scaleDownDuration)
            .SetEase(scaleDownEase));
        
        // Опциональная анимация цвета
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(pressedColor, colorChangeDuration));
        }
    }
    
    private void AnimateButtonRelease()
    {
        // Останавливаем текущую анимацию
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        // Создаем новую последовательность анимаций
        currentAnimation = DOTween.Sequence();
        
        // Анимация масштабирования при отпускании
        currentAnimation.Append(transform.DOScale(originalScale * scaleUpValue, scaleUpDuration)
            .SetEase(scaleUpEase));
        
        // Возвращаем к оригинальному масштабу
        currentAnimation.Append(transform.DOScale(originalScale, scaleUpDuration * 0.5f)
            .SetEase(Ease.OutQuad));
        
        // Опциональная анимация цвета
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(originalColor, colorChangeDuration));
        }
    }
    
    // Публичные методы для ручного управления анимацией
    public void AnimatePress()
    {
        AnimateButtonPress();
    }
    
    public void AnimateRelease()
    {
        AnimateButtonRelease();
    }
    
    // Метод для сброса к исходному состоянию
    public void ResetToOriginalState()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        transform.localScale = originalScale;
        if (selectableImage != null)
        {
            selectableImage.color = originalColor;
        }
    }
    
    // Метод для настройки параметров анимации во время выполнения
    public void SetAnimationParameters(float scaleDownDur, float scaleUpDur, float scaleDownVal, float scaleUpVal)
    {
        scaleDownDuration = scaleDownDur;
        scaleUpDuration = scaleUpDur;
        scaleDownValue = scaleDownVal;
        scaleUpValue = scaleUpVal;
    }
}

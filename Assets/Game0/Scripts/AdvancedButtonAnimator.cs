using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Selectable))]
public class AdvancedButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Animation")]
    [SerializeField] private float scaleDownDuration = 0.1f;
    [SerializeField] private float scaleUpDuration = 0.15f;
    [SerializeField] private float scaleDownValue = 0.95f;
    [SerializeField] private float scaleUpValue = 1.05f;
    [SerializeField] private Ease scaleDownEase = Ease.OutQuad;
    [SerializeField] private Ease scaleUpEase = Ease.OutBack;
    
    [Header("Color Animation")]
    [SerializeField] private bool enableColorChange = true;
    [SerializeField] private Color hoverColor = new Color(1.1f, 1.1f, 1.1f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    [SerializeField] private float colorChangeDuration = 0.1f;
    
    [Header("Rotation Animation")]
    [SerializeField] private bool enableRotation = false;
    [SerializeField] private float rotationAngle = 5f;
    [SerializeField] private float rotationDuration = 0.1f;
    
    [Header("Sound Effects")]
    [SerializeField] private bool enableSound = false;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip hoverSound;
    
    private Selectable selectable;
    private Image selectableImage;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private Color originalColor;
    private Sequence currentAnimation;
    private bool isHovered = false;
    
    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        selectableImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        
        // Добавляем AudioSource если его нет и включены звуки
        if (enableSound && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Сохраняем оригинальные значения
        originalScale = transform.localScale;
        originalRotation = transform.localEulerAngles;
        if (selectableImage != null)
        {
            originalColor = selectableImage.color;
        }
    }
    
    private void OnDestroy()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        isHovered = true;
        AnimateHoverEnter();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        isHovered = false;
        AnimateHoverExit();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        PlaySound(pressSound);
        AnimateButtonPress();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selectable.interactable) return;
        
        PlaySound(releaseSound);
        AnimateButtonRelease();
    }
    
    private void AnimateHoverEnter()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        currentAnimation = DOTween.Sequence();
        
        // Легкое увеличение при наведении
        currentAnimation.Append(transform.DOScale(originalScale * 1.02f, 0.1f)
            .SetEase(Ease.OutQuad));
        
        // Изменение цвета при наведении
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(hoverColor, colorChangeDuration));
        }
        
        // Легкий поворот при наведении
        if (enableRotation)
        {
            currentAnimation.Join(transform.DORotate(originalRotation + Vector3.forward * rotationAngle * 0.5f, rotationDuration)
                .SetEase(Ease.OutQuad));
        }
        
        PlaySound(hoverSound);
    }
    
    private void AnimateHoverExit()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        currentAnimation = DOTween.Sequence();
        
        // Возврат к оригинальному размеру
        currentAnimation.Append(transform.DOScale(originalScale, 0.1f)
            .SetEase(Ease.OutQuad));
        
        // Возврат к оригинальному цвету
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(originalColor, colorChangeDuration));
        }
        
        // Возврат к оригинальному повороту
        if (enableRotation)
        {
            currentAnimation.Join(transform.DORotate(originalRotation, rotationDuration)
                .SetEase(Ease.OutQuad));
        }
    }
    
    private void AnimateButtonPress()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        currentAnimation = DOTween.Sequence();
        
        // Уменьшение при нажатии
        currentAnimation.Append(transform.DOScale(originalScale * scaleDownValue, scaleDownDuration)
            .SetEase(scaleDownEase));
        
        // Изменение цвета при нажатии
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(pressedColor, colorChangeDuration));
        }
        
        // Поворот при нажатии
        if (enableRotation)
        {
            currentAnimation.Join(transform.DORotate(originalRotation + Vector3.forward * rotationAngle, rotationDuration)
                .SetEase(scaleDownEase));
        }
    }
    
    private void AnimateButtonRelease()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        currentAnimation = DOTween.Sequence();
        
        // Увеличение при отпускании
        currentAnimation.Append(transform.DOScale(originalScale * scaleUpValue, scaleUpDuration)
            .SetEase(scaleUpEase));
        
        // Возврат к оригинальному размеру
        currentAnimation.Append(transform.DOScale(originalScale, scaleUpDuration * 0.5f)
            .SetEase(Ease.OutQuad));
        
        // Возврат к оригинальному цвету или цвету при наведении
        Color targetColor = isHovered ? hoverColor : originalColor;
        if (enableColorChange && selectableImage != null)
        {
            currentAnimation.Join(selectableImage.DOColor(targetColor, colorChangeDuration));
        }
        
        // Возврат к оригинальному повороту или повороту при наведении
        Vector3 targetRotation = isHovered ? originalRotation + Vector3.forward * rotationAngle * 0.5f : originalRotation;
        if (enableRotation)
        {
            currentAnimation.Join(transform.DORotate(targetRotation, rotationDuration)
                .SetEase(Ease.OutQuad));
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (enableSound && audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Публичные методы для ручного управления
    public void TriggerPressAnimation()
    {
        AnimateButtonPress();
    }
    
    public void TriggerReleaseAnimation()
    {
        AnimateButtonRelease();
    }
    
    public void SetHoverState(bool hovered)
    {
        isHovered = hovered;
        if (hovered)
        {
            AnimateHoverEnter();
        }
        else
        {
            AnimateHoverExit();
        }
    }
    
    public void ResetToOriginalState()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill();
        }
        
        transform.localScale = originalScale;
        transform.localEulerAngles = originalRotation;
        if (selectableImage != null)
        {
            selectableImage.color = originalColor;
        }
        isHovered = false;
    }
}

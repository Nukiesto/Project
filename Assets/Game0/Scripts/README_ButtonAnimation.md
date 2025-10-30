# Анимация Selectable объектов с DoTween

Этот набор скриптов предоставляет готовые решения для анимации любых Selectable объектов в Unity с использованием DoTween.

## Скрипты

### 1. ButtonAnimator.cs
Базовый скрипт для простой анимации Selectable объекта при нажатии и отпускании.

**Возможности:**
- Анимация масштабирования при нажатии/отпускании
- Опциональное изменение цвета
- Настраиваемые параметры анимации
- Автоматическая очистка ресурсов
- Работает с любыми Selectable объектами (Button, Toggle, Slider, Dropdown и т.д.)

**Использование:**
1. Добавьте компонент `ButtonAnimator` к объекту с компонентом `Selectable`
2. Настройте параметры в инспекторе:
   - `Scale Down Duration` - длительность уменьшения
   - `Scale Up Duration` - длительность увеличения
   - `Scale Down Value` - значение уменьшения (0.95 = 95% от оригинала)
   - `Scale Up Value` - значение увеличения (1.0 = 100% от оригинала)
   - `Enable Color Change` - включить изменение цвета
   - `Pressed Color` - цвет при нажатии

### 2. AdvancedButtonAnimator.cs
Продвинутый скрипт с дополнительными эффектами.

**Дополнительные возможности:**
- Анимация при наведении курсора
- Анимация поворота
- Звуковые эффекты
- Более сложные последовательности анимаций
- Работает с любыми Selectable объектами

**Использование:**
1. Добавьте компонент `AdvancedButtonAnimator` к объекту с компонентом `Selectable`
2. Настройте параметры в инспекторе:
   - **Scale Animation** - настройки масштабирования
   - **Color Animation** - настройки изменения цвета
   - **Rotation Animation** - настройки поворота
   - **Sound Effects** - настройки звуков

## Поддерживаемые Selectable объекты

Скрипты работают со всеми объектами, наследующими от `Selectable`:
- **Button** - кнопки
- **Toggle** - переключатели
- **Slider** - ползунки
- **Dropdown** - выпадающие списки
- **InputField** - поля ввода
- **Scrollbar** - полосы прокрутки
- **ScrollRect** - области прокрутки
- **Custom Selectable** - пользовательские Selectable объекты

## Установка и настройка

### Требования
- Unity 2019.4 или новее
- DoTween (уже установлен в проекте)

### Пошаговая настройка

1. **Создание Selectable объекта:**
   ```
   GameObject → UI → Button/Toggle/Slider/etc.
   ```

2. **Добавление аниматора:**
   - Выберите Selectable объект в иерархии
   - В инспекторе нажмите "Add Component"
   - Найдите и добавьте `ButtonAnimator` или `AdvancedButtonAnimator`

3. **Настройка параметров:**
   - Откройте компонент аниматора в инспекторе
   - Настройте параметры под ваши нужды
   - Для звуков добавьте AudioClip в соответствующие поля

4. **Тестирование:**
   - Запустите сцену
   - Взаимодействуйте с объектом для проверки анимации

## Параметры настройки

### ButtonAnimator
- `Scale Down Duration` (0.1f) - длительность уменьшения в секундах
- `Scale Up Duration` (0.1f) - длительность увеличения в секундах
- `Scale Down Value` (0.95f) - коэффициент уменьшения
- `Scale Up Value` (1.0f) - коэффициент увеличения
- `Scale Down Ease` (OutQuad) - тип плавности уменьшения
- `Scale Up Ease` (OutBack) - тип плавности увеличения
- `Enable Color Change` (false) - включить изменение цвета
- `Pressed Color` (0.8, 0.8, 0.8, 1) - цвет при нажатии
- `Color Change Duration` (0.1f) - длительность изменения цвета

### AdvancedButtonAnimator
Дополнительно к параметрам ButtonAnimator:
- `Hover Color` (1.1, 1.1, 1.1, 1) - цвет при наведении
- `Enable Rotation` (false) - включить поворот
- `Rotation Angle` (5f) - угол поворота в градусах
- `Rotation Duration` (0.1f) - длительность поворота
- `Enable Sound` (false) - включить звуки
- `Press Sound` - звук при нажатии
- `Release Sound` - звук при отпускании
- `Hover Sound` - звук при наведении

## Публичные методы

### ButtonAnimator
- `AnimatePress()` - запустить анимацию нажатия
- `AnimateRelease()` - запустить анимацию отпускания
- `ResetToOriginalState()` - сбросить к исходному состоянию
- `SetAnimationParameters()` - изменить параметры во время выполнения

### AdvancedButtonAnimator
- `TriggerPressAnimation()` - запустить анимацию нажатия
- `TriggerReleaseAnimation()` - запустить анимацию отпускания
- `SetHoverState(bool)` - установить состояние наведения
- `ResetToOriginalState()` - сбросить к исходному состоянию

## Примеры использования

### Программное управление анимацией
```csharp
ButtonAnimator animator = selectableObject.GetComponent<ButtonAnimator>();
animator.AnimatePress(); // Запустить анимацию нажатия
```

### Изменение параметров во время выполнения
```csharp
ButtonAnimator animator = selectableObject.GetComponent<ButtonAnimator>();
animator.SetAnimationParameters(0.2f, 0.2f, 0.9f, 1.1f);
```

### Управление состоянием наведения
```csharp
AdvancedButtonAnimator animator = selectableObject.GetComponent<AdvancedButtonAnimator>();
animator.SetHoverState(true); // Симулировать наведение
```

### Применение к различным объектам
```csharp
// Для кнопки
Button button = GetComponent<Button>();
ButtonAnimator buttonAnimator = button.gameObject.AddComponent<ButtonAnimator>();

// Для переключателя
Toggle toggle = GetComponent<Toggle>();
AdvancedButtonAnimator toggleAnimator = toggle.gameObject.AddComponent<AdvancedButtonAnimator>();

// Для ползунка
Slider slider = GetComponent<Slider>();
ButtonAnimator sliderAnimator = slider.gameObject.AddComponent<ButtonAnimator>();
```

## Советы по использованию

1. **Производительность:** Используйте `ButtonAnimator` для простых случаев, `AdvancedButtonAnimator` для более сложных эффектов.

2. **Звуки:** Для лучшего пользовательского опыта используйте короткие звуки (0.1-0.3 секунды).

3. **Цвета:** Используйте цвета с альфа-каналом 1.0 для лучшей совместимости.

4. **Масштабирование:** Рекомендуемые значения:
   - Уменьшение: 0.9-0.95
   - Увеличение: 1.0-1.05

5. **Длительность:** Оптимальная длительность анимаций: 0.1-0.2 секунды.

6. **Selectable объекты:** Убедитесь, что объект имеет компонент Image для работы с изменением цвета.

## Устранение неполадок

### Объект не анимируется
- Убедитесь, что DoTween установлен
- Проверьте, что компонент Selectable присутствует
- Убедитесь, что объект интерактивен (Interactable = true)

### Анимация прерывается
- Проверьте настройки Ease
- Убедитесь, что нет конфликтующих анимаций
- Проверьте, что объект не уничтожается во время анимации

### Звуки не воспроизводятся
- Убедитесь, что AudioSource добавлен
- Проверьте, что AudioClip назначен
- Убедитесь, что звук не отключен в настройках

### Изменение цвета не работает
- Убедитесь, что объект имеет компонент Image
- Проверьте, что включена опция "Enable Color Change"
- Убедитесь, что Image не заблокирован другими скриптами

## Лицензия

Эти скрипты предоставляются как есть для использования в ваших проектах.

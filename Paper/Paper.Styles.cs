using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.CompilerServices;

using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;

namespace Prowl.PaperUI;

/// <summary>
///     Defines all available style properties for UI elements.
/// </summary>
public enum GuiProp
{
    #region Visual Properties

    BackgroundColor,
    BackgroundGradient,
    BorderColor,
    BorderWidth,
    Rounded,
    BoxShadow,

    #endregion

    #region Layout Properties

    // Core sizing
    AspectRatio,
    Width,
    Height,
    MinWidth,
    MaxWidth,
    MinHeight,
    MaxHeight,

    // Positioning
    Left,
    Right,
    Top,
    Bottom,
    MinLeft,
    MaxLeft,
    MinRight,
    MaxRight,
    MinTop,
    MaxTop,
    MinBottom,
    MaxBottom,

    // Child layout
    ChildLeft,
    ChildRight,
    ChildTop,
    ChildBottom,

    // Spacing
    RowBetween,
    ColBetween,

    // Border spacing
    BorderLeft,
    BorderRight,
    BorderTop,
    BorderBottom,

    #endregion

    #region Transform Properties

    TranslateX,
    TranslateY,
    ScaleX,
    ScaleY,
    Rotate,
    OriginX,
    OriginY,
    SkewX,
    SkewY,
    Transform,

    #endregion

    #region Text Properties

    TextColor,

    WordSpacing,
    LetterSpacing,
    LineHeight,

    TabSize,
    FontSize,

    #endregion
}

/// <summary>
///     Builds transformation matrices for UI elements.
/// </summary>
public class TransformBuilder
{
    #region Build Method

    /// <summary>
    ///     Builds the final transform matrix following the order: translate, rotate, scale, skew.
    /// </summary>
    /// <param name="rect">The rectangle to transform.</param>
    /// <returns>The complete transformation matrix.</returns>
    public Transform2D Build(Rect rect)
    {
        // Calculate origin in actual pixels
        double originX = rect.x + _originX * rect.width;
        double originY = rect.y + _originY * rect.height;

        // Create transformation matrix
        Transform2D result = Transform2D.Identity;

        // Create a matrix that transforms from origin
        Transform2D originMatrix = Transform2D.CreateTranslation(-originX, -originY);

        // Apply transforms in order: translate, rotate, scale, skew
        Transform2D transformMatrix = Transform2D.Identity;

        // 1. Translate
        if (_translateX != 0 || _translateY != 0)
        {
            transformMatrix *= Transform2D.CreateTranslation(_translateX, _translateY);
        }

        // 2. Rotate
        if (_rotate != 0)
        {
            transformMatrix *= Transform2D.CreateRotate(_rotate);
        }

        // 3. Scale
        if (_scaleX != 1 || _scaleY != 1)
        {
            transformMatrix *= Transform2D.CreateScale(_scaleX, _scaleY);
        }

        // 4. Skew
        if (_skewX != 0)
        {
            transformMatrix *= Transform2D.CreateSkewX(_skewX);
        }

        if (_skewY != 0)
        {
            transformMatrix *= Transform2D.CreateSkewY(_skewY);
        }

        // 5. Apply custom transform if specified
        if (_customTransform.HasValue)
        {
            transformMatrix *= _customTransform.Value;
        }

        // Complete transformation: move to origin, apply transform, move back from origin
        result = originMatrix * transformMatrix * Transform2D.CreateTranslation(originX, originY);

        return result;
    }

    #endregion

    #region Fields

    private double _translateX;
    private double _translateY;
    private double _scaleX = 1;
    private double _scaleY = 1;
    private double _rotate;
    private double _skewX;
    private double _skewY;
    private double _originX = 0.5f; // Default to center (50%)
    private double _originY = 0.5f; // Default to center (50%)
    private Transform2D? _customTransform;

    #endregion

    #region Public Methods
    public void Reset()
    {
        _translateX = 0;
        _translateY = 0;
        _scaleX = 1;
        _scaleY = 1;
        _rotate = 0;
        _skewX = 0;
        _skewY = 0;
        _originX = 0.5f;
        _originY = 0.5f;
    }
    #endregion

    #region Builder Methods

    /// <summary>
    ///     Sets the X translation.
    /// </summary>
    public TransformBuilder SetTranslateX(double x)
    {
        _translateX = x;
        return this;
    }

    /// <summary>
    ///     Sets the Y translation.
    /// </summary>
    public TransformBuilder SetTranslateY(double y)
    {
        _translateY = y;
        return this;
    }

    /// <summary>
    ///     Sets the X scale factor.
    /// </summary>
    public TransformBuilder SetScaleX(double x)
    {
        _scaleX = x;
        return this;
    }

    /// <summary>
    ///     Sets the Y scale factor.
    /// </summary>
    public TransformBuilder SetScaleY(double y)
    {
        _scaleY = y;
        return this;
    }

    /// <summary>
    ///     Sets the rotation angle in degrees.
    /// </summary>
    public TransformBuilder SetRotate(double angleInDegrees)
    {
        _rotate = angleInDegrees;
        return this;
    }

    /// <summary>
    ///     Sets the X skew angle.
    /// </summary>
    public TransformBuilder SetSkewX(double angle)
    {
        _skewX = angle;
        return this;
    }

    /// <summary>
    ///     Sets the Y skew angle.
    /// </summary>
    public TransformBuilder SetSkewY(double angle)
    {
        _skewY = angle;
        return this;
    }

    /// <summary>
    ///     Sets the X origin point (0-1 range).
    /// </summary>
    public TransformBuilder SetOriginX(double x)
    {
        _originX = x;
        return this;
    }

    /// <summary>
    ///     Sets the Y origin point (0-1 range).
    /// </summary>
    public TransformBuilder SetOriginY(double y)
    {
        _originY = y;
        return this;
    }

    /// <summary>
    ///     Sets a custom transform to be applied.
    /// </summary>
    public TransformBuilder SetCustomTransform(Transform2D transform)
    {
        _customTransform = transform;
        return this;
    }

    #endregion
}

/// <summary>
///     Configuration for property transitions.
/// </summary>
public class TransitionConfig
{
    /// <summary>Duration of the transition in seconds.</summary>
    public double Duration { get; set; }

    /// <summary>Optional easing function to control transition timing.</summary>
    public Func<double, double>? EasingFunction { get; set; }
}

/// <summary>
///     Manages styling and transitions for UI elements.
/// </summary>
internal class ElementStyle
{
    #region Nested Types

    private interface IInterpolationState
    {
        GuiProp Property { get; }
        bool Update(double deltaTime, ref GuiProperties currentValue, List<GuiProp> completedInterpolations);
    }

    /// <summary>
    ///     Helper class to track interpolation state.
    /// </summary>
    /// TODO this still causes boxing and unboxing because the interpolation state
    /// is converting everything to an object. We need to change this, but then we also need to update the
    /// Dictionary.
    /// I think a generic struct to store the information would be best, then we can just have a single object that
    /// we can collect all of our style type information from.
    private class InterpolationState<T> : IInterpolationState
    {
        public GuiProp Property { get; set; }
        public T StartValue { get; set; }
        public T TargetValue { get; set; }
        public double Duration { get; set; }
        public Func<double, double>? EasingFunction { get; set; }
        public double CurrentTime { get; set; }

        public bool Update(double deltaTime, ref GuiProperties currentValue, List<GuiProp> completedInterpolations)
        {
            CurrentTime += deltaTime;
            if (CurrentTime >= Duration)
            {
                StyleUtils.SetValueInStruct(Property, ref currentValue, TargetValue);
                return true; // complete
            }

            double t = Math.Min(1.0, CurrentTime / Duration);
            if (EasingFunction != null) t = EasingFunction(t);

            StyleUtils.SetValueInStruct(Property, ref currentValue, StyleUtils.Interpolate(StartValue, TargetValue, t));
            return false;
        }
    }

    #endregion

    #region Fields

    private static readonly GuiProperties _defaultValues = new();
    private GuiProperties _currentGuiValues = new();
    public GuiProperties Properties => _currentGuiValues;
    private GuiProperties _targetGuiValues = new();

    // State tracking
    private readonly HashSet<GuiProp> _propertiesSetThisFrame = new();
    private readonly HashSet<GuiProp> _propertiesWithTransitions = new();
    private bool _firstFrame = true;

    // Property values
    private HashSet<GuiProp> _currentValues = new();
    private HashSet<GuiProp> _targetValues = new();

    // Transition state
    private readonly Dictionary<GuiProp, TransitionConfig> _transitionConfigs = new();
    private readonly Dictionary<GuiProp, IInterpolationState> _interpolations = new();

    private static TransformBuilder _transformBuilder = new();

    // private readonly Dictionary<GuiProp, InterpolationState> _interpolations = new();

    // Inheritance
    private ElementStyle? _parent;

    #endregion

    #region Public Methods

    public void ReturnToPool()
    {
        _propertiesSetThisFrame.Clear();
        _propertiesWithTransitions.Clear();

        //TODO understand how this works better
        _targetValues.Clear();
        _currentValues.Clear();

        _currentGuiValues.SetDefaultValues();
        _targetGuiValues.SetDefaultValues();

        _interpolations.Clear();
    }

    /// <summary>
    ///     Marks the end of a frame, resetting per-frame state.
    /// </summary>
    public void EndOfFrame()
    {
        _propertiesSetThisFrame.Clear();
        _firstFrame = false;
    }

    /// <summary>
    ///     Sets the parent style for inheritance.
    /// </summary>
    public void SetParent(ElementStyle? currentStyle) => _parent = currentStyle;

    /// <summary>
    ///     Checks if a property has a value.
    /// </summary>
    public bool HasValue(GuiProp property) => _currentValues.Contains(property);

    /// <summary>
    ///     Gets the current value of a property, falling back to parent or default.
    /// </summary>
    public T GetValue<T>(GuiProp property)
    {
        // If we have the value, return it
        if (HasValue(property))
        {
            return StyleUtils.GetValueFromStruct<T>(property, _currentGuiValues);
        }

        // Otherwise check parent
        if (_parent != null)
        {
            return _parent.GetValue<T>(property);
        }

        // Otherwise return default
        return StyleUtils.GetValueFromStruct<T>(property, _defaultValues);
    }

    /// <summary>
    ///     Sets a property value directly without transition.
    /// </summary>
    public void SetDirectValue<T>(GuiProp property, T value)
    {
        _propertiesSetThisFrame.Add(property);

        StyleUtils.SetValueInStruct(property, ref _currentGuiValues, value);
        StyleUtils.SetValueInStruct(property, ref _targetGuiValues, value);
        _currentValues.Add(property);
        _targetValues.Add(property); // Ensure target matches current
        RemoveInterpolation(property);
    }

    /// <summary>
    ///     Sets a property's target value for transition.
    /// </summary>
    public void SetNextValue<T>(GuiProp property, T value)
    {
        _propertiesSetThisFrame.Add(property);
        // _targetValues.Add(property);
        // Store the target value - this is where we want to end up
        StyleUtils.SetValueInStruct(property, ref _targetGuiValues, value);
        _targetValues.Add(property);
    }

    /// <summary>
    ///     Configures a transition for a property.
    /// </summary>
    public void SetTransitionConfig(GuiProp property, double duration, Func<double, double>? easing = null)
    {
        // Store the transition configuration for this property
        _transitionConfigs[property] = new TransitionConfig { Duration = duration, EasingFunction = easing };

        // Mark this property as having a transition
        _propertiesWithTransitions.Add(property);
    }

    /// <summary>
    ///     Removes a property value and any related transition state.
    /// </summary>
    public void ClearValue(GuiProp property)
    {
        _currentValues.Remove(property);
        _targetValues.Remove(property);
        _transitionConfigs.Remove(property);
        RemoveInterpolation(property);
    }

    /// <summary>
    ///     Updates all property transitions for the current frame.
    /// </summary>
    public void Update(double deltaTime)
    {
        if (!_firstFrame)
        {
            // Initialize values for properties with transitions
            InitializeTransitionProperties();
        }

        // Track completed transitions for cleanup
        List<GuiProp> completedInterpolations = new();

        // _currentValues = _targetValues;
        // Process all properties that have target values
        foreach (GuiProp property in _targetValues)
        {
            Type targetType = StyleUtils.GuiPropTypes[property];
            switch (targetType)
            {
                case Type t when t == typeof(Color):
                    ProcessPropertyForAnimation<Color>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(Gradient):
                    ProcessPropertyForAnimation<Gradient>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(double):
                    ProcessPropertyForAnimation<double>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(Vector4):
                    ProcessPropertyForAnimation<Vector4>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(BoxShadow):
                    ProcessPropertyForAnimation<BoxShadow>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(UnitValue):
                    ProcessPropertyForAnimation<UnitValue>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(int):
                    ProcessPropertyForAnimation<int>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(float):
                    ProcessPropertyForAnimation<float>(property, deltaTime, completedInterpolations);
                    break;

                case Type t when t == typeof(Transform2D):
                    ProcessPropertyForAnimation<Transform2D>(property, deltaTime, completedInterpolations);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type {targetType} for property {property}.");
            }

            _currentValues.Add(property);
        }

        // Clean up completed interpolations
        foreach (GuiProp property in completedInterpolations)
        {
            RemoveInterpolation(property);
        }

        // Clear transition configs after processing - they don't persist across frames
        _transitionConfigs.Clear();
    }

    private void RemoveInterpolation(GuiProp property)
    {
        _interpolations.Remove(property);
    }

    private void ProcessPropertyForAnimation<T>(GuiProp property, double deltaTime,
        List<GuiProp> completedInterpolations)
    {
        // Get the target value based on what was set this frame or inherited
        T targetValue = GetTargetValue<T>(property);

        // If the property has a transition config, set up an interpolation
        if (_transitionConfigs.TryGetValue(property, out TransitionConfig? config))
        {
            ProcessPropertyWithTransition(property, targetValue, config, deltaTime, completedInterpolations);
        }
        else
        {
            // No transition config, set immediately
            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, targetValue);
            // _currentValues[property] = targetValue;
        }
    }

    /// <summary>
    ///     Gets the complete transform for an element.
    /// </summary>
    public Transform2D GetTransformForElement(Rect rect)
    {
        // TransformBuilder builder = new();
        _transformBuilder.Reset();

        // Set transform properties from the current values
        if (HasValue(GuiProp.TranslateX))
        {
            _transformBuilder.SetTranslateX(StyleUtils.GetValueFromStruct<double>(GuiProp.TranslateX, _currentGuiValues));
        }

        if (HasValue(GuiProp.TranslateY))
        {
            _transformBuilder.SetTranslateY(StyleUtils.GetValueFromStruct<double>(GuiProp.TranslateY, _currentGuiValues));
        }

        if (HasValue(GuiProp.ScaleX))
        {
            _transformBuilder.SetScaleX(StyleUtils.GetValueFromStruct<double>(GuiProp.ScaleX, _currentGuiValues));
        }

        if (HasValue(GuiProp.ScaleY))
        {
            _transformBuilder.SetScaleY(StyleUtils.GetValueFromStruct<double>(GuiProp.ScaleY, _currentGuiValues));
        }

        if (HasValue(GuiProp.Rotate))
        {
            _transformBuilder.SetRotate(StyleUtils.GetValueFromStruct<double>(GuiProp.Rotate, _currentGuiValues));
        }

        if (HasValue(GuiProp.SkewX))
        {
            _transformBuilder.SetSkewX(StyleUtils.GetValueFromStruct<double>(GuiProp.SkewX, _currentGuiValues));
        }

        if (HasValue(GuiProp.SkewY))
        {
            _transformBuilder.SetSkewY(StyleUtils.GetValueFromStruct<double>(GuiProp.SkewY, _currentGuiValues));
        }

        if (HasValue(GuiProp.OriginX))
        {
            _transformBuilder.SetOriginX(StyleUtils.GetValueFromStruct<double>(GuiProp.OriginX, _currentGuiValues));
        }

        if (HasValue(GuiProp.OriginY))
        {
            _transformBuilder.SetOriginY(StyleUtils.GetValueFromStruct<double>(GuiProp.OriginY, _currentGuiValues));
        }

        if (HasValue(GuiProp.Transform))
        {
            _transformBuilder.SetCustomTransform(StyleUtils.GetValueFromStruct<Transform2D>(GuiProp.Transform, _currentGuiValues));
        }

        return _transformBuilder.Build(rect);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    ///     Initializes values for properties with transitions.
    /// </summary>
    private void InitializeTransitionProperties()
    {
        foreach (GuiProp property in _propertiesWithTransitions)
        {
            // If we don't have a current value yet for a property with transition,
            // initialize it with the default or parent value
            if (HasValue(property))
            {
                continue;
            }

            Type targetType = StyleUtils.GuiPropTypes[property];
            switch (targetType)
            {
                case Type t when t == typeof(Color):
                    InitializeTransitionProperty<Color>(property);
                    break;

                case Type t when t == typeof(Gradient):
                    InitializeTransitionProperty<Gradient>(property);
                    break;

                case Type t when t == typeof(double):
                    InitializeTransitionProperty<double>(property);
                    break;

                case Type t when t == typeof(Vector4):
                    InitializeTransitionProperty<Vector4>(property);
                    break;

                case Type t when t == typeof(BoxShadow):
                    InitializeTransitionProperty<BoxShadow>(property);
                    break;

                case Type t when t == typeof(UnitValue):
                    InitializeTransitionProperty<UnitValue>(property);
                    break;

                case Type t when t == typeof(int):
                    InitializeTransitionProperty<int>(property);
                    break;

                case Type t when t == typeof(float):
                    InitializeTransitionProperty<float>(property);
                    break;

                case Type t when t == typeof(Transform2D):
                    InitializeTransitionProperty<Transform2D>(property);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type {targetType} for property {property}.");
            }
        }
    }

    private void InitializeTransitionProperty<T>(GuiProp property)
    {
        if (_parent != null && _parent.HasValue(property))
        {
            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, _parent.GetValue<T>(property));
        }
        else
        {
            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, GetDefaultValue<T>(property));
        }
    }

    /// <summary>
    ///     Gets the target value for a property based on explicit setting or inheritance.
    /// </summary>
    private T GetTargetValue<T>(GuiProp property)
    {
        if (_propertiesSetThisFrame.Contains(property)) // If property was set this frame, use the explicit value
        {
            return StyleUtils.GetValueFromStruct<T>(property, _targetGuiValues);
        }

        if (_parent != null && _parent.HasValue(property)) // If not set, but has parent, use parent value
        {
            return _parent.GetValue<T>(property);
        }

        // If not set and no parent, use default value
        return GetDefaultValue<T>(property);
    }

    private T GetDefaultValue<T>(GuiProp property)
    {
        return StyleUtils.GetValueFromStruct<T>(property, _defaultValues);
    }

    /// <summary>
    ///     Processes transitions for a property.
    /// </summary>
    private void ProcessPropertyWithTransition<T>(GuiProp property, T targetValue, TransitionConfig config,
        double deltaTime, List<GuiProp> completedInterpolations)
    {
        // If we don't have a current value yet, initialize it immediately
        T currentValue;

        if (!HasValue(property))
        {
            currentValue = targetValue;
            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, currentValue);
            // _currentValues[property] = currentValue;
            return;
        }

        currentValue = StyleUtils.GetValueFromStruct<T>(property, _currentGuiValues);
        // Skip if the values are already equal
        if (currentValue.Equals(targetValue))
        {
            return;
        }

        HandleInterpolationState(property, currentValue, targetValue, config, deltaTime, completedInterpolations);
    }

    private void HandleInterpolationState<T>(GuiProp property, T currentValue, T targetValue, TransitionConfig config, double deltaTime, List<GuiProp> completedInterpolations)
    {
        // Create or update interpolation state
        if (!_interpolations.TryGetValue(property, out IInterpolationState? state))
        {
            state = new InterpolationState<T>
            {
                Property = property,
                StartValue = currentValue,
                TargetValue = targetValue,
                Duration = config.Duration,
                EasingFunction = config.EasingFunction,
                CurrentTime = 0
            };
            _interpolations[property] = state;
        }

        InterpolationState<T> stateConverted = Unsafe.As<IInterpolationState, InterpolationState<T>>(ref state);

        if (!stateConverted.TargetValue.Equals(targetValue))
        {
            // Target has changed, restart interpolation
            stateConverted.StartValue = currentValue;
            stateConverted.TargetValue = targetValue;
            stateConverted.Duration = config.Duration;
            stateConverted.EasingFunction = config.EasingFunction;
            stateConverted.CurrentTime = 0;
        }

        // Update the interpolation
        stateConverted.CurrentTime += deltaTime;

        bool isFinished = stateConverted.Update(deltaTime, ref _currentGuiValues, completedInterpolations);

        if (isFinished) completedInterpolations.Add(property);
    }
    #endregion
}

public partial class Paper
{
    #region Style Management

    private ConcurrentBag<ElementStyle> _stylePool = new();
    private int _currentStyleIndex;
    public int stylesLastFrame = 0;

    /// <summary>
    ///     A dictionary to keep track of active styles for each element.
    /// </summary>
    private readonly Dictionary<ulong, ElementStyle> _activeStyles = new();

    /// <summary>
    ///     Update the styles for all active elements.
    /// </summary>
    /// <param name="deltaTime">The time since the last frame.</param>
    /// <param name="element">The root element to start updating from.</param>
    private void UpdateStyles(double deltaTime, ElementHandle element)
    {
        ulong id = element.Data.ID;
        if (_activeStyles.TryGetValue(id, out ElementStyle? style))
        {
            // Update the style properties
            style.Update(deltaTime);
            element.Data._elementStyle = style;
        }
        else
        {
            // Create a new style if it doesn't exist
            style = element.Data._elementStyle ?? new ElementStyle();
            element.Data._elementStyle = style;
            _activeStyles[id] = style;
        }

        // Update Children
        //
        foreach (int childIndex in element.Data.ChildIndices)
        {
            //TODO get the child here based on the child indices
            var child = new ElementHandle(this, childIndex);
            UpdateStyles(deltaTime, child);
        }
    }

    internal ElementStyle GetStyleFromPool()
    {
        if (_stylePool.IsEmpty)
        {
            return new ElementStyle();
        }

        if (_stylePool.TryTake(out ElementStyle elementStyle))
            return elementStyle;

        throw new InvalidOperationException("We failed to get a style from the pool. If you are seeing this, it means something went very wrong.");
    }

    /// <summary>
    ///     Set a style property value (no transition).
    /// </summary>
    /// TODO the issue is here because we don't have a way to check if the element style is already set and we were always overriding
    /// its value inside of the update function, since it wasn't a part of the active styles dictionary.
    /// maybe we need to add it to the active styles as soon as we create an element, rather than doing some janky shit here
    /// then it will be possible to directly attach a style to an element
    internal void SetStyleProperty<T>(ulong elementID, GuiProp property, T value)
    {
        if (!_activeStyles.TryGetValue(elementID, out ElementStyle? style))
        {
            // Create a new style if it doesn't exist
            style = GetStyleFromPool();
            _activeStyles[elementID] = style;
            // _createdElements[elementID]._elementStyle = style;
        }

        // Set the next value
        style.SetNextValue(property, value);
    }



    /// <summary>
    ///     Configure a transition for a property.
    /// </summary>
    internal void SetTransitionConfig(ulong elementID, GuiProp property, double duration,
        Func<double, double>? easing = null)
    {
        if (!_activeStyles.TryGetValue(elementID, out ElementStyle? style))
        {
            // Create a new style if it doesn't exist
            style = new ElementStyle();
            _activeStyles[elementID] = style;
            // _createdElements[elementID]._elementStyle = style;
        }

        // Set up the transition configuration
        style.SetTransitionConfig(property, duration, easing);
    }

    /// <summary>
    ///     Clean up styles at the end of a frame.
    /// </summary>
    private void EndOfFrameCleanupStyles(HashSet<ulong> createdElements)
    {
        int removedSomeStyles = 0;
        // Clean up any elements that haven't been accessed this frame
        foreach (KeyValuePair<ulong, ElementStyle> kvp in _activeStyles)
        {
            if (!createdElements.Contains(kvp.Key))
            {
                _activeStyles[kvp.Key].ReturnToPool();
                _stylePool.Add(_activeStyles[kvp.Key]);
                _activeStyles.Remove(kvp.Key);
                removedSomeStyles++;
            }
            else
            {
                kvp.Value.EndOfFrame(); // Reset the style for the next frame
            }
        }

        if (removedSomeStyles > 0) return;
    }

    #endregion

    #region Style Templates

    private readonly Dictionary<string, StyleTemplate> _styleTemplates = new();

    /// <summary>
    ///     Creates a new style template.
    /// </summary>
    public StyleTemplate DefineStyle(string name)
    {
        // Create a new style template
        StyleTemplate template = new();
        _styleTemplates[name] = template;
        return template;
    }

    /// <summary>
    ///     Creates a new style template. With one or more parent styles to inherit from.
    /// </summary>
    public StyleTemplate DefineStyle(string name, params string[] inheritFrom)
    {
        // Create a new style template
        StyleTemplate template = new();

        // Check if the parent style exists
        foreach (string parent in inheritFrom)
        {
            if (_styleTemplates.TryGetValue(parent, out StyleTemplate? parentTemplate))
            {
                parentTemplate.ApplyTo(template);
            }
            else
            {
                throw new ArgumentException($"Parent style '{parent}' does not exist yet.");
            }
        }

        _styleTemplates[name] = template;
        return template;
    }

    public void RegisterStyle(string name, StyleTemplate template) => _styleTemplates[name] = template;

    /// <summary>
    ///     Creates a new style template.
    /// </summary>
    public bool TryGetStyle(string name, out StyleTemplate? template) =>
        _styleTemplates.TryGetValue(name, out template);

    /// <summary>
    ///     Applies a named style and its pseudo-states to an element
    /// </summary>
    /// <param name="element">The element to apply styles to</param>
    /// <param name="baseName">The base style name (e.g., "button")</param>
    public void ApplyStyleWithStates(ElementHandle element, string baseName)
    {
        // Apply base style first
        if (TryGetStyle(baseName, out StyleTemplate? baseStyle))
        {
            baseStyle.ApplyTo(element);
        }

        // Apply pseudo-states in order
        (string, bool)[] pseudoStates = new[]
        {
            ("hovered", IsElementHovered(element.Data.ID)),
            ("focused", IsElementFocused(element.Data.ID)),
            ("active", IsElementActive(element.Data.ID))
        };

        foreach ((string state, bool isActive) in pseudoStates)
        {
            if (isActive)
            {
                string pseudoStyleName = $"{baseName}:{state}";
                if (TryGetStyle(pseudoStyleName, out StyleTemplate? pseudoStyle))
                {
                    pseudoStyle.ApplyTo(element);
                }
            }
        }
    }

    /// <summary>
    ///     Registers a complete style family (base + pseudo-states)
    /// </summary>
    /// <param name="baseName">The base style name</param>
    /// <param name="baseStyle">The base style</param>
    /// <param name="normalStyle">Optional normal state style</param>
    /// <param name="hoveredStyle">Optional hovered state style</param>
    /// <param name="focusedStyle">Optional focused state style</param>
    /// <param name="activeStyle">Optional active state style</param>
    public void RegisterStyleFamily(
        string baseName,
        StyleTemplate baseStyle,
        StyleTemplate normalStyle = null,
        StyleTemplate hoveredStyle = null,
        StyleTemplate focusedStyle = null,
        StyleTemplate activeStyle = null)
    {
        // Register base style
        RegisterStyle(baseName, baseStyle);

        // Register pseudo-states if provided
        if (normalStyle != null)
        {
            RegisterStyle($"{baseName}:normal", normalStyle);
        }

        if (hoveredStyle != null)
        {
            RegisterStyle($"{baseName}:hovered", hoveredStyle);
        }

        if (focusedStyle != null)
        {
            RegisterStyle($"{baseName}:focused", focusedStyle);
        }

        if (activeStyle != null)
        {
            RegisterStyle($"{baseName}:active", activeStyle);
        }
    }

    /// <summary>
    ///     Creates a style builder for easier style family creation
    /// </summary>
    /// <param name="baseName">The base style name</param>
    /// <returns>A style family builder</returns>
    public StyleFamilyBuilder CreateStyleFamily(string baseName) => new(this, baseName);

    /// <summary>
    ///     Helper class for building complete style families
    /// </summary>
    public class StyleFamilyBuilder
    {
        private readonly string _baseName;
        private readonly Paper _paper;
        private StyleTemplate _activeStyle;
        private StyleTemplate _baseStyle;
        private StyleTemplate _focusedStyle;
        private StyleTemplate _hoveredStyle;
        private StyleTemplate _normalStyle;

        internal StyleFamilyBuilder(Paper paper, string baseName)
        {
            _paper = paper;
            _baseName = baseName;
        }

        public StyleFamilyBuilder Base(StyleTemplate style)
        {
            _baseStyle = style;
            return this;
        }

        public StyleFamilyBuilder Normal(StyleTemplate style)
        {
            _normalStyle = style;
            return this;
        }

        public StyleFamilyBuilder Hovered(StyleTemplate style)
        {
            _hoveredStyle = style;
            return this;
        }

        public StyleFamilyBuilder Focused(StyleTemplate style)
        {
            _focusedStyle = style;
            return this;
        }

        public StyleFamilyBuilder Active(StyleTemplate style)
        {
            _activeStyle = style;
            return this;
        }

        public void Register() => _paper.RegisterStyleFamily(_baseName, _baseStyle, _normalStyle, _hoveredStyle,
            _focusedStyle, _activeStyle);
    }

    #endregion
}

public struct GuiProperties
{
    // Reference-like structs
    public Color BackgroundColor;
    public Color BorderColor;
    public Color TextColor;
    public Gradient BackgroundGradient;
    public BoxShadow BoxShadow;
    public Transform2D Transform;

    // Vector types
    public Vector4 Rounded;

    // Floating point numbers
    public double BorderWidth;
    public double AspectRatio;
    public double TranslateX;
    public double TranslateY;
    public double ScaleX;
    public double ScaleY;
    public double Rotate;
    public double OriginX;
    public double OriginY;
    public double SkewX;
    public double SkewY;
    public double WordSpacing;
    public double LetterSpacing;
    public double LineHeight;
    public double FontSize;

    // Integer types
    public int TabSize;

    // UnitValue types
    public UnitValue Width;
    public UnitValue Height;
    public UnitValue MinWidth;
    public UnitValue MaxWidth;
    public UnitValue MinHeight;
    public UnitValue MaxHeight;
    public UnitValue Left;
    public UnitValue Right;
    public UnitValue Top;
    public UnitValue Bottom;
    public UnitValue MinLeft;
    public UnitValue MaxLeft;
    public UnitValue MinRight;
    public UnitValue MaxRight;
    public UnitValue MinTop;
    public UnitValue MaxTop;
    public UnitValue MinBottom;
    public UnitValue MaxBottom;
    public UnitValue ChildLeft;
    public UnitValue ChildRight;
    public UnitValue ChildTop;
    public UnitValue ChildBottom;
    public UnitValue RowBetween;
    public UnitValue ColBetween;
    public UnitValue BorderLeft;
    public UnitValue BorderRight;
    public UnitValue BorderTop;
    public UnitValue BorderBottom;
    private bool _defaultValueSet = false;

    // Constructor
    public GuiProperties()
    {
        // if (_defaultValueSet)
        //     throw new InvalidOperationException(
        //         "You cannot set the default values twice. If you want to do this, cache them in another variable and set the struct directly");
        SetDefaultValues();
    }

    // Sets default values
    public void SetDefaultValues()
    {
        _defaultValueSet = true;
        // Colors and gradients
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;
        TextColor = Color.White;
        BackgroundGradient = Gradient.None;
        BoxShadow = BoxShadow.None;
        Transform = Transform2D.Identity;

        // Vector
        Rounded = new Vector4(0, 0, 0, 0);

        // Floating point numbers
        BorderWidth = 0.0;
        AspectRatio = -1.0;
        TranslateX = 0.0;
        TranslateY = 0.0;
        ScaleX = 1.0;
        ScaleY = 1.0;
        Rotate = 0.0;
        SkewX = 0.0;
        SkewY = 0.0;
        OriginX = 0.5;
        OriginY = 0.5;
        WordSpacing = 0.0;
        LetterSpacing = 0.0;
        LineHeight = 1.0;

        // Integers and floats
        TabSize = 4;
        FontSize = 16.0f;

        // UnitValues - layout
        Width = UnitValue.Stretch();
        Height = UnitValue.Stretch();
        MinWidth = UnitValue.Pixels(0);
        MaxWidth = UnitValue.Pixels(double.MaxValue);
        MinHeight = UnitValue.Pixels(0);
        MaxHeight = UnitValue.Pixels(double.MaxValue);

        Left = UnitValue.Auto;
        Right = UnitValue.Auto;
        Top = UnitValue.Auto;
        Bottom = UnitValue.Auto;
        MinLeft = UnitValue.Pixels(0);
        MaxLeft = UnitValue.Pixels(double.MaxValue);
        MinRight = UnitValue.Pixels(0);
        MaxRight = UnitValue.Pixels(double.MaxValue);
        MinTop = UnitValue.Pixels(0);
        MaxTop = UnitValue.Pixels(double.MaxValue);
        MinBottom = UnitValue.Pixels(0);
        MaxBottom = UnitValue.Pixels(double.MaxValue);

        ChildLeft = UnitValue.Auto;
        ChildRight = UnitValue.Auto;
        ChildTop = UnitValue.Auto;
        ChildBottom = UnitValue.Auto;
        RowBetween = UnitValue.Auto;
        ColBetween = UnitValue.Auto;

        BorderLeft = UnitValue.Pixels(0);
        BorderRight = UnitValue.Pixels(0);
        BorderTop = UnitValue.Pixels(0);
        BorderBottom = UnitValue.Pixels(0);
    }
}

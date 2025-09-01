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

    /// <summary>
    ///     Helper class to track interpolation state.
    /// </summary>
    /// TODO this still causes boxing and unboxing because the interpolation state
    /// is converting everything to an object. We need to change this, but then we also need to update the
    /// Dictionary.
    /// I think a generic struct to store the information would be best, then we can just have a single object that
    /// we can collect all of our style type information from.
    private class InterpolationState
    {
        public object StartValue { get; set; }
        public object TargetValue { get; set; }
        public double Duration { get; set; }
        public Func<double, double>? EasingFunction { get; set; }
        public double CurrentTime { get; set; }
    }

    #endregion

    #region Fields

    private static readonly GuiProperties _defaultValues = new();
    private GuiProperties _currentGuiValues = new();
    private GuiProperties _targetGuiValues = new();

    private readonly Dictionary<GuiProp, Color> _colorProperties = new();
    private readonly Dictionary<GuiProp, Gradient> _gradientProperties = new();
    private readonly Dictionary<GuiProp, double> _doubleProperties = new();
    private readonly Dictionary<GuiProp, Vector4> _vector4Properties = new();
    private readonly Dictionary<GuiProp, BoxShadow> _boxShadowProperties = new();
    private readonly Dictionary<GuiProp, UnitValue> _unitValueProperties = new();
    private readonly Dictionary<GuiProp, int> _intProperties = new();
    private readonly Dictionary<GuiProp, float> _floatProperties = new();
    private readonly Dictionary<GuiProp, Transform2D> _transform2DProperties = new();

    private readonly Dictionary<GuiProp, Color> _colorTargetProps = new();
    private readonly Dictionary<GuiProp, Gradient> _gradientTargetProps = new();
    private readonly Dictionary<GuiProp, double> _doubleTargetProps = new();
    private readonly Dictionary<GuiProp, Vector4> _vector4TargetProps = new();
    private readonly Dictionary<GuiProp, BoxShadow> _boxShadowTargetProps = new();
    private readonly Dictionary<GuiProp, UnitValue> _unitValueTargetProps = new();
    private readonly Dictionary<GuiProp, int> _intTargetProps = new();
    private readonly Dictionary<GuiProp, float> _floatTargetProps = new();
    private readonly Dictionary<GuiProp, Transform2D> _transform2DTargetProps = new();

    // public Color GetColorValue(GuiProp property)
    // {
    //     // If we have the value, return it
    //     if (_colorProperties.TryGetValue(property, out var value))
    //         return value;
    //
    //     // Otherwise check parent
    //     if (_parent != null)
    //         return _parent.GetColorValue(property);
    //
    //     // Otherwise return default
    //     return DefaultStyleValues.Get<Color>(property);
    // }


    // State tracking
    private readonly HashSet<GuiProp> _propertiesSetThisFrame = new();
    private readonly HashSet<GuiProp> _propertiesWithTransitions = new();
    private bool _firstFrame = true;

    // Property values
    private readonly HashSet<GuiProp> _currentValues = new();
    private readonly HashSet<GuiProp> _targetValues = new();

    // Transition state
    private readonly Dictionary<GuiProp, TransitionConfig> _transitionConfigs = new();
    private readonly Dictionary<GuiProp, InterpolationState> _interpolations = new();

    // Inheritance
    private ElementStyle? _parent;

    #endregion

    #region Public Methods

    public void RemoveFromPool()
    {
        _propertiesSetThisFrame.Clear();
        _propertiesWithTransitions.Clear();

        //TODO understand how this works better
        _targetValues.Clear();
        _currentValues.Clear();

        _currentGuiValues.SetDefaultValues();
        _targetGuiValues.SetDefaultValues();;
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
        _interpolations.Remove(property); // Remove any existing interpolation state
    }

    /// <summary>
    ///     Sets a property's target value for transition.
    /// </summary>
    public void SetNextValue<T>(GuiProp property, T value)
    {
        _propertiesSetThisFrame.Add(property);
        _targetValues.Add(property);
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
        _interpolations.Remove(property);
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
        }

        // Clean up completed interpolations
        foreach (GuiProp property in completedInterpolations)
        {
            _interpolations.Remove(property);
        }

        // Clear transition configs after processing - they don't persist across frames
        _transitionConfigs.Clear();
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
        TransformBuilder builder = new();

        // Set transform properties from the current values
        if (HasValue(GuiProp.TranslateX))
        {
            builder.SetTranslateX(StyleUtils.GetValueFromStruct<double>(GuiProp.TranslateX, _currentGuiValues));
        }

        if (HasValue(GuiProp.TranslateY))
        {
            builder.SetTranslateY(StyleUtils.GetValueFromStruct<double>(GuiProp.TranslateY, _currentGuiValues));
        }

        if (HasValue(GuiProp.ScaleX))
        {
            builder.SetScaleX(StyleUtils.GetValueFromStruct<double>(GuiProp.ScaleX, _currentGuiValues));
        }

        if (HasValue(GuiProp.ScaleY))
        {
            builder.SetScaleY(StyleUtils.GetValueFromStruct<double>(GuiProp.ScaleY, _currentGuiValues));
        }

        if (HasValue(GuiProp.Rotate))
        {
            builder.SetRotate(StyleUtils.GetValueFromStruct<double>(GuiProp.Rotate, _currentGuiValues));
        }

        if (HasValue(GuiProp.SkewX))
        {
            builder.SetSkewX(StyleUtils.GetValueFromStruct<double>(GuiProp.SkewX, _currentGuiValues));
        }

        if (HasValue(GuiProp.SkewY))
        {
            builder.SetSkewY(StyleUtils.GetValueFromStruct<double>(GuiProp.SkewY, _currentGuiValues));
        }

        if (HasValue(GuiProp.OriginX))
        {
            builder.SetOriginX(StyleUtils.GetValueFromStruct<double>(GuiProp.OriginX, _currentGuiValues));
        }

        if (HasValue(GuiProp.OriginY))
        {
            builder.SetOriginY(StyleUtils.GetValueFromStruct<double>(GuiProp.OriginY, _currentGuiValues));
        }

        if (HasValue(GuiProp.Transform))
        {
            builder.SetCustomTransform(StyleUtils.GetValueFromStruct<Transform2D>(GuiProp.Transform, _currentGuiValues));
        }

        return builder.Build(rect);
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

        // Create or update interpolation state
        if (!_interpolations.TryGetValue(property, out InterpolationState? state))
        {
            state = new InterpolationState
            {
                StartValue = currentValue,
                TargetValue = targetValue,
                Duration = config.Duration,
                EasingFunction = config.EasingFunction,
                CurrentTime = 0
            };
            _interpolations[property] = state;
        }
        else if (!state.TargetValue.Equals(targetValue))
        {
            // Target has changed, restart interpolation
            state.StartValue = currentValue;
            state.TargetValue = targetValue;
            state.Duration = config.Duration;
            state.EasingFunction = config.EasingFunction;
            state.CurrentTime = 0;
        }

        // Update the interpolation
        state.CurrentTime += deltaTime;

        if (state.CurrentTime >= state.Duration)
        {
            // Interpolation complete
            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, targetValue);
            // _currentValues[property] = targetValue;
            completedInterpolations.Add(property);
        }
        else
        {
            // Calculate interpolated value
            double t = state.CurrentTime / state.Duration;
            if (state.EasingFunction != null)
            {
                t = state.EasingFunction(t);
            }

            StyleUtils.SetValueInStruct(property, ref _currentGuiValues, Interpolate(state.StartValue, state.TargetValue, t));
            // _currentValues[property] = Interpolate(state.StartValue, state.TargetValue, t);
        }
    }

    /// <summary>
    ///     Interpolates between two values based on their type.
    /// </summary>
    private T Interpolate<T>(T start, T end, double t)
    {
        if (typeof(T) == typeof(double))
        {
            double result = Unsafe.As<T, double>(ref start) +
                            (Unsafe.As<T, double>(ref end) - Unsafe.As<T, double>(ref start)) * t;
            return Unsafe.As<double, T>(ref result);
        }

        if (typeof(T) == typeof(float))
        {
            float result = Unsafe.As<T, float>(ref start) +
                           (Unsafe.As<T, float>(ref end) - Unsafe.As<T, float>(ref start)) * (float)t;
            return Unsafe.As<float, T>(ref result);
        }

        if (typeof(T) == typeof(int))
        {
            int result = Unsafe.As<T, int>(ref start) +
                         (int)((Unsafe.As<T, int>(ref end) - Unsafe.As<T, int>(ref start)) * t);
            return Unsafe.As<int, T>(ref result);
        }

        if (typeof(T) == typeof(Color))
        {
            Color result = InterpolateColor(Unsafe.As<T, Color>(ref start), Unsafe.As<T, Color>(ref end), t);
            return Unsafe.As<Color, T>(ref result);
        }

        if (typeof(T) == typeof(Vector2))
        {
            Vector2 result = Vector2.Lerp(Unsafe.As<T, Vector2>(ref start), Unsafe.As<T, Vector2>(ref end), (float)t);
            return Unsafe.As<Vector2, T>(ref result);
        }

        if (typeof(T) == typeof(Vector3))
        {
            Vector3 result = Vector3.Lerp(Unsafe.As<T, Vector3>(ref start), Unsafe.As<T, Vector3>(ref end), (float)t);
            return Unsafe.As<Vector3, T>(ref result);
        }

        if (typeof(T) == typeof(Vector4))
        {
            Vector4 result = Vector4.Lerp(Unsafe.As<T, Vector4>(ref start), Unsafe.As<T, Vector4>(ref end), (float)t);
            return Unsafe.As<Vector4, T>(ref result);
        }

        if (typeof(T) == typeof(UnitValue))
        {
            UnitValue result = UnitValue.Lerp(Unsafe.As<T, UnitValue>(ref start), Unsafe.As<T, UnitValue>(ref end), t);
            return Unsafe.As<UnitValue, T>(ref result);
        }

        if (typeof(T) == typeof(Transform2D))
        {
            Transform2D result = Transform2D.Lerp(Unsafe.As<T, Transform2D>(ref start),
                Unsafe.As<T, Transform2D>(ref end), t);
            return Unsafe.As<Transform2D, T>(ref result);
        }

        if (typeof(T) == typeof(string))
        {
            return t > 0.5 ? end : start;
        }

        if (typeof(T) == typeof(Gradient))
        {
            Gradient result = Gradient.Lerp(Unsafe.As<T, Gradient>(ref start), Unsafe.As<T, Gradient>(ref end), t);
            return Unsafe.As<Gradient, T>(ref result);
        }

        if (typeof(T) == typeof(BoxShadow))
        {
            BoxShadow result = BoxShadow.Lerp(Unsafe.As<T, BoxShadow>(ref start), Unsafe.As<T, BoxShadow>(ref end), t);
            return Unsafe.As<BoxShadow, T>(ref result);
        }

        // Default to just returning the end value if type is unknown
        return end;
    }


    /// <summary>
    ///     Interpolates between two colors.
    /// </summary>
    private Color InterpolateColor(Color start, Color end, double t)
    {
        int r = (int)(start.R + (end.R - start.R) * t);
        int g = (int)(start.G + (end.G - start.G) * t);
        int b = (int)(start.B + (end.B - start.B) * t);
        int a = (int)(start.A + (end.A - start.A) * t);

        return Color.FromArgb(a, r, g, b);
    }

    /// <summary>
    ///     Gets the default value for a property.
    /// </summary>
    private object GetDefaultValue(GuiProp property) =>
        property switch
        {
            // Visual Properties
            GuiProp.BackgroundColor => Color.Transparent,
            GuiProp.BackgroundGradient => Gradient.None,
            GuiProp.BorderColor => Color.Transparent,
            GuiProp.BorderWidth => 0.0,
            GuiProp.Rounded => new Vector4(0, 0, 0, 0),
            GuiProp.BoxShadow => BoxShadow.None,

            // Core Layout Properties
            GuiProp.AspectRatio => -1.0,
            GuiProp.Width => UnitValue.Stretch(),
            GuiProp.Height => UnitValue.Stretch(),
            GuiProp.MinWidth => UnitValue.Pixels(0),
            GuiProp.MaxWidth => UnitValue.Pixels(double.MaxValue),
            GuiProp.MinHeight => UnitValue.Pixels(0),
            GuiProp.MaxHeight => UnitValue.Pixels(double.MaxValue),

            // Positioning Properties
            GuiProp.Left => UnitValue.Auto,
            GuiProp.Right => UnitValue.Auto,
            GuiProp.Top => UnitValue.Auto,
            GuiProp.Bottom => UnitValue.Auto,
            GuiProp.MinLeft => UnitValue.Pixels(0),
            GuiProp.MaxLeft => UnitValue.Pixels(double.MaxValue),
            GuiProp.MinRight => UnitValue.Pixels(0),
            GuiProp.MaxRight => UnitValue.Pixels(double.MaxValue),
            GuiProp.MinTop => UnitValue.Pixels(0),
            GuiProp.MaxTop => UnitValue.Pixels(double.MaxValue),
            GuiProp.MinBottom => UnitValue.Pixels(0),
            GuiProp.MaxBottom => UnitValue.Pixels(double.MaxValue),

            // Child Layout Properties
            GuiProp.ChildLeft => UnitValue.Auto,
            GuiProp.ChildRight => UnitValue.Auto,
            GuiProp.ChildTop => UnitValue.Auto,
            GuiProp.ChildBottom => UnitValue.Auto,
            GuiProp.RowBetween => UnitValue.Auto,
            GuiProp.ColBetween => UnitValue.Auto,
            GuiProp.BorderLeft => UnitValue.Pixels(0),
            GuiProp.BorderRight => UnitValue.Pixels(0),
            GuiProp.BorderTop => UnitValue.Pixels(0),
            GuiProp.BorderBottom => UnitValue.Pixels(0),

            // Transform Properties
            GuiProp.TranslateX => 0.0,
            GuiProp.TranslateY => 0.0,
            GuiProp.ScaleX => 1.0,
            GuiProp.ScaleY => 1.0,
            GuiProp.Rotate => 0.0,
            GuiProp.SkewX => 0.0,
            GuiProp.SkewY => 0.0,
            GuiProp.OriginX => 0.5, // Default is center
            GuiProp.OriginY => 0.5, // Default is center
            GuiProp.Transform => Transform2D.Identity,

            // Text Properties
            GuiProp.TextColor => Color.White,

            GuiProp.WordSpacing => 0.0,
            GuiProp.LetterSpacing => 0.0,
            GuiProp.LineHeight => 1.0,

            GuiProp.TabSize => 4,
            GuiProp.FontSize => 16.0f,

            _ => throw new ArgumentOutOfRangeException(nameof(property), property, null)
        };

    #endregion
}

public partial class Paper
{
    #region Style Management

    /// <summary>
    ///     A dictionary to keep track of active styles for each element.
    /// </summary>
    private readonly Dictionary<ulong, ElementStyle> _activeStyles = new();

    /// <summary>
    ///     Update the styles for all active elements.
    /// </summary>
    /// <param name="deltaTime">The time since the last frame.</param>
    /// <param name="element">The root element to start updating from.</param>
    private void UpdateStyles(double deltaTime, Element element)
    {
        ulong id = element.ID;
        if (_activeStyles.TryGetValue(id, out ElementStyle? style))
        {
            // Update the style properties
            style.Update(deltaTime);
            element._elementStyle = style;
        }
        else
        {
            // Create a new style if it doesn't exist
            style = element._elementStyle ?? new ElementStyle();
            element._elementStyle = style;
            _activeStyles[id] = style;
        }

        // Update Children
        foreach (Element child in element.Children)
        {
            UpdateStyles(deltaTime, child);
        }
    }

    /// <summary>
    ///     Set a style property value (no transition).
    /// </summary>
    internal void SetStyleProperty<T>(ulong elementID, GuiProp property, T value)
    {
        if (!_activeStyles.TryGetValue(elementID, out ElementStyle? style))
        {
            // Create a new style if it doesn't exist
            style = new ElementStyle();
            _activeStyles[elementID] = style;
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
        }

        // Set up the transition configuration
        style.SetTransitionConfig(property, duration, easing);
    }

    /// <summary>
    ///     Clean up styles at the end of a frame.
    /// </summary>
    private void EndOfFrameCleanupStyles(Dictionary<ulong, Element> createdElements)
    {
        // Clean up any elements that haven't been accessed this frame
        List<ulong> elementsToRemove = new();
        foreach (KeyValuePair<ulong, ElementStyle> kvp in _activeStyles)
        {
            if (!createdElements.ContainsKey(kvp.Key))
            {
                elementsToRemove.Add(kvp.Key);
            }
            else
            {
                kvp.Value.EndOfFrame(); // Reset the style for the next frame
            }
        }

        foreach (ulong id in elementsToRemove)
        {
            _activeStyles.Remove(id);
        }
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
    public void ApplyStyleWithStates(Element element, string baseName)
    {
        // Apply base style first
        if (TryGetStyle(baseName, out StyleTemplate? baseStyle))
        {
            baseStyle.ApplyTo(element);
        }

        // Apply pseudo-states in order
        (string, bool)[] pseudoStates = new[]
        {
            ("hovered", IsElementHovered(element.ID)), ("focused", IsElementFocused(element.ID)),
            ("active", IsElementActive(element.ID))
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

    // Integer types
    public int TabSize;
    public float FontSize;

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

    // Constructor
    public GuiProperties() => SetDefaultValues();

    // Sets default values
    public void SetDefaultValues()
    {
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

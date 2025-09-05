// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;
using System.Runtime.CompilerServices;

using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;

namespace Prowl.PaperUI;

public static class StyleUtils
{
    public static readonly IReadOnlyDictionary<GuiProp, Type> GuiPropTypes = new Dictionary<GuiProp, Type>
    {
        { GuiProp.BackgroundColor, typeof(Color) },
        { GuiProp.BackgroundGradient, typeof(Gradient) },
        { GuiProp.BorderColor, typeof(Color) },
        { GuiProp.BorderWidth, typeof(double) },
        { GuiProp.Rounded, typeof(Vector4) },
        { GuiProp.BoxShadow, typeof(BoxShadow) },
        { GuiProp.AspectRatio, typeof(double) },
        { GuiProp.Width, typeof(UnitValue) },
        { GuiProp.Height, typeof(UnitValue) },
        { GuiProp.MinWidth, typeof(UnitValue) },
        { GuiProp.MaxWidth, typeof(UnitValue) },
        { GuiProp.MinHeight, typeof(UnitValue) },
        { GuiProp.MaxHeight, typeof(UnitValue) },
        { GuiProp.Left, typeof(UnitValue) },
        { GuiProp.Right, typeof(UnitValue) },
        { GuiProp.Top, typeof(UnitValue) },
        { GuiProp.Bottom, typeof(UnitValue) },
        { GuiProp.MinLeft, typeof(UnitValue) },
        { GuiProp.MaxLeft, typeof(UnitValue) },
        { GuiProp.MinRight, typeof(UnitValue) },
        { GuiProp.MaxRight, typeof(UnitValue) },
        { GuiProp.MinTop, typeof(UnitValue) },
        { GuiProp.MaxTop, typeof(UnitValue) },
        { GuiProp.MinBottom, typeof(UnitValue) },
        { GuiProp.MaxBottom, typeof(UnitValue) },
        { GuiProp.ChildLeft, typeof(UnitValue) },
        { GuiProp.ChildRight, typeof(UnitValue) },
        { GuiProp.ChildTop, typeof(UnitValue) },
        { GuiProp.ChildBottom, typeof(UnitValue) },
        { GuiProp.RowBetween, typeof(UnitValue) },
        { GuiProp.ColBetween, typeof(UnitValue) },
        { GuiProp.BorderLeft, typeof(UnitValue) },
        { GuiProp.BorderRight, typeof(UnitValue) },
        { GuiProp.BorderTop, typeof(UnitValue) },
        { GuiProp.BorderBottom, typeof(UnitValue) },
        { GuiProp.TranslateX, typeof(double) },
        { GuiProp.TranslateY, typeof(double) },
        { GuiProp.ScaleX, typeof(double) },
        { GuiProp.ScaleY, typeof(double) },
        { GuiProp.Rotate, typeof(double) },
        { GuiProp.OriginX, typeof(double) },
        { GuiProp.OriginY, typeof(double) },
        { GuiProp.SkewX, typeof(double) },
        { GuiProp.SkewY, typeof(double) },
        { GuiProp.Transform, typeof(Transform2D) },
        { GuiProp.TextColor, typeof(Color) },
        { GuiProp.WordSpacing, typeof(double) },
        { GuiProp.LetterSpacing, typeof(double) },
        { GuiProp.LineHeight, typeof(double) },
        { GuiProp.TabSize, typeof(int) },
        { GuiProp.FontSize, typeof(double) }
    };

public static T GetValueFromStruct<T>(GuiProp property, ref GuiProperties propsStruct)
    {
        switch (property)
        {
            case GuiProp.BackgroundColor:
                return Unsafe.As<Color, T>(ref propsStruct.BackgroundColor);

            case GuiProp.BorderColor:
                return Unsafe.As<Color, T>(ref propsStruct.BorderColor);

            case GuiProp.TextColor:
                return Unsafe.As<Color, T>(ref propsStruct.TextColor);

            case GuiProp.BackgroundGradient:
                return Unsafe.As<Gradient, T>(ref propsStruct.BackgroundGradient);

            case GuiProp.BoxShadow:
                return Unsafe.As<BoxShadow, T>(ref propsStruct.BoxShadow);

            case GuiProp.Transform:
                return Unsafe.As<Transform2D, T>(ref propsStruct.Transform);

            case GuiProp.Rounded:
                return Unsafe.As<Vector4, T>(ref propsStruct.Rounded);

            case GuiProp.BorderWidth:
                return Unsafe.As<double, T>(ref propsStruct.BorderWidth);

            case GuiProp.AspectRatio:
                return Unsafe.As<double, T>(ref propsStruct.AspectRatio);

            case GuiProp.TranslateX:
                return Unsafe.As<double, T>(ref propsStruct.TranslateX);

            case GuiProp.TranslateY:
                return Unsafe.As<double, T>(ref propsStruct.TranslateY);

            case GuiProp.ScaleX:
                return Unsafe.As<double, T>(ref propsStruct.ScaleX);

            case GuiProp.ScaleY:
                return Unsafe.As<double, T>(ref propsStruct.ScaleY);

            case GuiProp.Rotate:
                return Unsafe.As<double, T>(ref propsStruct.Rotate);

            case GuiProp.OriginX:
                return Unsafe.As<double, T>(ref propsStruct.OriginX);

            case GuiProp.OriginY:
                return Unsafe.As<double, T>(ref propsStruct.OriginY);

            case GuiProp.SkewX:
                return Unsafe.As<double, T>(ref propsStruct.SkewX);

            case GuiProp.SkewY:
                return Unsafe.As<double, T>(ref propsStruct.SkewY);

            case GuiProp.WordSpacing:
                return Unsafe.As<double, T>(ref propsStruct.WordSpacing);

            case GuiProp.LetterSpacing:
                return Unsafe.As<double, T>(ref propsStruct.LetterSpacing);

            case GuiProp.LineHeight:
                return Unsafe.As<double, T>(ref propsStruct.LineHeight);

            case GuiProp.TabSize:
                return Unsafe.As<int, T>(ref propsStruct.TabSize);

            case GuiProp.FontSize:
                return Unsafe.As<double, T>(ref propsStruct.FontSize);

            case GuiProp.Width:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Width);

            case GuiProp.Height:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Height);

            case GuiProp.MinWidth:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinWidth);

            case GuiProp.MaxWidth:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxWidth);

            case GuiProp.MinHeight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinHeight);

            case GuiProp.MaxHeight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxHeight);

            case GuiProp.Left:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Left);

            case GuiProp.Right:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Right);

            case GuiProp.Top:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Top);

            case GuiProp.Bottom:
                return Unsafe.As<UnitValue, T>(ref propsStruct.Bottom);

            case GuiProp.MinLeft:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinLeft);

            case GuiProp.MaxLeft:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxLeft);

            case GuiProp.MinRight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinRight);

            case GuiProp.MaxRight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxRight);

            case GuiProp.MinTop:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinTop);

            case GuiProp.MaxTop:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxTop);

            case GuiProp.MinBottom:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MinBottom);

            case GuiProp.MaxBottom:
                return Unsafe.As<UnitValue, T>(ref propsStruct.MaxBottom);

            case GuiProp.ChildLeft:
                return Unsafe.As<UnitValue, T>(ref propsStruct.ChildLeft);

            case GuiProp.ChildRight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.ChildRight);

            case GuiProp.ChildTop:
                return Unsafe.As<UnitValue, T>(ref propsStruct.ChildTop);

            case GuiProp.ChildBottom:
                return Unsafe.As<UnitValue, T>(ref propsStruct.ChildBottom);

            case GuiProp.RowBetween:
                return Unsafe.As<UnitValue, T>(ref propsStruct.RowBetween);

            case GuiProp.ColBetween:
                return Unsafe.As<UnitValue, T>(ref propsStruct.ColBetween);

            case GuiProp.BorderLeft:
                return Unsafe.As<UnitValue, T>(ref propsStruct.BorderLeft);

            case GuiProp.BorderRight:
                return Unsafe.As<UnitValue, T>(ref propsStruct.BorderRight);

            case GuiProp.BorderTop:
                return Unsafe.As<UnitValue, T>(ref propsStruct.BorderTop);

            case GuiProp.BorderBottom:
                return Unsafe.As<UnitValue, T>(ref propsStruct.BorderBottom);

            default:
                throw new InvalidOperationException($"Unsupported property {property} or type {typeof(T)}.");
        }
    }

    public static void SetValueInStruct<T>(GuiProp property, ref GuiProperties propsStruct, T value, bool targetValue = false)
    {
        switch (property)
        {
            case GuiProp.BackgroundColor:
                propsStruct.BackgroundColor = Unsafe.As<T, Color>(ref value);
                break;

            case GuiProp.BorderColor:
                propsStruct.BorderColor = Unsafe.As<T, Color>(ref value);
                break;

            case GuiProp.TextColor:
                propsStruct.TextColor = Unsafe.As<T, Color>(ref value);
                break;

            case GuiProp.BackgroundGradient:
                propsStruct.BackgroundGradient = Unsafe.As<T, Gradient>(ref value);
                break;

            case GuiProp.BoxShadow:
                propsStruct.BoxShadow = Unsafe.As<T, BoxShadow>(ref value);
                break;

            case GuiProp.Transform:
                propsStruct.Transform = Unsafe.As<T, Transform2D>(ref value);
                break;

            case GuiProp.Rounded:
                propsStruct.Rounded = Unsafe.As<T, Vector4>(ref value);
                break;

            case GuiProp.BorderWidth:
                propsStruct.BorderWidth = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.AspectRatio:
                propsStruct.AspectRatio = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.TranslateX:
                propsStruct.TranslateX = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.TranslateY:
                propsStruct.TranslateY = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.ScaleX:
                propsStruct.ScaleX = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.ScaleY:
                propsStruct.ScaleY = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.Rotate:
                propsStruct.Rotate = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.OriginX:
                propsStruct.OriginX = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.OriginY:
                propsStruct.OriginY = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.SkewX:
                propsStruct.SkewX = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.SkewY:
                propsStruct.SkewY = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.WordSpacing:
                propsStruct.WordSpacing = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.LetterSpacing:
                propsStruct.LetterSpacing = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.LineHeight:
                propsStruct.LineHeight = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.TabSize:
                propsStruct.TabSize = Unsafe.As<T, int>(ref value);
                break;

            case GuiProp.FontSize:
                propsStruct.FontSize = Unsafe.As<T, double>(ref value);
                break;

            case GuiProp.Width:
                propsStruct.Width = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.Height:
                propsStruct.Height = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinWidth:
                propsStruct.MinWidth = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxWidth:
                propsStruct.MaxWidth = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinHeight:
                propsStruct.MinHeight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxHeight:
                propsStruct.MaxHeight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.Left:
                propsStruct.Left = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.Right:
                propsStruct.Right = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.Top:
                propsStruct.Top = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.Bottom:
                propsStruct.Bottom = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinLeft:
                propsStruct.MinLeft = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxLeft:
                propsStruct.MaxLeft = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinRight:
                propsStruct.MinRight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxRight:
                propsStruct.MaxRight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinTop:
                propsStruct.MinTop = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxTop:
                propsStruct.MaxTop = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MinBottom:
                propsStruct.MinBottom = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.MaxBottom:
                propsStruct.MaxBottom = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.ChildLeft:
                propsStruct.ChildLeft = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.ChildRight:
                propsStruct.ChildRight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.ChildTop:
                propsStruct.ChildTop = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.ChildBottom:
                propsStruct.ChildBottom = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.RowBetween:
                propsStruct.RowBetween = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.ColBetween:
                propsStruct.ColBetween = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.BorderLeft:
                propsStruct.BorderLeft = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.BorderRight:
                propsStruct.BorderRight = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.BorderTop:
                propsStruct.BorderTop = Unsafe.As<T, UnitValue>(ref value);
                break;

            case GuiProp.BorderBottom:
                propsStruct.BorderBottom = Unsafe.As<T, UnitValue>(ref value);
                break;

            default:
                throw new InvalidOperationException($"Unsupported property {property} or type {typeof(T)}.");
        }
    }

        /// <summary>
    ///     Interpolates between two values based on their type.
    /// </summary>
    public static T Interpolate<T>(T start, T end, double t)
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
    private static Color InterpolateColor(Color start, Color end, double t)
    {
        int r = (int)(start.R + (end.R - start.R) * t);
        int g = (int)(start.G + (end.G - start.G) * t);
        int b = (int)(start.B + (end.B - start.B) * t);
        int a = (int)(start.A + (end.A - start.A) * t);

        return Color.FromArgb(a, r, g, b);
    }
}

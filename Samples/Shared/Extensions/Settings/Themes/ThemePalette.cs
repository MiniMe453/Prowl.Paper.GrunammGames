// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.


using System.Drawing;

namespace Prowl.PaperUI;

   /// <summary>
/// Defines the available color options for UI elements.
/// Use CONTRAST instead of LIGHT or DARK for theme-consistent light/dark colors.
/// </summary>
public enum ElementColor
{
    /// <summary>
    /// Light color variant (internal use only). Use CONTRAST for theme-aware light/dark appearance.
    /// </summary>
    [Obsolete("LIGHT is for internal use only. Use CONTRAST for theme-aware light/dark appearance.")]
    LIGHT,

    /// <summary>
    /// Primary color, used for main actions and highlights.
    /// </summary>
    PRIMARY,

    /// <summary>
    /// Secondary color, used for less prominent actions.
    /// </summary>
    SECONDARY,

    /// <summary>
    /// Success color, typically used to indicate successful operations.
    /// </summary>
    SUCCESS,

    /// <summary>
    /// Info color, typically used for informational messages.
    /// </summary>
    INFO,

    /// <summary>
    /// Warning color, used to highlight potential issues or warnings.
    /// </summary>
    WARNING,

    /// <summary>
    /// Danger color, used for errors or destructive actions.
    /// </summary>
    DANGER,

    /// <summary>
    /// Dark color variant (internal use only). Use CONTRAST for theme-aware light/dark appearance.
    /// </summary>
    [Obsolete("DARK is for internal use only. Use CONTRAST for theme-aware light/dark appearance.")]
    DARK,

    /// <summary>
    /// Transparent color, used when no background color is desired.
    /// </summary>
    TRANSPARENT,

    /// <summary>
    /// Pure white color, not theme-dependent.
    /// </summary>
    WHITE,

    /// <summary>
    /// Pure black color, not theme-dependent.
    /// </summary>
    BLACK,

    /// <summary>
    /// Contrast color that adapts to the program's theme (light or dark).
    /// Recommended for theme-consistent light/dark buttons.
    /// </summary>
    CONTRAST,

    /// <summary>
    /// Count of enum entries (not a color). Used for iteration or bounds checking.
    /// </summary>
    COUNT
};



    /// <summary>
    /// Represents different shades of a base color.
    /// The default color is SHADE_400, and other shades are percentage variations of this.
    /// In dark mode, SHADE_0 is the lightest color, and in light mode, SHADE_0 is the darkest.
    /// </summary>
    public enum ColorShades
    {
        // /// <summary>
        // /// Extreme contrast shade. Lightest in dark mode, darkest in light mode.
        // /// </summary>
        // SHADE_0 = 0,
        //
        // /// <summary>
        // /// Very light/dark variation of the base color.
        // /// </summary>
        // SHADE_100 = 1,
        //
        // /// <summary>
        // /// Middle Ground Theme Color
        // /// </summary>
        // SHADE_150 = 2,
        //
        // /// <summary>
        // /// Lighter/darker variant with noticeable contrast from the base.
        // /// </summary>
        // SHADE_200 = 3,
        //
        // /// <summary>
        // /// Slightly adjusted variant, closer to the base color.
        // /// </summary>
        // SHADE_300 = 4,
        //
        // /// <summary>
        // /// Default base color.
        // /// </summary>
        // SHADE_400 = 5,
        // /// <summary>
        // /// A more extreme version of the default base color. SHOULD ONLY BE USED FOR WINDOW BACKGROUND
        // /// </summary>
        // SHADE_500 = 6,
        SHADE_LIGHT,
        SHADE_DARK,
        SHADE_NORMAL,
        COUNT = 3
    }

public abstract class ThemePalette
{
    public Color LightColor { get; private set; }
    public Color DarkColor { get; private set; }
    public Color PrimaryColor { get; private set; }
    public Color SecondaryColor { get; private set; }
    public Color InfoColor { get; private set; }
    public Color SuccessColor { get; private set; }
    public Color WarningColor { get; private set; }
    public Color DangerColor { get; private set; }
    public float WindowRounding { get; private set; }
    public float ButtonRounding { get; private set; }

    public void SetColorPalette(
        Color lightColor,
        Color darkColor,
        Color primaryColor,
        Color secondaryColor,
        Color infoColor,
        Color successColor,
        Color warningColor,
        Color dangerColor,
        float windowRounding,
        float buttonRounding)
    {
        LightColor = lightColor;
        DarkColor = darkColor;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        InfoColor = infoColor;
        SuccessColor = successColor;
        WarningColor = warningColor;
        DangerColor = dangerColor;
        WindowRounding = windowRounding;
        ButtonRounding = buttonRounding;
    }

    public void SetAsActiveTheme(bool darkMode)
    {
        _darkMode = darkMode;   
        _elementColorMap[ElementColor.LIGHT] = LightColor;
        _elementColorMap[ElementColor.DARK] = DarkColor;
        _elementColorMap[ElementColor.SECONDARY] = SecondaryColor;
        _elementColorMap[ElementColor.SUCCESS] = SuccessColor;
        _elementColorMap[ElementColor.WARNING] = WarningColor;
        _elementColorMap[ElementColor.DANGER] = DangerColor;
        _elementColorMap[ElementColor.PRIMARY] = PrimaryColor;
        _elementColorMap[ElementColor.CONTRAST] = _darkMode? LightColor : DarkColor;
        _elementColorMap[ElementColor.TRANSPARENT] = Color.FromArgb(0, 0, 0, 0);
        _elementColorMap[ElementColor.WHITE] = Color.White;
        _elementColorMap[ElementColor.BLACK] = Color.Black;

        foreach (var kvp in _elementColorMap)
        {
            bool useBlackText = ThemeUtils.UseBlackText(ThemeUtils.ColorToVector4(kvp.Value));
            _textColorMap[kvp.Key] = useBlackText ? _elementColorMap[ElementColor.DARK] : _elementColorMap[ElementColor.LIGHT];
        }
    }

    private static bool _darkMode = true;

    private static Dictionary<ElementColor, Color> _elementColorMap = new();
    private static Dictionary<ElementColor, Color> _textColorMap = new();

    private static Dictionary<ColorShades, float> _shadePctMap = new()
    {
        { ColorShades.SHADE_LIGHT, 0.25f }, { ColorShades.SHADE_DARK, 0.75f }, { ColorShades.SHADE_NORMAL, 0.25f },
    };

    public static Color GetBgColor(ColorShades shade = ColorShades.SHADE_NORMAL)
    {
        return GetShadedColor(_darkMode ? ElementColor.DARK : ElementColor.LIGHT);
    }

    public static Color GetBgTextColor(ColorShades shade = ColorShades.SHADE_NORMAL)
    {
        return GetShadedTextColor(_darkMode ? ElementColor.DARK : ElementColor.LIGHT, shade);
    }

    public static Color GetShadedColor(ElementColor color, ColorShades shade = ColorShades.SHADE_NORMAL)
    {
        if(shade == ColorShades.SHADE_NORMAL) return _elementColorMap[color];

        return ThemeUtils.LerpColor(_elementColorMap[color], _elementColorMap[shade == ColorShades.SHADE_LIGHT ? ElementColor.LIGHT : ElementColor.DARK], _shadePctMap[shade]);
    }

    public static Color GetShadedTextColor(ElementColor color, ColorShades shade = ColorShades.SHADE_NORMAL)
    {
        if(shade == ColorShades.SHADE_NORMAL) return _textColorMap[color];

        return ThemeUtils.LerpColor(_textColorMap[color], _textColorMap[shade == ColorShades.SHADE_LIGHT ? ElementColor.LIGHT : ElementColor.DARK], _shadePctMap[shade]);
    }
}

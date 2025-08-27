// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI;

public static class StyleSettings
{
    private static ThemePalette _activePalette;
    public static ThemePalette ActivePalette => _activePalette;
    static Dictionary<Type, ThemePalette> _palettes = new();
    public static List<StyleDefinition> Styles { get; set; } = new();
    private static bool _darkMode = true;
    public static bool DarkMode { get => _darkMode; set => _darkMode = value; }

    public static void Initialize()
    {
        //Here we collect the palettes and the attributes from the generated code and
        //create all the data that is needed

        SetActiveTheme<DefaultTheme>();
    }

    public static void SetActiveTheme<T>() where T : ThemePalette
    {
        if (!_palettes.ContainsKey(typeof(T))) throw new InvalidOperationException($"No palette registered for {typeof(T)} but you are trying to call it anyways");

        _activePalette = _palettes[typeof(T)];
        DefineTheme(ActivePalette, DarkMode);
    }

    public static void ToggleDarkMode()
    {
        _darkMode = !_darkMode;
        DefineTheme(ActivePalette, DarkMode);
    }

    public static void DefineTheme(ThemePalette activePalette, bool darkMode)
    {
        foreach (StyleDefinition style in Styles) style.DefineStyle(activePalette, darkMode);
    }
}

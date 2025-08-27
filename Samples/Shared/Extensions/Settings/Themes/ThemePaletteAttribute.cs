// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class ThemePaletteAttribute : Attribute
{
    public string ThemeName { get; }

    public ThemePaletteAttribute(string pThemeName)
    {
        ThemeName = pThemeName;
    }
}

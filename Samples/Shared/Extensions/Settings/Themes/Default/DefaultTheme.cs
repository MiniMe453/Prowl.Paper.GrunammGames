// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

namespace Prowl.PaperUI;

[ThemePalette("Default")]
public class DefaultTheme : ThemePalette
{
    public DefaultTheme()
    {
        SetColorPalette(
            Color.Azure,
            Color.Black,
            Color.Blue,
            Color.Chocolate,
            Color.Purple,
            Color.ForestGreen,
            Color.DarkOrange,
            Color.Red,
            10,
            5);
    }
}

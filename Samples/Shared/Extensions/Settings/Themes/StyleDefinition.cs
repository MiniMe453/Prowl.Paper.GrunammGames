// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI;

public abstract class StyleDefinition
{
    private static Paper _gui;

    public static void SetPaper(Paper gui)
    {
        _gui = gui;
    }

    public virtual void DefineStyle(ThemePalette palette, bool darkMode)
    {

    }
}

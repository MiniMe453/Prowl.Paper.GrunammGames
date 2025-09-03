// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;

public class CustomDropdown : Dropdown
{
    protected override void DrawDropdownContent(Paper gui)
    {
        gui.Box("Hello world")
            .Height(200)
            // .Text("Hello world")
            .BackgroundColor(Color.Brown);
    }
}

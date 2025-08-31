// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;

public class Modal : Component<Modal>, IPersistentState
{
    public override ElementBuilder DrawDefault() => ElementBuilder.Visible(_isVisible);
    private bool _isVisible = false;
    protected override Modal OnCreated()
    {
        ElementBuilder.BackgroundColor(Color.DimGray)
            .PositionType(PositionType.SelfDirected)
            .Top(100)
            .Left(100)
            .Layer(Layer.Overlay)
            .Rounded(5);
        return this;
    }

    public Modal SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
        return this;
    }

    public override Modal Draw()
    {
        return this;
    }

    public void Reset()
    {

    }

    public Modal Width(UnitValue unit)
    {
        ElementBuilder.Width(unit);
        return this;
    }

    public Modal Height(UnitValue unit)
    {
        ElementBuilder.Height(unit);
        return this;
    }
}

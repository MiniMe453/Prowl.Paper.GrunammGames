// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Origami.Styling;

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace Origami.Components;

public class Button : Component<Button>
{
    public override void Finish() => Origami.ReturnToPool(this);

    protected override Button OnCreated()
    {
        _elementBuilder.BorderWidth(1).BorderColor(System.Drawing.Color.Chartreuse).Margin(5);
        return this;
    }

    public override Button Draw()
    {
        return this;
    }
    public override void Reset()
    {

    }
    public Button Color(Colors color)
    {
        //TODO this should define the color, then we define the variant using a different function
        _elementBuilder.BackgroundColor(System.Drawing.Color.Aqua);
        return this;
    }
    public Button Radius(Rounding rounding)
    {
        //TODO eventually replace this with a connection to the styling system
        _elementBuilder.Rounded(5 * (int)rounding);
        return this;
    }

    public Button Variant(StyleVariants variant)
    {
        return this;
    }

    public Button Text(string text)
    {
        _elementBuilder.Text(text)
            .Alignment(TextAlignment.MiddleCenter)
            .Width(150)
            .Height(50);
        return this;
    }

    public Button Alignment(TextAlignment alignment)
    {
        _elementBuilder.Alignment(alignment);
        return this;
    }

    public Button Width(UnitValue unit)
    {
        _elementBuilder.Width(unit);
        return this;
    }

    public Button Height(UnitValue unit)
    {
        _elementBuilder.Height(unit);
        return this;
    }
}

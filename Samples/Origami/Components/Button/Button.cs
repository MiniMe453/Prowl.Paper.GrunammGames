// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.
using Prowl.PaperUI;
using Prowl.PaperUI.Events;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;


public class Button : Component<Button>, IPersistentState
{
    public bool IsWhiteButton = false;
    public override ElementBuilder DrawDefault() => ElementBuilder;
    private void OnClickEvent(ClickEvent e)
    {
        IsWhiteButton = !IsWhiteButton;
    }



    //TODO if the user tries to access the state here, it will fail.
    protected override Button OnCreated()
    {
        ElementBuilder.BorderWidth(1).BorderColor(System.Drawing.Color.Chartreuse).Margin(5);
        return this;
    }

    public Button OnClick(Action<ClickEvent> onClick)
    {
        ElementBuilder.OnClick(onClick);
        return this;
    }

    public override Button Draw()
    {
        return this;
    }

    public Button Color(Colors color)
    {
        //TODO this should define the color, then we define the variant using a different function
        ElementBuilder.BackgroundColor(IsWhiteButton? System.Drawing.Color.Aqua : System.Drawing.Color.DimGray);
        return this;
    }
    public Button Radius(Rounding rounding)
    {
        //TODO eventually replace this with a connection to the styling system
        ElementBuilder.Rounded(5 * (int)rounding);
        return this;
    }

    public Button Variant(StyleVariants variant)
    {
        switch (variant)
        {
            case StyleVariants.Faded:
                ElementBuilder.Style("faded");
                break;
            case StyleVariants.Flat:
                ElementBuilder.Style("flat");
                break;
            case StyleVariants.Solid:
                ElementBuilder.Style("solid");
                break;
            case StyleVariants.Transparent:
                ElementBuilder.Style("transparent");
                break;
        }

        return this;
    }

    public Button Text(string text)
    {
        ElementBuilder.Text(text)
            .Alignment(TextAlignment.MiddleCenter)
            .Width(150)
            .Height(50);
        return this;
    }

    public Button Alignment(TextAlignment alignment)
    {
        ElementBuilder.Alignment(alignment);
        return this;
    }

    public Button Width(UnitValue unit)
    {
        ElementBuilder.Width(unit);
        return this;
    }

    public Button Height(UnitValue unit)
    {
        ElementBuilder.Height(unit);
        return this;
    }

    public void Reset() => IsWhiteButton = false;
}

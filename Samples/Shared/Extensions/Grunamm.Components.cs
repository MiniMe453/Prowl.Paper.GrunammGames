// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI.Events;
using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;

using Shared;

namespace Prowl.PaperUI;
public partial class PaperComponents
{
    private Paper _gui;
    // private ReadOnlySpan

    public Paper InitializeComponentLibrary(Paper paper)
    {
        _gui = paper;
        return _gui;
    }

    private bool _showDropdownList = false;
    public bool ShowDropdownList
    {
        get { return _showDropdownList; }
        set
        {
            _showDropdownList = value;
            Console.WriteLine(_showDropdownList ? "Show Dropdown List" : "Hide Dropdown List");
        }
    }

    public void CloseDropdown(ElementEvent eventData)
    {
        if (!ShowDropdownList) return;
        ShowDropdownList = false;
    }

    public void ShowDropdown(ElementEvent eventData)
    {
        ShowDropdownList = !ShowDropdownList;
    }

    private void CalculateDropdownWidth(Element element, Rect rect)
    {
        _dropdownWidth = Math.Abs(rect.BottomLeft.x - rect.BottomRight.x);
    }

    public double _dropdownWidth;
    public ElementBuilder Dropdown(string id, string[] values, int index, Action<int> onNewSelection)
    {
        if (_gui == null) throw new NullReferenceException();

        var parent = _gui.Box(values[index])
            .Width(_gui.Stretch())
            .OnClick(ShowDropdown)
            .Height(40)
            .BackgroundColor(ShowDropdownList ? Themes.backgroundColor: Color.Black)
            .BorderWidth(ShowDropdownList ? 2 : 0)
            .BorderColor(Themes.primaryColor)
            .OnLeave(CloseDropdown)
            .OnPostLayout(CalculateDropdownWidth)
            .RoundedTop(5);

        var cleanup = parent.Enter();
        using (_gui.Row("Preview information")
                   .Margin(5, 0)
                   .Enter())
        {
            using (_gui.Box("PreviewBox")
                       .Text(Text.MiddleLeft(values[index], Fonts.fontMedium, ShowDropdownList? Themes.textColor : Themes.lightTextColor))
                       .Left(_gui.Pixels(5))
                       .Enter())
            { }

            using(_gui.Box("MenuItemIcon")
                      .Text(Text.MiddleRight(Icons.ArrowDown, Fonts.fontSmall, ShowDropdownList? Themes.textColor : Themes.lightTextColor))
                      .Enter()) {}

        }

        if (!ShowDropdownList)
        {
            cleanup.Dispose();
            return parent;
        }

        //We need to do this prepass so we can figure out what the width of the dropdown elements should be.
        double dropdownWidth = 0;
        for (int i = 0; i < values.Length; ++i)
        {
            dropdownWidth = Math.Max(dropdownWidth, Fonts.fontMedium.MeasureString(values[i]).X + 50);
        }

        using (_gui.Box("Dropdown")
                   .PositionType(PositionType.SelfDirected)
                   .Top(_gui.Percent(100, 1))
                   .Width(dropdownWidth)
                   .Height(values.Length * 35)
                   // .BorderWidth(2)
                   // .BorderColor(Themes.primaryColor)
                   .Layer(Layer.Overlay)
                   .Rounded(5)
                   .BoxShadow(0,6,16,-5,Color.FromArgb(128, Color.Black))
                   .Enter())
        {
            for (int i = 0; i < values.Length; i++)
            {
                //We need to store this in a temp variable, otherwise the action that we invoke
                //doesn't get correctly invoked by the code.
                int idx = i;
                bool isSelected = i == index;
                Color tabColor = isSelected ? Themes.primaryColor : Themes.backgroundColor;

                _gui.Box($"Tab_{i}")
                    .Width(dropdownWidth)
                    .Height(_gui.Stretch())
                    .Text(Text.MiddleCenter(values[i], Fonts.fontMedium,
                        isSelected ? Themes.textColor : Themes.lightTextColor))
                    .OnClick(e =>
                    {
                        onNewSelection.Invoke(idx);
                    })
                    .BackgroundColor(tabColor)
                    // .BorderWidth(1)
                    // .BorderColor(Color.FromArgb(50, Color.White))
                    .Rounded(i == 0 ? 5 : 0, i == 0 ? 5 : 0, i == values.Length - 1 ? 5 : 0,
                        i == values.Length - 1 ? 5 : 0);
            }
        }

        cleanup.Dispose();
        return parent;
    }
}

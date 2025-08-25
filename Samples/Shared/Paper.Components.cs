// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

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

    public bool _showDropdownList = false;
    public Vector2 _screenPositionDropdown;
    public double _dropdownWidth;
    public ElementBuilder Dropdown(string id, string[] values, int index, Action<int> onNewSelection)
    {
        if (_gui == null) throw new NullReferenceException();

        void DrawDropdown(Vector2 screenPosition, double width)
        {
            using (_gui.Box("Dropdown")
                       .PositionType(PositionType.SelfDirected)
                       .Position(screenPosition.x, screenPosition.y + 5)
                       .Width(width)
                       // .BackgroundColor(Color.Aqua)
                       .Height(values.Length * 35)
                       // .BorderWidth(2)
                       // .BorderColor(Themes.primaryColor)
                       .Rounded(5)
                       .BoxShadow(0,6,16,-5,Color.FromArgb(128, Color.Black))
                       .Enter())
            {

                _gui.MoveToRoot();
                for (int i = 0; i < values.Length; i++)
                {
                    //We need to store this in a temp variable, otherwise the action that we invoke
                    //doesn't get correctly invoked by the code.
                    int idx = i;
                    bool isSelected = i == index;
                    Color tabColor = isSelected ? Themes.primaryColor : Themes.backgroundColor;

                    _gui.Box($"Tab_{i}")
                        .Width(_gui.Stretch())
                        .Height(_gui.Stretch())
                        .Text(Text.MiddleCenter(values[i], Fonts.fontMedium,
                            isSelected ? Themes.textColor : Themes.lightTextColor))
                        .OnClick(e =>
                        {
                            onNewSelection.Invoke(idx);
                            _showDropdownList = false;
                        })
                        .BackgroundColor(tabColor)
                        // .BorderWidth(1)
                        // .BorderColor(Color.FromArgb(50, Color.White))
                        .Rounded(i == 0 ? 5 : 0, i == 0 ? 5 : 0, i == values.Length - 1 ? 5 : 0,
                            i == values.Length - 1 ? 5 : 0);
                }
            }
        }

        if (!_showDropdownList)
        {
            var parent = _gui.Box(id)
                .BackgroundColor(Color.Black)
                .Height(40)
                .OnClick(e => _showDropdownList = !_showDropdownList)
                .OnLeave(e => _showDropdownList = false);
            var cleanup = parent.Enter();

            using (_gui.Row("Preview information")
                       .Margin(5, 0)
                       // .Width(_gui.Percent(100))
                       .Enter())
            {
                using (_gui.Box("PreviewBox")
                           .Text(Text.MiddleLeft(values[index], Fonts.fontMedium, Themes.lightTextColor))
                           .Left(_gui.Pixels(5))
                           .Enter())
                { }

                using(_gui.Box($"MenuItemIcon")
                         .Text(Text.MiddleRight(Icons.ArrowDown, Fonts.fontSmall, Themes.lightTextColor))
                         .Enter()) {}

            }

            cleanup.Dispose();
            return parent;
        }
        else
        {
            var parent = _gui.Box(values[index])
                .Width(_gui.Stretch())
                .OnClick(e => _showDropdownList = !_showDropdownList)
                .Height(40)
                .BackgroundColor(Themes.backgroundColor)
                .BorderWidth(2)
                .BorderColor(Themes.primaryColor)
                .OnPostLayout((e, rect) =>
                {
                    _screenPositionDropdown = rect.BottomLeft;
                    _dropdownWidth = Math.Abs(rect.BottomLeft.x - rect.BottomRight.x);
                })
                .RoundedTop(5);

            var cleanup = parent.Enter();
            using (_gui.Row("Preview information")
                       .Margin(5, 0)
                       .Enter())
            {
                using (_gui.Box("PreviewBox")
                           .Text(Text.MiddleLeft(values[index], Fonts.fontMedium, Themes.textColor))
                           .Left(_gui.Pixels(5))
                           .Enter())
                { }

                using(_gui.Box("MenuItemIcon")
                          .Text(Text.MiddleRight(Icons.ArrowDown, Fonts.fontSmall, Themes.textColor))
                          .Enter()) {}

            }
            cleanup.Dispose();
            DrawDropdown(_screenPositionDropdown, _dropdownWidth);
            return parent;
        }
    }
}

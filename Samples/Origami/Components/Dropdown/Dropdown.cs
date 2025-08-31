// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI;
using Prowl.PaperUI.Events;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;

public class Dropdown : Component<Dropdown>, IPersistentState
{
    private bool _isOpened = false;
    private Action<int> _onItemSelected;
    private int _selectedIdx;
    private List<string> _values = new();

    public override void Finish()
    {
        Draw();
    }

    private void ShowDropdown(ClickEvent e)
    {
        _isOpened = !_isOpened;
    }

    protected override Dropdown OnCreated()
    {
        ElementBuilder.OnClick(ShowDropdown)
            .OnLeave(e => _isOpened = false);
        return this;
    }

    public Dropdown SetData(List<string> values, int selectedIdx, Action<int> onItemSelected)
    {
        _values = values;
        _selectedIdx = selectedIdx;
        _onItemSelected = onItemSelected;
        return this;
    }

    public override Dropdown Draw()
    {
        Paper gui = Origami.Gui;
        var cleanup = ElementBuilder.Enter();
        using (gui.Row(PaperId.Next())
                   .Margin(5, 0)
                   .Enter())
        {
            using (gui.Box(PaperId.Next())
                       .Text(_values[_selectedIdx])
                       .Alignment(TextAlignment.MiddleCenter)
                       .Left(gui.Pixels(5))
                       .Enter())
            { }

            // using(gui.Box("MenuItemIcon")
            //           .Text(Icons.ArrowDown)
            //           .Enter()) {}
        }
        if (_isOpened)
        {
            using (gui.Box(PaperId.Next())
                       .PositionType(PositionType.SelfDirected)
                       .Top(gui.Percent(100, 1))
                       .Width(250)
                       .Height(UnitValue.Auto)
                       // .BorderWidth(2)
                       // .BorderColor(Themes.primaryColor)
                       .Layer(Layer.Overlay)
                       .Rounded(5)
                       .BoxShadow(0, 6, 16, -5, Color.FromArgb(128, Color.Black))
                       .Enter())
            {
                DrawDropdownContent(gui);
            }
        }

        cleanup.Dispose();
        return this;
    }

    protected virtual void DrawDropdownContent(Paper gui)
    {
            for (int i = 0; i < _values.Count; i++)
            {
                //We need to store this in a temp variable, otherwise the action that we invoke
                //doesn't get correctly invoked by the code.
                int idx = i;
                bool isSelected = i == _selectedIdx;
                Color tabColor = isSelected ? Color.Aqua : Color.DimGray;

                gui.Box(PaperId.Next())
                    .Width(250)
                    .Height(35)
                    .Text(_values[i])
                    .Alignment(TextAlignment.MiddleCenter)
                    .OnClick(e =>
                    {
                        _onItemSelected.Invoke(idx);
                    })
                    .BackgroundColor(tabColor)
                    // .BorderWidth(1)
                    // .BorderColor(Color.FromArgb(50, Color.White))
                    .Rounded(i == 0 ? 5 : 0, i == 0 ? 5 : 0, i == _values.Count - 1 ? 5 : 0,
                        i == _values.Count - 1 ? 5 : 0);
            }
    }

    public Dropdown Width(UnitValue unit)
    {
        ElementBuilder.Width(unit);
        return this;
    }

    public Dropdown Height(UnitValue unit)
    {
        ElementBuilder.Height(unit);
        return this;
    }

    public void Reset()
    {
        _isOpened = false;
        _onItemSelected = null;
        _selectedIdx = 0;
        _values.Clear();;
    }
}

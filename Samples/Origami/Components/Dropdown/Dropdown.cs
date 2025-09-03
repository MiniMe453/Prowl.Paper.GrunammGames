// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI;
using Prowl.PaperUI.Events;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;

[Flags]
public enum DropdownFlags
{

}

public class Dropdown : Component<Dropdown>
{
    private Action<int> _onItemSelected;
    private int _selectedIdx;
    private string _displayValue;
    private List<string> _values = new();

    private const string IS_OPENED_KEY = "IsOpened";
    private const string USE_SIMPLE_DROPDOWN = "UseSimpleDropdown";

    public override ElementBuilder DrawDefault()
    {
        if (!Origami.Gui.GetElementStorage<bool>(ElementBuilder._handle, USE_SIMPLE_DROPDOWN))
            throw new InvalidOperationException(
                "In order to use DrawDefault(), you must use ActiveIndex() and SetValues()");

        using (ContentBox().Enter())
        {
            DrawDropdownContent(Origami.Gui);
        }
        return ElementBuilder;
    }

    private void ShowDropdown(ClickEvent e)
    {
        Origami.Gui.SetElementStorage(ElementBuilder._handle, IS_OPENED_KEY,
            !Origami.Gui.GetElementStorage<bool>(ElementBuilder._handle, IS_OPENED_KEY));

        bool key = Origami.Gui.GetElementStorage<bool>(ElementBuilder._handle, IS_OPENED_KEY);
    }

    private void CloseDropdown(ElementEvent e)
    {
        Origami.Gui.SetElementStorage(ElementBuilder._handle, IS_OPENED_KEY, false);
    }

    protected override Dropdown OnCreated()
    {
        ElementBuilder.OnLeave(CloseDropdown);
        if(!Origami.Gui.HasElementStorage(ElementBuilder._handle, IS_OPENED_KEY))
        {
            Origami.Gui.SetElementStorage(ElementBuilder._handle, IS_OPENED_KEY, false);
            Origami.Gui.SetElementStorage(ElementBuilder._handle, USE_SIMPLE_DROPDOWN, false);
        }

        return this;
    }

    public ElementBuilder ContentBox()
    {
        if (String.IsNullOrEmpty(_displayValue))
            throw new InvalidOperationException(
                "You must call DisplayValue(string) before rendering anything to the screen");

        Paper gui = Origami.Gui;
        using(ElementBuilder.Enter())
        {
            using (gui.Row("Preview Information")
                       .Margin(5, 0)
                       .OnClick(ShowDropdown)
                       .Enter())
            {
                gui.Box("Preview Text")
                    // .Text(_displayValue)
                    .Alignment(TextAlignment.MiddleLeft)
                    .Left(gui.Pixels(5));

                // using(gui.Box("MenuItemIcon")
                //           .Text(Icons.ArrowDown)
                //           .Enter()) {}
            }

            return gui.Box("Dropdown Content Box")
                .PositionType(PositionType.SelfDirected)
                .Top(gui.Percent(100, 1))
                .Width(250)
                .Height(UnitValue.Auto)
                .Visible(Origami.Gui.GetElementStorage<bool>(ElementBuilder._handle, IS_OPENED_KEY))
                .Layer(Layer.Topmost)
                .Rounded(5)
                .BoxShadow(0, 6, 16, -5, Color.FromArgb(128, Color.Black));
        }
    }

    public Dropdown SetValues(List<string> values, Action<int> onItemSelected, int idx)
    {
        _onItemSelected = onItemSelected;
        _values = values;
        _selectedIdx = idx;
        Origami.Gui.SetElementStorage(ElementBuilder._handle, USE_SIMPLE_DROPDOWN, true);
        return this;
    }

    public Dropdown DisplayValue(string value)
    {
        _displayValue = value;
        return this;
    }

    public override Dropdown Draw()
    {
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

                gui.Box("Dropdown Item", i)
                    .Width(250)
                    .Height(35)
                    // .Text(_values[i])
                    .Alignment(TextAlignment.MiddleCenter)
                    .OnClick(e =>
                    {
                        _onItemSelected.Invoke(idx);
                        Origami.Gui.SetElementStorage(ElementBuilder._handle, IS_OPENED_KEY, false);
                    })
                    .BackgroundColor(tabColor)
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

    // public void Reset()
    // {
    //     Origami.Gui.SetElementStorage(ElementBuilder._element, IS_OPENED_KEY, false);
    //     _onItemSelected = null;
    //     _selectedIdx = 0;
    //     _values.Clear();
    //     _displayValue = "";
    //     Origami.Gui.SetElementStorage(ElementBuilder._element, USE_SIMPLE_DROPDOWN, false);
    // }
}

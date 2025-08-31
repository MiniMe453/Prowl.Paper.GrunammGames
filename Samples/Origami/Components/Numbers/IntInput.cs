// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Prowl.PaperUI;
using Prowl.PaperUI.Events;
using Prowl.PaperUI.LayoutEngine;

namespace OrigamiUI;

[Flags]
public enum NumberInputFlags
{

}

public class IntInput : Component<IntInput>
{
    private int _min = Int32.MinValue;
    private int _max = Int32.MaxValue;
    private bool _useRange;
    private Action<int> _onValueChanged;
    private int _currValue;

    private void OnInputFieldDragged(DragEvent e)
    {
        int deltaX = (int)Math.Round(e.Delta.x);

        _onValueChanged?.Invoke(CheckValueRange(_currValue + deltaX, _min, _max));
    }

    private void IncrementValue(ClickEvent e)
    {
        _onValueChanged?.Invoke(CheckValueRange(++_currValue, _min, _max));
    }

    private void Decrementvalue(ClickEvent e)
    {
        _onValueChanged?.Invoke(CheckValueRange(--_currValue, _min, _max));
    }

    private int CheckValueRange(int currValue, int min, int max)
    {
        if (currValue < min) currValue = min;
        if (currValue > max) currValue = max;

        return currValue;
    }

    public override ElementBuilder DrawDefault()
    {
        Paper gui = Origami.Gui;
        using (ElementBuilder.Enter())
        {
            gui.Box("IntInput")
                .BackgroundColor(Color.Black)
                .OnDragging(OnInputFieldDragged)
                .Text(_currValue.ToString())
                .Alignment(TextAlignment.MiddleCenter);

            using (gui.Column("IncrementingColumns")
                       .Width(20)
                       .Enter())
            {
                gui.Box("IncreaseButton")
                    .OnClick(IncrementValue)
                    .Margin(1)
                    .BackgroundColor(Color.Aqua);

                gui.Box("DecreaseButton")
                    .OnClick(Decrementvalue)
                    .Margin(1)
                    .BackgroundColor(Color.Aqua);
            }
        }

        return ElementBuilder;
    }

    protected override IntInput OnCreated()
    {
        ElementBuilder.LayoutType(LayoutType.Row);
        return this;
    }

    public IntInput SetData(int currValue, Action<int> onValueChanged)
    {
        _currValue = currValue;
        _onValueChanged = onValueChanged;
        return this;
    }

    public override IntInput Draw()
    {
        return this;
    }

    public IntInput SetRange(int min, int max)
    {
        _min = min;
        _max = max;
        _useRange = true;

        return this;
    }

    public IntInput Width(UnitValue unit)
    {
        ElementBuilder.Width(unit);
        return this;
    }

    public IntInput Height(UnitValue unit)
    {
        ElementBuilder.Height(unit);
        return this;
    }
}

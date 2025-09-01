// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI.LayoutEngine;

namespace Prowl.PaperUI;

public partial class Paper
{
    // private ObjectPool<ElementBuilder> _builderPool = new ObjectPool<ElementBuilder>();
    private List<ElementBuilder> _builderPool = new();
    private int _currentBuilderIndex = 0;

    private List<ElementStyle> _stylePool = new();
    private int _currentStyleIndex;

    internal ElementStyle GetStyleFromPool()
    {
        ElementStyle style;
        if (_currentStyleIndex >= _stylePool.Count)
        {
            style = new ElementStyle();
            _stylePool.Add(style);
        }
        else
        {
            style = _stylePool[_currentStyleIndex];
        }

        _currentStyleIndex++;
        style.RemoveFromPool();
        return style;
    }

    private void EndOfFramePoolCleanup()
    {
        UnitValue.Free();
        _currentBuilderIndex = 0;
        _currentStyleIndex = 0;
    }
}

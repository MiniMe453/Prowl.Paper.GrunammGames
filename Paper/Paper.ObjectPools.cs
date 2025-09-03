// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI.LayoutEngine;

namespace Prowl.PaperUI;

public partial class Paper
{
    // private ObjectPool<ElementBuilder> _builderPool = new ObjectPool<ElementBuilder>();
    private List<ElementBuilder> _builderPool = new();
    private int _currentBuilderIndex = 0;
    public int _buildersLastFrame = 0;

    internal ElementBuilder GetBuilderFromPool(ElementHandle handle)
    {
        _currentBuilderIndex++;
        if (_currentBuilderIndex >= _builderPool.Count)
        {
            _builderPool.Add(new ElementBuilder(this, handle));
            return _builderPool[_currentBuilderIndex - 1];
        }
        else
        {
            return _builderPool[_currentBuilderIndex].SetData(this, handle);
        }

        return null;
    }

    // internal ElementStyle GetStyleFromPool(ulong id)
    // {
    //     ElementStyle style;
    //     if (_currentStyleIndex >= _stylePool.Count)
    //     {
    //         style = new ElementStyle();
    //         _stylePool.Add(style);
    //         _styleOwnerIds.Add(id);
    //     }
    //     else
    //     {
    //         style = _stylePool[_currentStyleIndex];
    //
    //         if (_styleOwnerIds[_currentStyleIndex] != id)
    //         {
    //             style.ReturnToPool();
    //             _styleOwnerIds[_currentStyleIndex] = id;
    //         }
    //     }
    //
    //     _currentStyleIndex++;
    //
    //     return style;
    // }

    private void EndOfFramePoolCleanup()
    {
        // UnitValue.ResetCount();
        stylesLastFrame = _currentStyleIndex;
        _buildersLastFrame = _builderPool.Count;
        _currentBuilderIndex = 0;
        _currentStyleIndex = 0;
    }
}

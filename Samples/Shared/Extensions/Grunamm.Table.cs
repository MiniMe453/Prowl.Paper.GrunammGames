// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Drawing;

using Shared;

namespace Prowl.PaperUI;

public struct ColumnInfo
{
    public string Name;
    public int Width;
}
public partial class PaperComponents
{
    public ElementBuilder Table(string id, ref List<ColumnInfo> columnInfos, Action drawTableRow)
    {
        var parent = _gui.Box(id)
            .SetScroll(Scroll.ScrollXY);

        var cleanup = parent.Enter();

        using (_gui.Row($"header_row_{id}")
                   .Width(_gui.Auto)
                   .Height(Fonts.fontMedium.FontSize + 20)
                   .Enter())
        {
            for (int i = 0; i < columnInfos.Count; i++)
            {
                var columnInfo = columnInfos[i];
                _gui.Box($"header_row_{i}")
                    .Text(Text.MiddleLeft(columnInfo.Name, Fonts.fontMedium))
                    .Width(columnInfo.Width)
                    .BorderWidth(1)
                    .BorderColor(Color.White)
                    .BackgroundColor(Color.Indigo)
                    .Clip();
            }
        }


        cleanup.Dispose();
        return parent;
    }
}

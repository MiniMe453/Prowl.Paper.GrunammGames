// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI;

namespace OrigamiUI;

public class AccordianItem : Component<AccordianItem>
{
    public override ElementBuilder Finish() => ElementBuilder;

    protected override AccordianItem OnCreated() => throw new NotImplementedException();

    public override AccordianItem Draw() => throw new NotImplementedException();
}

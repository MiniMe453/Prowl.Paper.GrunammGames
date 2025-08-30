// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Origami.Components;

public class AccordianItem : Component<AccordianItem>
{
    public override void Finish() => Origami.ReturnToPool(this);

    protected override AccordianItem OnCreated() => throw new NotImplementedException();

    public override AccordianItem Draw() => throw new NotImplementedException();

    public override void ResetComponent() => throw new NotImplementedException();
}

// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI;

public class StyleDefinitionAttribute : Attribute
{
    public string Name;

    public StyleDefinitionAttribute(string name)
    {
        Name = name;
    }
}

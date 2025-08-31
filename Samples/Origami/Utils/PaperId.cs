// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System.Runtime.InteropServices;
using System.Text;

namespace OrigamiUI;

public static class PaperId
{
    private static int _currentId;

    private static List<string> _idPool = new();

    public static int NumberAccessedLastFrame;
    public static int MaxNumberAccessed => _idPool.Count - 1;

    //TODO - refactor this to use a string builder and 0 allocation strings. This is super fucked
    // right now because it returns a value between 0-255, not a proper byte array.
    // that should have been obvious, but wasn't for a while so fml.
    // on the bright side, if we change the output to string, then it shouldn't break
    // any existing uses of the ID system ;)
    // for info on how to do this, follow the tutorial posted in the Hexa.NET best practices channel
    public static string Next()
    {
        _currentId++;

        if(_idPool.Count <= _currentId)
               _idPool.Add(_currentId.ToString());

        return _idPool[_currentId - 1];
    }

    public static string Current()
    {
        return _idPool[CurrentInt()];
    }


    /// <summary>
    /// Increments the current ID by 1.
    /// </summary>
    /// <returns>int -> the next ID</returns>
    private static int NextInt()
    {
        return _currentId++;
    }

    /// <summary>
    /// This should only be used for ImNode and ImPlot!
    /// </summary>
    /// <returns>int -> the current ID</returns>
    private static int CurrentInt()
    {
        return _currentId;
    }

    public static void Reset()
    {
        NumberAccessedLastFrame = _currentId;
        _currentId = 0;
    }
}

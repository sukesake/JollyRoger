using UnityEngine;
using System.Collections;

public static class WindowIDManager
{
    private static int m_NextWindowId = 1000;
    public static int GetWindowsID()
    {
        return m_NextWindowId++;
    }
}

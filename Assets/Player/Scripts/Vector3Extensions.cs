using System;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 ScaleIt(this Vector3 sourceVector, Vector3 scalar)
    {
        sourceVector.Scale(scalar);
        return sourceVector;
    }

    public static Vector3 WorldSpaceIt(this Vector3 sourceVector, Quaternion transformRotation)
    {
        return transformRotation*sourceVector;
    }

    public static Vector3 ClampIt(this Vector3 sourceVector, float maxMagnitude)
    {
        if (sourceVector.magnitude > maxMagnitude)
        {
            sourceVector.Normalize();
            sourceVector *= maxMagnitude;
        }

        return sourceVector;
    }

    public static Vector3 ClampIt(this Vector3 sourceVector, float xClamp, float zClamp, float yClamp)
    {
        xClamp = Math.Abs(xClamp);
        zClamp = Math.Abs(zClamp);
        yClamp = Math.Abs(yClamp);

        sourceVector.x = Mathf.Clamp(sourceVector.x, -xClamp, xClamp);
        sourceVector.z = Mathf.Clamp(sourceVector.z, -zClamp, zClamp);
        sourceVector.y = Mathf.Clamp(sourceVector.y, -yClamp, yClamp);
        return sourceVector;
    }
}
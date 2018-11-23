using System;
using UnityEngine;

public class Vector3Utility
{
    public static float DistanceSquared(Vector3 to, Vector3 from)
    {
        Vector3 _ref = to - from;
        return (_ref.x * _ref.x) + (_ref.y * _ref.y) + (_ref.z + _ref.z);
    }
}
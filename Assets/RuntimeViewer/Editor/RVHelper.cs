using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public static class RVHelper
{

    public static Component[] GetComponent(GameObject data)
    {
        Component[] c = data.GetComponents<Component>();

        if (c == null || c.Length == 0)
            return null;

        return c;
    }

    public static bool IsNormalType(Type t)
    {
        if (t == null)
            return false;

        if (typeof(Enum).IsAssignableFrom(t) == true)
            return true;

        if (t.IsValueType == true && t.IsPrimitive == true)
        {
            return true;
        }

        return false;
    }

    public static bool IsString(Type t)
    {
        if (typeof(string).IsAssignableFrom(t) == true)
            return true;

        if (typeof(String).IsAssignableFrom(t) == true)
            return true;

        return false;
    }

    public static bool IsEnum(Type t)
    {
        return typeof(Enum).IsAssignableFrom(t);
    }

    //是否是个字典 or 集合
    public static bool IsCollection(Type type)
    {
        if (typeof(Enum).IsAssignableFrom(type) == true)
            return false;

        if (typeof(IDictionary).IsAssignableFrom(type) == true ||
            typeof(ICollection).IsAssignableFrom(type) == true)
        {
            return true;
        }
        return false;
    }

    public static bool IsCanToStringDirently(Type t)
    {
        if (RVHelper.IsNormalType(t) == true ||
            RVHelper.IsString(t) == true ||
            typeof(Vector2).IsAssignableFrom(t) == true ||
            typeof(Color).IsAssignableFrom(t) == true ||
            typeof(Color32).IsAssignableFrom(t) == true ||
            typeof(Vector4).IsAssignableFrom(t) == true ||
            typeof(Quaternion).IsAssignableFrom(t) == true ||
            typeof(Vector3).IsAssignableFrom(t) == true)
            return true;

        return false;
    }
}

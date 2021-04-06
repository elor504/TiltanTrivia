using System;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunc
{
    static System.Random _random = new System.Random();
    /// Used in Shuffle(T).
    /// </summary>
    public static void Shuffle<T>(ref List<T> list)
    {
        if (list.Count == 0 || list == null)
            return;
        for (int i = 0; i < list.Count - 1; i++)
        {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(list.Count - i);
            T t = list[r];
            list[r] = list[i];
            list[i] = t;
        }
    }
    public static void Shuffle<T>(ref T[] array) {
        if (array.Length == 0)
            return;
        for (int i = 0; i < array.Length - 1; i++) {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(array.Length - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }
    public static void NotImplementedError() => Debug.LogError("NOT IMPLEMENETED!");
}

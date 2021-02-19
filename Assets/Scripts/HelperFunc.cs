using System;
public static class HelperFunc
{
    static Random _random = new Random();
    /// Used in Shuffle(T).
    /// </summary>
    public static void ArrayShuffle<T>(ref T[] array) {
        int n = array.Length;
        for (int i = 0; i < (n - 1); i++) {
            // Use Next on random instance with an argument.
            // ... The argument is an exclusive bound.
            //     So we will not go past the end of the array.
            int r = i + _random.Next(n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }

    }
}

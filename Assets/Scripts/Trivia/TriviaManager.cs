using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriviaManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Shuffle(ref array);
        //Console.WriteLine("SHUFFLE: {0}", string.Join(",", array));

        string[] array2 = { "bird", "frog", "cat","Kaki" };
        Shuffle(ref array2);


        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log(array[i].ToString());
        }
        for (int i = 0; i < array2.Length; i++)
        {
            Debug.Log(array2[i].ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public string[] AnswerRandomizer(string Answer1,string Answer2,string Answer3,string Answer4)
    {
        string[] ShuffleCache = new string[4];
        ShuffleCache[0] = Answer1;
        ShuffleCache[1] = Answer2;
        ShuffleCache[2] = Answer3;
        ShuffleCache[3] = Answer4;
        Shuffle(ref ShuffleCache);

        return ShuffleCache;
    }



    /// Used in Shuffle(T).
    /// </summary>
    static System.Random _random = new System.Random();


    void Shuffle<T>(ref T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < (n - 1); i++)
        {
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

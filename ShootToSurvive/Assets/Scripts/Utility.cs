using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random psudoRandomNumberGenerator = new System.Random(seed);

        for(int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = psudoRandomNumberGenerator.Next(i, array.Length);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }
}

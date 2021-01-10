using System;
using System.Collections.Generic;
using System.Linq;

public static class StringExtension
{
    public static string Anagram(this string str, bool keepwords=true, List<int> KeepIndexes = null) // Note use of this
    {
        List<string> listOfWords = new List<string>();
        if (keepwords) listOfWords = str.Split(' ', '\t', '\n').ToList();
        else listOfWords.Add(str);

        if (KeepIndexes == null) KeepIndexes = new List<int>();
        var minIndexes = 0;

        for (int ii = 0; ii < listOfWords.Count; ii++)
        {
            var maxIndexes = listOfWords[ii].Length+minIndexes;
            var RangeIndexes = KeepIndexes.Where(x => x >= minIndexes && x <= maxIndexes).ToList();
            string attempt = Shuffle(listOfWords[ii], RangeIndexes);
            var counter = 0;
            while (attempt == listOfWords[ii] && counter < 9)
            {
                attempt = Shuffle(listOfWords[ii], RangeIndexes);
                counter++;
            }
            listOfWords[ii] = attempt;
            minIndexes = maxIndexes;
        }
        var fullanagram = "";
        foreach (var item in listOfWords)
        {
            if (fullanagram.Length > 0) fullanagram += " " + item;
            else fullanagram = item;
        }
        return fullanagram;
    }

    // Based on something we got from the web, not re-written for clarity
    private static string Shuffle(string str, List<int> keepIndexes = null)
    {
        if (keepIndexes == null) keepIndexes = new List<int>();
        char[] characters = str.ToCharArray();
        System.Random randomRange = new System.Random();
        int numberOfCharacters = characters.Length;
        while (numberOfCharacters > 1)
        {
            numberOfCharacters--;
            int index = randomRange.Next(numberOfCharacters + 1);
            if (keepIndexes.Contains(numberOfCharacters) || keepIndexes.Contains(index)) continue;
            var value = characters[index];
            characters[index] = characters[numberOfCharacters];
            characters[numberOfCharacters] = value;
        }
        return new string(characters);
    }
}
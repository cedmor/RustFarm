﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustFarm.CodeHelper
{
    public class PermuteUtils
    {
        // Returns an enumeration of enumerators, one for each permutation
        // of the input.
        public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }

        // Returns an enumeration of enumerators, one for each permutation
        // of the input.
        public static IEnumerable<IEnumerable<T>> PermuteUnique<T>(IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in PermuteUnique(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    List<T> tempList = list.ToList();
                    tempList.RemoveAt(0);
                    list = tempList;
                }
            }
        }

        // Enumerates over contents of both lists.
        public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
        }

        // Enumerates over all items in the input, skipping over the item
        // with the specified offset.
        public static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
        {
            int index = 0;
            foreach (T item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }
    }

    class AppTestPermutUtils
    {
        static void Main2(string[] args)
        {
            int[] intInput = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ShowPermutations<int>(intInput, 2);

            string[] stringInput = { "Hello", "World", "Foo" };
            ShowPermutations<string>(stringInput, 3);

        }

        // Print out the permutations of the input 
        static void ShowPermutations<T>(IEnumerable<T> input, int count)
        {
            foreach (IEnumerable<T> permutation in PermuteUtils.Permute<T>(input, count))
            {
                foreach (T i in permutation)
                {
                    Console.Write(" " + i);
                }
                Console.WriteLine();
            }
        }
    }
}

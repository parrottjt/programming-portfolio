// See https://aka.ms/new-console-template for more information

using DataStructuresAndAlgorithms;
using DataStructuresAndAlgorithms.DataStructures;
using DataStructuresAndAlgorithms.Interfaces;
using DataStructuresAndAlgorithms.Sorting;

public class Program
{
    public static void Main(string[] args)
    {
        var list = new List<int>
        {
            6, 8, 3, 5, 12, 51, 745, 14, 0
        };

        var sort = new QuickSort();

        foreach (var i in sort.Sort(list))
        {
            Console.WriteLine(i);
        }
    }
}
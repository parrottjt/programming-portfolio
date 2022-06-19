// See https://aka.ms/new-console-template for more information

using DataStructuresAndAlgorithms;
using DataStructuresAndAlgorithms.Interfaces;

public class Program
{
    public static void Main(string[] args)
    {
        var list = new []{1,2,3,4,5,6,7,8,9,10};
        var target = 10;

        var search = new BinarySearch();
        
        var index = search.Search(list, target);
        var recursiveIndex = search.RecursiveSearch(list, target, 0, list.Length - 1);
        
        Console.WriteLine(search.Verify(index));
        Console.WriteLine(search.Verify(recursiveIndex));
    }

    
}
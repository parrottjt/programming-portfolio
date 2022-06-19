using DataStructuresAndAlgorithms.Interfaces;

namespace DataStructuresAndAlgorithms;

public class BinarySearch : ISearch
{
    public int Search<T>(IEnumerable<T> list, T target)
    {
        var arr = list.ToList();
        var first = 0;
        var last = arr.Count - 1;

        while (first <= last)
        {
            var midpoint = (first + last) / 2;
            var compare = Comparer<T>.Default.Compare(arr[midpoint], target);
            switch (compare)
            {
                case 0:
                    return midpoint;
                case < 0:
                    first = midpoint + 1;
                    break;
                case > 0:
                    last = midpoint - 1;
                    break;
            }
        }

        return -1;
    }

    public int RecursiveSearch<T>(IEnumerable<T> list, T target, int first, int last)
    {
        var arr = list.ToList();
        if (first > last)
        {
            return -1;
        }
        
        var midpoint = (first + last) / 2;
        var compare = Comparer<T>.Default.Compare(arr[midpoint], target);
        return compare switch
        {
            0 => midpoint,
            < 0 => RecursiveSearch(arr, target, midpoint + 1, last),
            > 0 => RecursiveSearch(arr, target, first, midpoint - 1)
        };
    }

    public string Verify(int index)
    {
        var value = index == -1 ? "Target was not found" : $"Target was found at index {index}";

        return value;
    }
}
using DataStructuresAndAlgorithms.Interfaces;

namespace DataStructuresAndAlgorithms;

public class LinearSearch : ISearch
{
    public int Search<T>(IEnumerable<T> list, T target)
    {
        var arr = list.ToArray();
        for (var i = 0; i < arr.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(arr[i], target))
            {
                return i;
            }
        }
        return -1;
    }
    
    public string Verify(int index)
    {
        var value = index == -1 ? "Target was not found" : $"Target was found at index {index}";

        return value;
    }
}
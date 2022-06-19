namespace DataStructuresAndAlgorithms.Sorting;

public class MergeSort
{
    public IEnumerable<T> Sort<T>(IEnumerable<T> list)
    {
        if (list.Count() <= 1)
            return list;
        
        SplitAtMidPoint(list.ToArray(), out var first, out var second);
        
        var left = Sort<T>(first);
        var right = Sort<T>(second);

        return Merge(left.ToArray(), right.ToArray());
    }

    void SplitAtMidPoint<T>(T[] list, out T[] first, out T[] second)
    {
        Split(list, list.Length/2, out first, out second);
    }

    void Split<T>(T[] list, int index, out T[] first, out T[] second)
    {
        first = list.Take(index).ToArray();
        second = list.Skip(index).ToArray();
    }

    IEnumerable<T> Merge<T>(T[] left, T[] right)
    {
        var list = new List<T>();
        var i = 0;
        var j = 0;

        while (i < left.Count() && j < right.Count())
        {
            var compare = Comparer<T>.Default.Compare(left[i], right[j]);
            if (compare < 0)
            {
                list.Add(left[i]);
                i += 1;
            }
            else
            {
                list.Add(right[j]);
                j += 1;
            }
        }

        while (i < left.Length)
        {
            list.Add(left[i]);
            i += 1;
        }
        while (j < right.Length)
        {
            list.Add(right[j]);
            j += 1;
        }

        return list;
    }

    public bool Verify<T>(T[] list)
    {
        if (list.Length <= 1) return true;
        var compare = Comparer<T>.Default.Compare(list[0], list[1]);

        Console.WriteLine(list[0]);
        return compare <= 0 && Verify<T>(list.Skip(1).ToArray());
    }
}
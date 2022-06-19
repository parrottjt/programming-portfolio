using DataStructuresAndAlgorithms.DataStructures;
using DataStructuresAndAlgorithms.Utils;

namespace DataStructuresAndAlgorithms.Sorting;

public class LinkedListMergedSort
{
    public LinkedListPractice<T> MergeSort<T>(LinkedListPractice<T> linkedList)
    {
        if (linkedList.IsEmpty || linkedList.Size() == 1)
        {
            return linkedList;
        }

        Split(linkedList, out var leftHalf, out var rightHalf);

        var left = MergeSort<T>(leftHalf);
        var right = MergeSort<T>(rightHalf);

        return Merge(left, right);
    }

    void Split<T>(LinkedListPractice<T> linkedList, out LinkedListPractice<T> left, out LinkedListPractice<T> right)
    {
        if (linkedList.IsEmpty || linkedList.Size() == 1)
        {
            left = linkedList;
            right = null;
        }

        var size = linkedList.Size();
        var mid = size / 2;

        var midNode = linkedList.NodeAt(mid - 1);

        left = linkedList;
        right = new LinkedListPractice<T>();
        right.Head = midNode.NextNode;
        midNode.NextNode = null;
    }
    
    LinkedListPractice<T> Merge<T>(LinkedListPractice<T> left, LinkedListPractice<T> right)
    {
        var merged = new LinkedListPractice<T>();
        merged.Add(default);

        var current = merged.Head;
        var leftHead = left.Head;
        var rightHead = right.Head;

        while (leftHead.isNotNull() || rightHead.isNotNull())
        {
            if (leftHead.isNull())
            {
                current.NextNode = rightHead;
                rightHead = rightHead.NextNode;
            }
            else if (rightHead.isNull())
            {
                current.NextNode = leftHead;
                leftHead = leftHead.NextNode;
            }
            else
            {
                var leftData = leftHead.Data;
                var rightData = rightHead.Data;
                var compare = Comparer<T>.Default.Compare(leftData, rightData);
                if (compare <= 0)
                {
                    current.NextNode = leftHead;
                    leftHead = leftHead.NextNode;
                }
                else
                {
                    current.NextNode = rightHead;
                    rightHead = rightHead.NextNode;
                }
            }
            current = current.NextNode;
        }

        var head = merged.Head.NextNode;
        merged.Head = head;
        
        return merged;
    }
}
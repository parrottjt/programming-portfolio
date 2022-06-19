using DataStructuresAndAlgorithms.Utils;
using Microsoft.VisualBasic;

namespace DataStructuresAndAlgorithms.DataStructures;

public class LinkedListPractice<T>
{
    public Node<T> Head { get; set; }
    public bool IsEmpty => Head == null;

    public int Size()
    {
        var current = Head;
        var count = 0;
        while (current.isNotNull())
        {
            count += 1;
            current = current.NextNode;
        }

        return count;
    }

    public void Add(T data)
    {
        var node = new Node<T>(data)
        {
            NextNode = Head!
        };
        Head = node;
    }

    public Node<T> Search(T key)
    {
        var current = Head;

        while (current.isNotNull())
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, key))
            {
                return current;
            }
            current = current.NextNode;
        }

        return null!;
    }

    public void Insert(T data, int index)
    {
        if (index == 0)
        {
            Add(data);
            return;
        }
        
        var current = Head;
        var position = 1;
            
        while (position < index)
        {
            position += 1;
            current = current.NextNode;
        }
            
        var newNode = new Node<T>(data)
        {
            NextNode = current.NextNode
        };
        current.NextNode = newNode;

    }

    public Node<T> Remove(T data)
    {
        var current = Head;
        var prev = current;
        var found = false;

        while (current.isNotNull() && !found)
        {
            if (EqualityComparer<T>.Default.Equals(current.Data, data))
            {
                if (current == Head)
                {
                    Head = current.NextNode;
                }
                prev.NextNode = current.NextNode;
                found = true;
            }
            else
            {
                prev = current;
                current = current.NextNode;
            }
        }

        return current;
    }

    public Node<T> NodeAt(int index)
    {
        if (index == 0) return Head;

        var current = Head;
        var position = 0;
            
        while (position < index)
        {
            position += 1;
            current = current.NextNode;
        }

        return current;
    }

    public override string ToString()
    {
        List<string> value = new List<string>();

        var current = Head;
        while (current.isNotNull())
        {
            if (current == Head)
            {
                value.Add($"[Head: {current.Data}]");
            }
            else if(current.NextNode.isNull())
            {
                value.Add($"[Tail: {current.Data}]");
            }
            else
            {
                value.Add($"[{current.Data}]");
            }
            
            current = current.NextNode;
        }
        
        return String.Join(" -> " ,value);
    }
}
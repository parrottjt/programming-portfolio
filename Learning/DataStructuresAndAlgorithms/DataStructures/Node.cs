namespace DataStructuresAndAlgorithms.DataStructures;

public class Node<T>
{
    public readonly T Data;
    public Node<T>? NextNode { get; set; }

    public Node(T data)
    {
        Data = data;
    }

    public override string ToString()
    {
        return $"<Node Data: {Data?.ToString()}>";
    }
}
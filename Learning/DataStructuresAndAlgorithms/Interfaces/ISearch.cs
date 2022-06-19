namespace DataStructuresAndAlgorithms.Interfaces;

public interface ISearch
{
    public int Search<T>(IEnumerable<T> list, T target);
    public string Verify(int index);
}
namespace DataStructuresAndAlgorithms.Utils;

public static class Extentions
{
    public static bool isNull<T>(this T value) => value == null;
    public static bool isNotNull<T>(this T value) => value.isNull() == false;
}
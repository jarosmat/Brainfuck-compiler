namespace Utilities;

public static class NotRelatedFunctions
{
    public static T PopFromList<T>(List<T> list)
    {
        var lastItem = list.Count - 1;
        var itemPopped = list[lastItem];
        list.RemoveAt(lastItem);
        return itemPopped;
    }
}
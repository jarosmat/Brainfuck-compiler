namespace Utilities;

public class NotRelatedFunctions
{
    public static T PopFromList<T>(List<T> list)
    {
        int lastItem = list.Count - 1;
        T itemPopped = list[lastItem];
        list.RemoveAt(lastItem);
        return itemPopped;
    }
}
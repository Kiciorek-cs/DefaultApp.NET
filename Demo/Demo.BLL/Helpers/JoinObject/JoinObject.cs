using System.Collections.Generic;

namespace Demo.BLL.Helpers.JoinObject;

public static class JoinObject
{
    public static List<T> JoinList<T>(params List<T>[] lists)
    {
        var joinedList = new List<T>();
        foreach (var list in lists) joinedList.AddRange(list);

        return joinedList;
    }
}
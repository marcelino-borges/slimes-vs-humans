using System;
using System.Collections.Generic;
using System.Linq;

public enum Language
{
    Null,
    English,
    Portuguese,
    Spanish,
    Japonese,
    Chinese,
    Korean,
    Italian,
    Russian,
    Thai,
    French,
    Deutsch
}

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

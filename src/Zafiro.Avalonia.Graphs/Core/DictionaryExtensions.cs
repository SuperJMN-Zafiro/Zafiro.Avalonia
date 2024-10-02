using System;
using System.Collections.Generic;

namespace Graphs;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TValue, TKey>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> calculate)
    {
        if (dict.TryGetValue(key, out var v))
        {
            return v;
        }

        var calculated = calculate();
        dict[key] = calculated;

        return calculated;
    }
}
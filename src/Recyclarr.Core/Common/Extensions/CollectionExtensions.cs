using System.Collections;

namespace Recyclarr.Common.Extensions;

public static class CollectionExtensions
{
    // From: https://stackoverflow.com/a/34362585/157971
    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source as IReadOnlyCollection<T> ?? new ReadOnlyCollectionAdapter<T>(source);
    }

    // From: https://stackoverflow.com/a/34362585/157971
    private sealed class ReadOnlyCollectionAdapter<T>(ICollection<T> source)
        : IReadOnlyCollection<T>
    {
        public int Count => source.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            destination.Add(item);
        }
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(x => x is not null).Select(x => x!);
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source, Func<T, bool> predicate)
        where T : class
    {
        return source.Where(x => x is not null && predicate(x)).Select(x => x!);
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
        where T : struct
    {
        return source.Where(x => x is not null).Select(x => x!.Value);
    }

    public static bool IsEmpty<T>(this ICollection<T>? collection)
    {
        return collection is null or { Count: 0 };
    }

    public static bool IsEmpty<T>(this IReadOnlyCollection<T>? collection)
    {
        return collection is null or { Count: 0 };
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection is not null && collection.Any();
    }

    public static IList<T>? ToListOrNull<T>(this IEnumerable<T> source)
    {
        var list = source.ToList();
        return list.Count != 0 ? list : null;
    }

    public static IEnumerable<T> Flatten<T>(
        this IEnumerable<T> items,
        Func<T, IEnumerable<T>> flattenWhich
    )
    {
        return items.SelectMany(x => flattenWhich(x).Flatten(flattenWhich).Append(x));
    }

    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Task<TResult>> method
    )
    {
        return await Task.WhenAll(source.Select(async s => await method(s)));
    }
}

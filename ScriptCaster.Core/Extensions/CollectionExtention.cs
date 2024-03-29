namespace ScriptCaster.Core.Extensions;

public static class CollectionExtensions
{
	public static void AddRangeOverride<TKey, TValue>(this IDictionary<TKey, TValue> dic,
		IDictionary<TKey, TValue> dicToAdd)
	{
		dicToAdd.ForEach(x => dic[x.Key] = x.Value);
	}

	public static void AddRangeNewOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic,
		IDictionary<TKey, TValue> dicToAdd)
	{
		dicToAdd.ForEach(x =>
		{
			if (!dic.ContainsKey(x.Key))
			{
				dic.Add(x.Key, x.Value);
			}
		});
	}

	public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
	{
		dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
	}

	public static bool ContainsKeys<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
	{
		var result = false;
		keys.ForEachOrBreak(x =>
		{
			result = dic.ContainsKey(x);
			return result;
		});
		return result;
	}

	private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			action(item);
		}
	}

	private static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
	{
		foreach (var item in source)
		{
			var result = func(item);
			if (result)
			{
				break;
			}
		}
	}
}

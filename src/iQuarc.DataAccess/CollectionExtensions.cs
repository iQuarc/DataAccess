using System.Collections.Generic;

namespace iQuarc.DataAccess
{
	internal static class CollectionExtensions
	{
		public static void AddIfNotExists<T>(this ICollection<T> list, T item)
		{
			if (!list.Contains(item))
				list.Add(item);
		}
	}
}
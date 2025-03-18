using System.Collections.Generic;
using System.Linq;

namespace MailAutoconf.Util
{
    /// <summary>
    /// This static class contains extensions for enumerations.
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Returns an enumeration with the passed item as only item.
        /// </summary>
		public static IEnumerable<T> OneItemEnum<T>(this T item)
		{
			yield return item;
		}
		
        /// <summary>
		/// Returns the items not being null.
		/// </summary>
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> items)
            => items
                .Where(item => item != null)
                .Cast<T>();

        /// <summary>
        /// Returns the items not being null or empty (empty string).
        /// </summary>
        public static IEnumerable<T> NotEmpty<T>(this IEnumerable<T> items)
            => items.Where(item => !IsNullOrEmpty(item));

        private static bool IsNullOrEmpty(object obj) => obj is string s
            ? string.IsNullOrEmpty(s)
            : obj == null;
	}
}

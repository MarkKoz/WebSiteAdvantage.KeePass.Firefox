using System.Collections.Generic;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Extension methods for various types.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Skips all elements of a sequence that throw an exception upon retrieval.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> of which to skip exceptions.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the elements which did not throw an exception.</returns>
        public static IEnumerable<TSource> SkipExceptions<TSource>(this IEnumerable<TSource> source)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                var next = true;

                while (next)
                {
                    try
                    {
                        next = enumerator.MoveNext();
                    }
                    catch
                    {
                        continue;
                    }

                    if (next)
                        yield return enumerator.Current;
                }
            }
        }
    }
}

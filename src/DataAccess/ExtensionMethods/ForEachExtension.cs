namespace Helpers.ExtensionMethods
{
    public static class ForEachExtension
    {
        /// <summary>
        ///     Custom foreach for <seealso cref="IEnumerable{T}"/> type.
        /// </summary>
        /// <typeparam name="TSource">
        ///     Type of source
        /// </typeparam>
        /// <param name="sources">
        ///     Source for enumerating through,
        /// </param>
        /// <param name="action">
        ///     Action to do while enumarating through,
        /// </param>
        public static void ForEach<TSource>(
            this IEnumerable<TSource> sources,
            Action<TSource> action
        )
        {
            foreach (var item in sources)
            {
                action(obj: item);
            }
        }
    }
}

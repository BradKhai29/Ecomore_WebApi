namespace DataAccess.Commons.SystemConstants
{
    public class DefaultValues
    {
        public const string UserAvatarUrl =
            "https://firebasestorage.googleapis.com/v0/b/comic-image-storage.appspot.com/o/blank-profile-picture-973460_1280.png?alt=media&token=2309abba-282c-4f81-846e-6336235103dc";

        /// <summary>
        ///     Id that represents for the system.
        /// </summary>
        public static readonly Guid SystemId = new("1111aaaa-1111-aaaa-1111-aaaa1111aaaa");

        public static readonly DateTime MinDateTime = new DateTime(1753, 1, 1);
    }
}

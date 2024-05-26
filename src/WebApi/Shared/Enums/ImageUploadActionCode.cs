namespace WebApi.Shared.Enums
{
    /// <summary>
    ///     The action code of the image item 
    ///     when doing update.
    /// </summary>
    public enum ImageUploadActionCode
    {
        /// <summary>
        ///     The image item is kept as the original.
        /// </summary>
        Keep = 0,

        /// <summary>
        ///     The image item will be added new.
        /// </summary>
        AddNew = 1,

        /// <summary>
        ///     The image item is update with other image.
        /// </summary>
        Update = 2,

        /// <summary>
        ///     The image item is removed.
        /// </summary>
        Delete = 3,

        /// <summary>
        ///     Represent the invalid or undefined action code.
        /// </summary>
        Undefined = 4,
    }
}

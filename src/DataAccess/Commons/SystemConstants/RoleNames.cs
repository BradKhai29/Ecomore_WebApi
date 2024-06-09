namespace DataAccess.Commons.SystemConstants
{
    public static class RoleNames
    {
        public const string System = "system";

        public const string Admin = "admin";

        public const string Employee = "employee";

        /// <summary>
        ///     This role is only used for situation
        ///     when one of important role is removed by accident
        ///     and need to recover the consistence quickly.
        /// </summary>
        public const string DoNotRemove = "do_not_remove";
    }
}

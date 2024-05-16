using DataAccess.Commons.SystemConstants;

namespace DataAccess.DataSeedings
{
    public static class AppRoles
    {
        public static class System
        {
            public static readonly Guid Id = DefaultValues.SystemId;

            public const string Name = RoleNames.System;
        }

        public static class DoNotRemove
        {
            public static readonly Guid Id = new("3120575b-9f22-4330-9f73-8ac89ba3a15c");

            public const string Name = RoleNames.DoNotRemove;
        }

        public static class Employee
        {
            public static readonly Guid Id = new("cc751bfc-77b9-4a97-85d4-c88e1f3db4de");

            public const string Name = RoleNames.Employee;
        }

        public static class Admin
        {
            public static readonly Guid Id = new("c4cc80c2-c5b1-4852-8f32-f59c6d5b2213");

            public const string Name = RoleNames.Admin;
        }
    }
}

using DataAccess.Entities;

namespace DataAccess.DataSeedings
{
    public static class AccountStatuses
    {
        /// <summary>
        ///     Represent for the pending-to-confirm status.
        /// </summary>
        public static class PendingConfirmed
        {
            public static readonly Guid Id = new("2d92a50b-6d0a-46f1-9f55-53de6b585600");

            public const string Name = "Pending Confirmed";
        }

        /// <summary>
        ///     Represent for the email-confirmed-success status.
        /// </summary>
        public static class EmailConfirmed
        {
            public static readonly Guid Id = new("cc751bfc-77b9-4a97-85d4-c88e1f3db4de");

            public const string Name = "Email Confirmed";
        }

        public static class Banned
        {
            public static readonly Guid Id = new("c4cc80c2-c5b1-4852-8f32-f59c6d5b2213");

            public const string Name = "Banned";
        }

        #region Public Methods
        private static IEnumerable<AccountStatusEntity> _values;
        private static readonly object _lock = new();

        public static IEnumerable<AccountStatusEntity> GetValues()
        {
            const int totalStatuses = 3;

            lock (_lock)
            {
                if (_values == null)
                {
                    _values = new List<AccountStatusEntity>(capacity: totalStatuses)
                    {
                        new() { Id = PendingConfirmed.Id, Name = PendingConfirmed.Name },
                        new() { Id = EmailConfirmed.Id, Name = EmailConfirmed.Name },
                        new() { Id = Banned.Id, Name = Banned.Name },
                    };
                }

                return _values;
            }
        }
        #endregion
    }
}

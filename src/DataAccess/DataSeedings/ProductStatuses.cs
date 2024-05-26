using DataAccess.Entities;

namespace DataAccess.DataSeedings
{
    public static class ProductStatuses
    {
        /// <summary>
        ///     Represent for the in-stock status.
        /// </summary>
        public static class InStock
        {
            public static readonly Guid Id = new("2d92a50b-6d0a-46f1-9f55-53de6b585600");

            public const string Name = "In Stock";
        }

        /// <summary>
        ///     Represent for the out-of-stock status.
        /// </summary>
        public static class OutOfStock
        {
            public static readonly Guid Id = new("cc751bfc-77b9-4a97-85d4-c88e1f3db4de");

            public const string Name = "Out Of Stock";
        }

        /// <summary>
        ///     Used when the product is removed or disable,
        ///     this status will be set to that product.
        /// </summary>
        public static class NotAvailable
        {
            public static readonly Guid Id = new("c4cc80c2-c5b1-4852-8f32-f59c6d5b2213");

            public const string Name = "Not Available";
        }

        #region Public Methods
        private static IEnumerable<ProductStatusEntity> _values;
        private static readonly object _lock = new object();

        public static IEnumerable<ProductStatusEntity> GetValues()
        {
            const int totalStatuses = 3;

            lock (_lock)
            {
                if (_values == null)
                {
                    _values = new List<ProductStatusEntity>(capacity: totalStatuses)
                    {
                        new() { Id = InStock.Id, Name = InStock.Name },
                        new() { Id = OutOfStock.Id, Name = OutOfStock.Name },
                        new() { Id = NotAvailable.Id, Name = NotAvailable.Name },
                    };
                }

                return _values;
            }
        }

        public static bool IsStatusExistedById(Guid statusId)
        {
            if (statusId == InStock.Id)
            {
                return true;
            }

            if (statusId == OutOfStock.Id)
            {
                return true;
            }

            if (statusId == NotAvailable.Id)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}

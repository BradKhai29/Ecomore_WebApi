using DataAccess.Commons.SystemConstants;
using DataAccess.Entities;
using Helpers.ExtensionMethods;

namespace DataAccess.DataSeedings
{
    public static class Categories
    {
        public static class ProcessedNut
        {
            public static readonly Guid Id = new("2d92a50b-6d0a-46f1-9f55-53de6b5856af");

            public const string Name = "Hạt qua chế biến";

            public const string ImageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717941508/Ecomore/Categories/processed_nuts_c5fbby.png";
        }

        public static class Nut
        {
            public static readonly Guid Id = new("7d92a50b-ad04-46f1-9f55-53de6b585633");

            public const string Name = "Hạt thô";

            public const string ImageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717941508/Ecomore/Categories/nuts_qt1drr.png";
        }

        public static class NutSnack
        {
            public static readonly Guid Id = new("7d92a50b-ad04-46f1-9f55-b3de6b585699");

            public const string Name = "Snack hạt";

            public const string ImageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717941508/Ecomore/Categories/snacks_w707k8.png";
        }

        public static class NutMilk
        {
            public static readonly Guid Id = new("7d92a50b-ad04-46f1-9f55-53ac6b585611");

            public const string Name = "Sữa hạt";

            public const string ImageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717941508/Ecomore/Categories/nuts_milk_mvhnwp.png";
        }

        #region Public Methods
        private static IEnumerable<CategoryEntity> _values;
        private static readonly object _lock = new();

        public static IEnumerable<CategoryEntity> GetValues()
        {
            const int totalCategories = 4;

            lock (_lock)
            {
                if (_values == null)
                {
                    _values = new List<CategoryEntity>(capacity: totalCategories)
                    {
                        new() { Id = Nut.Id, Name = Nut.Name, ImageUrl = Nut.ImageUrl },
                        new() { Id = ProcessedNut.Id, Name = ProcessedNut.Name, ImageUrl = ProcessedNut.ImageUrl },
                        new() { Id = NutSnack.Id, Name = NutSnack.Name, ImageUrl = NutSnack.ImageUrl },
                        new() { Id = NutMilk.Id, Name = NutMilk.Name, ImageUrl = NutMilk.ImageUrl },
                    };
                }

                var dateTimeUtcNow = DateTime.UtcNow;
                _values.ForEach(category =>
                {
                    category.CreatedBy = DefaultValues.SystemId;
                    category.CreatedAt = dateTimeUtcNow;
                });

                return _values;
            }
        }
        #endregion 
    }
}

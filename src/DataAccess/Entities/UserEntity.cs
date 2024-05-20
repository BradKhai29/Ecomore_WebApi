using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class UserEntity : IdentityUser<Guid>, IGuidEntity
    {
        public const string NameSeparator = "[:]";
        public const string AssetIdPlaceHolder = "assetId";
        public const string AvatarUrlSeparator = ";";
        public static readonly int NameSeparatorLength = NameSeparator.Length;

        public string FullName { get; set; }

        public string AvatarUrl { get; set; }

        public bool Gender { get; set; }

        public Guid AccountStatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        #region Public Methods
        public string FirstName()
        {
            return GetFirstName(FullName);
        }

        public string LastName()
        {
            return GetLastName(FullName);
        }

        public static string GetFullName(string firstName, string lastName)
        {
            return $"{firstName}{NameSeparator}{lastName}";
        }

        public string GetFullNameWithoutSeparator()
        {
            var nameItems = FullName.Split(NameSeparator);

            var firstName = nameItems[0];
            var lastName = nameItems[1];

            return $"{lastName} {firstName}";
        }

        public static string GetFirstName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || !fullName.Contains(NameSeparator))
            {
                return string.Empty;
            }

            int firstNameIndex = fullName.IndexOf(NameSeparator);

            return fullName[..firstNameIndex];
        }

        public static string GetLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || !fullName.Contains(NameSeparator))
            {
                return string.Empty;
            }

            int lastNameIndex = fullName.IndexOf(NameSeparator) + NameSeparatorLength;

            return fullName[lastNameIndex..];
        }
        #endregion

        #region Relationships
        public AccountStatusEntity AccountStatus { get; set; }

        public IEnumerable<OrderEntity> Orders { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Users";
        }
        #endregion
    }
}

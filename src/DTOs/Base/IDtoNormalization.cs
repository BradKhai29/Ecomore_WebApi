namespace DTOs.Base;

/// <summary>
///     This class contains methods to implement the data normalization on Dto.
/// </summary>
public interface IDtoNormalization
{
    /// <summary>
    ///     Normalize all properties of this DTO object
    ///     to assure no property contains redundant value.
    /// </summary>
    void NormalizeAllProperties();
}

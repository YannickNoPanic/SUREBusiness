namespace SUREBusiness.Core.Interfaces;

public interface ILicensePlateValidator
{
    Task<bool> IsValidAsync(string licensePlate);
}
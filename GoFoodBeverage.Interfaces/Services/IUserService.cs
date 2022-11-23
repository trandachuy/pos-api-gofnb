using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces
{
    public interface IUserService
    {
        bool PasswordValidation(string currentPassword, out Account account);
        bool GoAppPasswordValidation(string currentPassword, out Account account);
    }
}

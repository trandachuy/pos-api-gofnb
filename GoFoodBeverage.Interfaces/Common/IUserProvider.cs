using GoFoodBeverage.Common.Models.User;

namespace GoFoodBeverage.Interfaces
{
    public interface IUserProvider : IProvider<LoggedUserModel>
    {
        LoggedUserModel Provide();

        LoggedUserModel GetLoggedUserModelFromJwt(string token);

        string GetPlatformId();

        #region Gofood App

        LoggedUserModel GetLoggedCustomer();

        #endregion
    }
}

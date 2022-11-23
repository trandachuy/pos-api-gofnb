using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using GoFoodBeverage.Interfaces;

namespace GoFoodBeverage.Services.User
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeService _dateTimeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private bool EnableUserActivityLogging;

        public UserActivityService(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            EnableUserActivityLogging = _configuration.GetValue<bool>("EnableUserActivityLogging");
        }

        public async Task LogAsync(string activityName)
        {
            if (!EnableUserActivityLogging) return;

            var accountId = GetAccountIdFromJwt();
            var platformId = GetPlatformId();
            var userActivity = new UserActivity()
            {
                AccountId = accountId,
                ActivityName = activityName,
                Platform = platformId,
                PreviousData = string.Empty,
                NewData = string.Empty,
                Time = _dateTimeService.NowUtc
            };

            try
            {
                var usedTimeFromHeader = _httpContextAccessor.HttpContext.Request.Headers[DefaultConstants.USED_TIME];
                var method = _httpContextAccessor.HttpContext.Request.Method;
                var usedTime = usedTimeFromHeader.ToString();
                var time = int.TryParse(usedTime, out int number) ? number : 0;

                /// Update GET last activity
                if(!string.IsNullOrEmpty(method) && method.Equals("GET"))
                {
                    var lastestActivity = await _unitOfWork.UserActivities.Find(u => u.AccountId == accountId && u.ActivityName.Contains("GET")).OrderByDescending(i => i.Time).FirstOrDefaultAsync();
                    if (lastestActivity != null && lastestActivity.UsedTime == 0 && time > 0)
                    {
                        lastestActivity.UsedTime = time;
                        _unitOfWork.UserActivities.Update(lastestActivity);
                    }

                    _unitOfWork.UserActivities.Add(userActivity);
                    await _unitOfWork.SaveChangesAsync();
                }

                /// TODO: Implement update for POST, PUT, DELETE methods
                /// Code here
            }
            catch { }
        }

        public async Task LogAsync<T>(T request)
        {
            if (!EnableUserActivityLogging) return;

            try
            {
                var activityName = GetActivityName();
                var accountId = GetAccountIdFromJwt();
                var platformId = GetPlatformId();
                var userActivity = new UserActivity()
                {
                    AccountId = accountId,
                    ActivityName = activityName,
                    Platform = platformId,
                    PreviousData = string.Empty,
                    NewData = request.ToJson(),
                    Time = _dateTimeService.NowUtc
                };

                await _unitOfWork.UserActivities.AddAsync(userActivity);
            }
            catch { }
        }

        public async Task LogAsync<T>(T previousData, T newData)
        {
            if (!EnableUserActivityLogging) return;

            try
            {
                var accountId = GetAccountIdFromJwt();
                var platformId = GetPlatformId();
                var activityName = GetActivityName();
                var userActivity = new UserActivity()
                {
                    AccountId = accountId,
                    ActivityName = activityName,
                    Platform = platformId,
                    PreviousData = previousData.ToJson(),
                    NewData = newData.ToJson(),
                    Time = _dateTimeService.NowUtc
                };

                await _unitOfWork.UserActivities.AddAsync(userActivity);
            }
            catch { }
        }

        private Guid? GetAccountIdFromJwt()
        {
            var accountId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
            return accountId?.Value.ToGuid();
        }

        private string GetPlatformId()
        {
            var platform = _httpContextAccessor.HttpContext.Request.Headers[DefaultConstants.PLATFORM_ID];
            return platform;
        }

        private string GetActivityName()
        {
            var method = _httpContextAccessor.HttpContext.Request.Method;
            var routeData = _httpContextAccessor.HttpContext.GetRouteData();
            var actionName = routeData.Values["action"];
            var controllerName = routeData.Values["controller"];
            var activityName = $"{method}-{controllerName}-{actionName}";

            return activityName;
        }
    }
}

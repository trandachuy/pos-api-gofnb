using System;
using System.Linq;
using Serilog.Events;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;


namespace GoFoodBeverage.Loging.Serilog
{
    public class OperationTelemetryConverter : TraceTelemetryConverter
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            foreach (ITelemetry telemetry in base.Convert(logEvent, formatProvider))
            {
                if (logEvent.Properties.ContainsKey("UserId"))
                {
                    telemetry.Context.User.Id = logEvent.Properties["UserId"].ToString();
                }
                if (logEvent.Properties.ContainsKey("RoleName"))
                {
                    var roleName = logEvent.Properties["RoleName"]?.ToString().Trim(new char[] { '"' });
                    telemetry.Context.Cloud.RoleName = roleName;
                    telemetry.Context.Cloud.RoleInstance = roleName;
                }
                ISupportProperties propTelematry = (ISupportProperties)telemetry;
                var removeProps = new[] { "UserId" };
                removeProps = removeProps.Where(prop => propTelematry.Properties.ContainsKey(prop)).ToArray();

                foreach (var prop in removeProps)
                {
                    propTelematry.Properties.Remove(prop);
                }

                yield return telemetry;
            }

        }
    }
}

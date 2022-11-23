using System;

namespace AzureFunctionTrigger.Helpers
{
    public class CommonHelper
    {
        public static string GetEnvironmentVariable(string variableKey)
        {
            return Environment.GetEnvironmentVariable(variableKey, EnvironmentVariableTarget.Process);
        }
    }
}

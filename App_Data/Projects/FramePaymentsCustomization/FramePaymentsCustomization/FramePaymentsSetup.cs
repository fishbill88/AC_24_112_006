using System;
using System.Collections.Generic;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using PX.Objects.CA;

namespace FramePaymentsIntegration
{
    /// <summary>
    /// Frame Payments Processing Center configuration and setup
    /// </summary>
    public static class FramePaymentsSetup
    {
        public const string PluginTypeName = Messages.PluginTypeName;
        public const string PluginDisplayName = Messages.PluginDisplayName;
        
        /// <summary>
        /// Configuration settings for Frame Payments processing center
        /// </summary>
        public static class Settings
        {
            public const string ApiKey = Messages.SettingApiKey;
            public const string ApiUrl = Messages.SettingApiUrl;
            public const string Environment = Messages.SettingEnvironment;
            public const string WebhookUrl = Messages.SettingWebhookUrl;
            public const string TimeoutSeconds = Messages.SettingTimeoutSeconds;
        }

        /// <summary>
        /// Default settings values
        /// </summary>
        public static class Defaults
        {
            public const string ProductionApiUrl = Messages.DefaultApiUrl;
            public const string SandboxApiUrl = Messages.DefaultSandboxApiUrl;
            public const string DefaultEnvironment = Messages.DefaultEnvironment;
            public const int DefaultTimeoutSeconds = Messages.DefaultTimeoutSeconds;
            public const string DefaultTimeoutSecondsString = "30"; // String version for default values
        }

        /// <summary>
        /// Gets the required settings for Frame Payments processing center
        /// </summary>
        public static IEnumerable<SettingsField> GetRequiredSettings()
        {
            return new List<SettingsField>
            {
                new SettingsField
                {
                    DetailID = Settings.ApiKey,
                    DisplayName = Messages.ApiKey,
                    IsRequired = true,
                    IsPassword = true,
                    ControlType = SettingsControlType.Text,
                    DefaultValue = Messages.EmptyString
                },
                new SettingsField
                {
                    DetailID = Settings.Environment,
                    DisplayName = Messages.Environment,
                    IsRequired = true,
                    ControlType = SettingsControlType.Combo,
                    DefaultValue = Defaults.DefaultEnvironment,
                    ComboValues = new[] { Messages.Sandbox, Messages.Production }
                },
                new SettingsField
                {
                    DetailID = Settings.ApiUrl,
                    DisplayName = Messages.ApiUrl,
                    IsRequired = false,
                    ControlType = SettingsControlType.Text,
                    DefaultValue = Defaults.SandboxApiUrl
                },
                new SettingsField
                {
                    DetailID = Settings.WebhookUrl,
                    DisplayName = Messages.WebhookUrl,
                    IsRequired = false,
                    ControlType = SettingsControlType.Text,
                    DefaultValue = Messages.EmptyString
                },
                new SettingsField
                {
                    DetailID = Settings.TimeoutSeconds,
                    DisplayName = Messages.TimeoutSeconds,
                    IsRequired = false,
                    ControlType = SettingsControlType.Text,
                    DefaultValue = Defaults.DefaultTimeoutSecondsString // Use string version
                }
            };
        }

        /// <summary>
        /// Validates Frame Payments settings
        /// </summary>
        public static void ValidateSettings(IEnumerable<SettingsValue> settings)
        {
            var settingsDict = new Dictionary<string, string>();
            foreach (var setting in settings)
            {
                settingsDict[setting.DetailID] = setting.Value;
            }

            // Validate API Key
            if (!settingsDict.ContainsKey(Settings.ApiKey) || string.IsNullOrWhiteSpace(settingsDict[Settings.ApiKey]))
            {
                throw new PXException(Messages.FramePaymentsApiKeyRequired);
            }

            // Validate Environment
            if (settingsDict.ContainsKey(Settings.Environment))
            {
                var environment = settingsDict[Settings.Environment];
                if (environment != Messages.Sandbox && environment != Messages.Production)
                {
                    throw new PXException(Messages.EnvironmentMustBeSandboxOrProduction);
                }
            }

            // Validate API URL format
            if (settingsDict.ContainsKey(Settings.ApiUrl) && !string.IsNullOrWhiteSpace(settingsDict[Settings.ApiUrl]))
            {
                if (!Uri.TryCreate(settingsDict[Settings.ApiUrl], UriKind.Absolute, out Uri apiUri))
                {
                    throw new PXException(Messages.ApiUrlMustBeValid);
                }
            }

            // Validate timeout
            if (settingsDict.ContainsKey(Settings.TimeoutSeconds) && !string.IsNullOrWhiteSpace(settingsDict[Settings.TimeoutSeconds]))
            {
                if (!int.TryParse(settingsDict[Settings.TimeoutSeconds], out int timeout) || timeout <= 0)
                {
                    throw new PXException(Messages.TimeoutMustBePositiveNumber);
                }
            }
        }

        /// <summary>
        /// Gets the effective API URL based on environment setting
        /// </summary>
        public static string GetEffectiveApiUrl(IEnumerable<SettingsValue> settings)
        {
            var settingsDict = new Dictionary<string, string>();
            foreach (var setting in settings)
            {
                settingsDict[setting.DetailID] = setting.Value;
            }

            // If API URL is explicitly set, use it
            if (settingsDict.ContainsKey(Settings.ApiUrl) && !string.IsNullOrWhiteSpace(settingsDict[Settings.ApiUrl]))
            {
                return settingsDict[Settings.ApiUrl];
            }

            // Otherwise, determine based on environment
            var environment = settingsDict.ContainsKey(Settings.Environment) ? settingsDict[Settings.Environment] : Defaults.DefaultEnvironment;
            
            return environment == Messages.Production ? Defaults.ProductionApiUrl : Defaults.SandboxApiUrl;
        }
    }

    /// <summary>
    /// Settings field definition for Frame Payments
    /// </summary>
    public class SettingsField
    {
        public string DetailID { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsPassword { get; set; }
        public SettingsControlType ControlType { get; set; }
        public string DefaultValue { get; set; }
        public string[] ComboValues { get; set; }
    }

    /// <summary>
    /// Settings control types
    /// </summary>
    public enum SettingsControlType
    {
        Text,
        Combo,
        CheckBox
    }
}
using System;
using System.Collections.Generic;
using PX.CCProcessingBase.Interfaces.V2;
using PX.Data;
using System.Linq;
using System.ComponentModel;
using PX.Common;

namespace FramePaymentsIntegration
{
    /// <summary>
    /// Simplified Frame Payments Plugin implementing basic V2 processing interface
    /// </summary>
    [DisplayName("Frame Payments")]
    public class FrameProcessingCenterPlugin : ICCProcessingPlugin
    {
        public T CreateProcessor<T>(IEnumerable<SettingsValue> settings) where T : class
        {
            if (settings == null) return default(T);
            
            // Only return null for now - this is a simplified implementation
            // In a full implementation, you would create actual processors here
            return default(T);
        }

        public IEnumerable<SettingsDetail> ExportSettings()
        {
            return new List<SettingsDetail>
            {
                new SettingsDetail
                {
                    DetailID = Messages.SettingApiKey,
                    Descr = Messages.FrameApiKey,
                    DefaultValue = Messages.EmptyString,
                    ControlType = PX.CCProcessingBase.Interfaces.V2.SettingsControlType.Text
                },
                new SettingsDetail
                {
                    DetailID = Messages.SettingEnvironment, 
                    Descr = Messages.Environment,
                    DefaultValue = Messages.Sandbox,
                    ControlType = PX.CCProcessingBase.Interfaces.V2.SettingsControlType.Combo,
                    ComboValues = new Dictionary<string, string>
                    {
                        { Messages.Sandbox, Messages.Sandbox },
                        { Messages.Production, Messages.Production }
                    }
                }
            };
        }

        public string ValidateSettings(SettingsValue setting)
        {
            if (setting?.DetailID == Messages.SettingApiKey && string.IsNullOrWhiteSpace(setting.Value))
            {
                return Messages.FrameApiKeyRequired;
            }
            return null;
        }

        public void TestCredentials(IEnumerable<SettingsValue> settings)
        {
            if (settings == null)
            {
                throw new PXException(Messages.FrameApiKeyRequiredForTesting);
            }

            var settingsDict = settings.ToDictionary(s => s.DetailID ?? string.Empty, s => s.Value ?? string.Empty);
            var apiKey = settingsDict.ContainsKey(Messages.SettingApiKey) ? settingsDict[Messages.SettingApiKey] : Messages.EmptyString;
            
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new PXException(Messages.FrameApiKeyRequiredForTesting);
            }

            try
            {
                // Test connection to Frame API
                var client = new FrameApiClient(apiKey);
                // In a real implementation, you would make a test API call here
                if (apiKey.Length < 10)
                {
                    throw new PXException(Messages.InvalidApiKeyFormat);
                }
                
                // For testing purposes, we'll just verify the API key format is reasonable
                // A real implementation would make an actual API call to verify credentials
            }
            catch (Exception ex) when (!(ex is PXException))
            {
                throw new PXException(Messages.FrameApiConnectionTestFailed, ex.Message);
            }
        }
    }
}
using Sullinger.ValidatableBase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleUniversalApp
{
    public class ValidationLocalizationService : IValidationLocalizationService
    {
        /// <summary>
        /// Gets the localized message from the apps resources and returns it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetLocalizedMessage(string key)
        {
            return Windows.ApplicationModel.Resources.ResourceLoader
                .GetForViewIndependentUse()
                .GetString(key);
        }
    }
}

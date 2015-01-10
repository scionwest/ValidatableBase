using Sullinger.ValidatableBase.Models;

namespace ValidatableBase
{
    public static class ValidationLocalizationFactory
    {
        /// <summary>
        /// The validation localization service singleton.
        /// </summary>
        private static IValidationLocalizationService _serviceSingleton;

        /// <summary>
        /// Creates a singleton instance of T that will be returned when CreateService is invoked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Initialize<T>() where T : class, IValidationLocalizationService, new()
        {
            _serviceSingleton = new T();
        }

        /// <summary>
        /// Creates the localization service used by the validation system 
        /// to provide failure message localization.
        /// </summary>
        /// <returns></returns>
        public static IValidationLocalizationService CreateService()
        {
            return _serviceSingleton;
        }
    }
}

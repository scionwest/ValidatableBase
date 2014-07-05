using System;
using System.Collections.Generic;

namespace Scionwest.Validatable.Models
{
    /// <summary>
    /// Provides a contract to objects wanting to support data validation.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Gets the validation messages.
        /// </summary>
        /// <value>
        /// The validation messages.
        /// </value>
        Dictionary<string, List<IValidationMessage>> ValidationMessages { get; }

        /// <summary>
        /// Adds the supplied validation message to the ValidationMessages collection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property this validation was performed against.</param>
        void AddValidationMessage(IValidationMessage message, string property = "");

        /// <summary>
        /// Removes the validation message from the ValidationMessages collection.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="property">The property this validation was performed against.</param>
        void RemoveValidationMessage(string message, string property = "");

        /// <summary>
        /// Removes all of the validation messages associated to the supplied property from the ValidationMessages collection.
        /// </summary>
        /// <param name="property">The property this validation was performed against.</param>
        void RemoveValidationMessages(string property = "");

        /// <summary>
        /// Determines whether the object has any validation message Type's matching T for the the specified property.
        /// </summary>
        /// <typeparam name="T">A Type implementing IValidationMessage</typeparam>
        /// <param name="property">The property this validation was performed against.</param>
        /// <returns></returns>
        bool HasValidationMessageType<T>(string property = "") where T : IValidationMessage, new();

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="validationDelegate">The validation delegate.</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <param name="property">The property this validation was performed against.</param>
        /// <returns>Returns a validation message if the validation failed. Otherwise, null is returned.</returns>
        IValidationMessage ValidateProperty(Func<string, IValidationMessage> validationDelegate, string failureMessage, string propertyName = "");
    }
}

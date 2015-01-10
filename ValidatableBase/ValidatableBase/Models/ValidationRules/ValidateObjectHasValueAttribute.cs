using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Valides if the property is null or not. 
    /// If the value is a string, it will check if the string is empty or null.
    /// If the value is an ICollection, it will check if it is empty or not.
    /// </summary>
    public class ValidateObjectHasValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="property">The property that will its value validated.</param>
        /// <param name="sender">The sender who owns the property.</param>
        /// <returns>
        /// Returns a validation message if validation failed. Otherwise null is returned to indicate a passing validation.
        /// </returns>
        public override IValidationMessage Validate(System.Reflection.PropertyInfo property, IValidatable sender)
        {
            if (!this.CanValidate(sender))
            {
                return null;
            }

            // Set up localization if available.
            this.PrepareLocalization();

            var validationMessage = Activator.CreateInstance(this.ValidationMessageType, this.FailureMessage) as IValidationMessage;
            var value = property.GetValue(sender, null);

            IValidationMessage result = null;
            if (value is string)
            {
                result = string.IsNullOrWhiteSpace(value.ToString()) ? validationMessage : null;
            }
            else if (value is IEnumerable)
            {
                if (value is ICollection)
                {
                    result = (value as ICollection).Count > 0 ? null : validationMessage;
                }
                else
                {
                    // Only perform the cast if the underlying Type is not an ICollection.
                    result = (value as IEnumerable<object>).Any() ? null : validationMessage;
                }
            }
            else
            {
                result = value == null ? validationMessage : null;
            }

            return this.RunInterceptedValidation(sender, property, result);
        }
    }
}

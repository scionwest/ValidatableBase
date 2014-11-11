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
            
            if (!string.IsNullOrWhiteSpace(this.FailureMessageResourceName))
                this.FailureMessage = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse().GetString(this.FailureMessageResourceName);

            var validationMessage = Activator.CreateInstance(this.ValidationMessageType, this.FailureMessage) as IValidationMessage;
            var value = property.GetValue(sender, null);

            if (value is string)
            {
                return string.IsNullOrWhiteSpace(value.ToString()) ? validationMessage : null;
            }
            else if (value is IEnumerable)
            {
                if (value is ICollection)
                {
                    return (value as ICollection).Count > 0 ? null : validationMessage;
                }
                else
                {
                    // Only perform the cast if the underlying Type is not an ICollection.
                    return (value as IEnumerable<object>).Any() ? null : validationMessage;
                }
            }
            else
            {
                return value == null ? validationMessage : null;
            }
        }
    }
}

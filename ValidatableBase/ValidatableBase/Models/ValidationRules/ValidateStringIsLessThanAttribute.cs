using System;
using System.Globalization;
using System.Reflection;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Validates if the string is longer than the maximum specified.
    /// </summary>
    public class ValidateStringIsLessThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the minimum length for the string. If the string provided is null, it will be converted to an empty string.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int LessThanValue { get; set; }

        /// <summary>
        /// Gets or sets the optional comparison property.
        /// This can be a child property within a property as long as the root property belongs to the sender provided to the Validate method.
        /// If a comparison property is specified, then MaximumLength property is ignored.
        /// Using this comparison is much slower than just specifying a MaximumLength.
        /// </summary>
        /// <value>
        /// The optional comparison property.
        /// </value>
        public string ComparisonProperty { get; set; }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="property">The property that will its value validated.</param>
        /// <param name="sender">The sender who owns the property.</param>
        /// <returns>
        /// Returns a validation message if validation failed. Otherwise null is returned to indicate a passing validation.
        /// </returns>
        public override IValidationMessage Validate(PropertyInfo property, IValidatable sender)
        {
            if (!this.CanValidate(sender))
            {
                return null;
            }

            var validationMessage = Activator.CreateInstance(this.ValidationMessageType, this.FailureMessage) as IValidationMessage;
            var value = property.GetValue(sender, null);

            // Check if we need to compare against another property.
            if (!string.IsNullOrEmpty(this.ComparisonProperty))
            {
                // Fetch the value of the secondary property specified.
                object secondaryPropertyValue = this.GetComparisonValue(sender, this.ComparisonProperty);

                if (secondaryPropertyValue != null && !(secondaryPropertyValue is string))
                {
                    int output = 0;
                    if (int.TryParse(secondaryPropertyValue.ToString(), NumberStyles.Integer, null, out output))
                    {
                        this.LessThanValue = output;
                    }
                } 
                else if (secondaryPropertyValue is string)
                {
                    this.LessThanValue = secondaryPropertyValue.ToString().Length;
                }
            }

            if (value == null)
            {
                value = string.Empty;
            }

            // While we do convert it to a string below, we want to make sure that the actual Type is a string
            // so that we are not doing a string length comparison check on ToString() of a concrete Type that is not a string.
            IValidationMessage validationResult = null;
            if (value is string)
            {
                validationResult = value.ToString().Length > this.LessThanValue ? validationMessage : null;
            }
            return this.RunInterceptedValidation(sender, property, validationResult);
        }
    }
}

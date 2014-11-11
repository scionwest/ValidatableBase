using System;
using System.Globalization;
using System.Reflection;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Validates if the length of a string is greater than the minimum length specified.
    /// </summary>
    public class ValidateStringIsGreaterThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the minimum length for the string. If the string provided is null, it will be converted to an empty string.
        /// </summary>
        /// <value>
        /// The minimum length.
        /// </value>
        public int GreaterThanValue { get; set; }

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
            
            if (!string.IsNullOrWhiteSpace(this.FailureMessageResourceName))
            	this.FailureMessage = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse().GetString(this.FailureMessageResourceName);

            var validationMessage = Activator.CreateInstance(this.ValidationMessageType, this.FailureMessage) as IValidationMessage;
            var value = property.GetValue(sender, null);

            // Check if we need to compare against another property.
            if (!string.IsNullOrEmpty(this.ComparisonProperty))
            {
                // Fetch the value of the secondary property specified.
                object result = this.GetComparisonValue(sender, this.ComparisonProperty);

                if (result != null && !(result is string))
                {
                    int output = 0;
                    if (int.TryParse(result.ToString(), NumberStyles.Integer, null, out output))
                    {
                        this.GreaterThanValue = output;
                    }
                }
				else if (result != null && result is string)
                {
                    this.GreaterThanValue = result.ToString().Length;
                }
            }

            if (value == null)
            {
                value = string.Empty;
            }

            // While we do convert it to a string below, we want to make sure that the actual Type is a string
            // so that we are not doing a string length comparison check on ToString() of a concrete Type that is not a string.
            if (value is string)
            {
                return (this.GreaterThanValue > value.ToString().Length || value.ToString().Length == 0) ? validationMessage : null;
            }

            return validationMessage;
        }
    }
}

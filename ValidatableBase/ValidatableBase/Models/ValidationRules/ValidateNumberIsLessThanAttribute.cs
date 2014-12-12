using System;
using System.Globalization;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Validates that a property has a number less than a maximum value
    /// </summary>
    public class ValidateNumberIsLessThanAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the type of the number data.
        /// </summary>
        /// <value>
        /// The type of the number data.
        /// </value>
        private ValidationNumberDataTypes numberDataType;
            
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public string LessThanValue { get; set; }

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
        public override IValidationMessage Validate(System.Reflection.PropertyInfo property, IValidatable sender)
        {
            if (!this.CanValidate(sender))
            {
                return null;
            }
            
            if (!string.IsNullOrWhiteSpace(this.FailureMessageResourceName))
                this.FailureMessage = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse().GetString(this.FailureMessageResourceName);

            var validationMessage =
                Activator.CreateInstance(this.ValidationMessageType, this.FailureMessage) as IValidationMessage;

            // Get the property value.
            var propertyValue = property.GetValue(sender, null);

            // Ensure the property value is the same data type we are comparing to.
            if (!this.ValidateDataTypesAreEqual(propertyValue))
            {
                var error = string.Format(
                    "The property '{0}' data type is not the same as the data type ({1}) specified for validation checks. They must be the same Type.",
                    property.PropertyType.Name,
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }

            // Check if we need to compare against another property.
            object alternateProperty = null;
            if (!string.IsNullOrEmpty(this.ComparisonProperty))
            {
                // Fetch the value of the secondary property specified.
                alternateProperty = this.GetComparisonValue(sender, this.ComparisonProperty);
            }

            IValidationMessage result = null;
            if (this.numberDataType == ValidationNumberDataTypes.Short)
            {
                result = ValidateShortValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Int)
            {
                result = this.ValidateIntegerValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Long)
            {
                result = this.ValidateLongValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Float)
            {
                result = this.ValidateFloatValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Double)
            {
                result = this.ValidateDoubleValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Decimal)
            {
                result = this.ValidateDecimalValueIsGreaterThan(propertyValue, alternateProperty, validationMessage);
            }

            return this.RunInterceptedValidation(sender, property, result);
        }

        /// <summary>
        /// Validates the data types are equal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns true if the types match.</returns>
        private bool ValidateDataTypesAreEqual(object value)
        {
            if (this.numberDataType == ValidationNumberDataTypes.None)
            {
                if (value is short)
                {
                    this.numberDataType = ValidationNumberDataTypes.Short;
                }
                else if (value is int)
                {
                    this.numberDataType = ValidationNumberDataTypes.Int;
                }
                else if (value is long)
                {
                    this.numberDataType = ValidationNumberDataTypes.Long;
                }
                else if (value is float)
                {
                    this.numberDataType = ValidationNumberDataTypes.Float;
                }
                else if (value is double)
                {
                    this.numberDataType = ValidationNumberDataTypes.Double;
                }
                else if (value is decimal)
                {
                    this.numberDataType = ValidationNumberDataTypes.Decimal;
                }
                else
                {
                    return false;
                }

                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Short && value is short)
            {
                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Int && value is int)
            {
                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Long && value is long)
            {
                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Float && value is float)
            {
                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Double && value is double)
            {
                return true;
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Decimal && value is decimal)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the minimum short value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateShortValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            short convertedValueFromProperty = 0;
            short maximumConvertedValue = 0;
            bool propertyConversionSucceeded = short.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = short.TryParse(this.LessThanValue, NumberStyles.Integer, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            short alternateValue;
            if (alternateProperty != null &&
                short.TryParse(alternateProperty.ToString(), NumberStyles.Integer, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }

        /// <summary>
        /// Validates the minimum integer value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateIntegerValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            int convertedValueFromProperty = 0;
            int maximumConvertedValue = 0;
            bool propertyConversionSucceeded = int.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = int.TryParse(this.LessThanValue, NumberStyles.Integer, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            int alternateValue;
            if (alternateProperty != null &&
                int.TryParse(alternateProperty.ToString(), NumberStyles.Integer, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }

        /// <summary>
        /// Validates the minimum long value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateLongValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            long convertedValueFromProperty = 0;
            long maximumConvertedValue = 0;
            bool propertyConversionSucceeded = long.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = long.TryParse(this.LessThanValue, NumberStyles.Integer, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            long alternateValue;
            if (alternateProperty != null &&
                long.TryParse(alternateProperty.ToString(), NumberStyles.Integer, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }

        /// <summary>
        /// Validates the minimum float value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateFloatValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            float convertedValueFromProperty = 0;
            float maximumConvertedValue = 0;
            bool propertyConversionSucceeded = float.TryParse(propertyValue.ToString(), NumberStyles.Float, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = float.TryParse(this.LessThanValue, NumberStyles.Float, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            float alternateValue;
            if (alternateProperty != null &&
                float.TryParse(alternateProperty.ToString(), NumberStyles.Float, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }

        /// <summary>
        /// Validates the minimum double value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateDoubleValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            double convertedValueFromProperty = 0;
            double maximumConvertedValue = 0;
            bool propertyConversionSucceeded = double.TryParse(propertyValue.ToString(), NumberStyles.Number, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = double.TryParse(this.LessThanValue, NumberStyles.Number, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            double alternateValue;
            if (alternateProperty != null &&
                double.TryParse(alternateProperty.ToString(), NumberStyles.Number, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }

        /// <summary>
        /// Validates the minimum decimal value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateProperty">The alternate property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        private IValidationMessage ValidateDecimalValueIsGreaterThan(object propertyValue, object alternateProperty, IValidationMessage validationMessage)
        {
            decimal convertedValueFromProperty = 0;
            decimal maximumConvertedValue = 0;
            bool propertyConversionSucceeded = decimal.TryParse(propertyValue.ToString(), NumberStyles.Number, null, out convertedValueFromProperty);
            bool valueComparisonConversionSucceeded = decimal.TryParse(this.LessThanValue, NumberStyles.Number, null, out maximumConvertedValue);

            if (!propertyConversionSucceeded && !valueComparisonConversionSucceeded && alternateProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            decimal alternateValue;
            if (alternateProperty != null &&
                decimal.TryParse(alternateProperty.ToString(), NumberStyles.Number, null, out alternateValue))
            {
                return alternateValue > convertedValueFromProperty ? null : validationMessage;
            }
            else
            {
                // Compare the value to the maximum allowed by the attribute. 
                return maximumConvertedValue > convertedValueFromProperty ? null : validationMessage;
            } 
        }
    }
}

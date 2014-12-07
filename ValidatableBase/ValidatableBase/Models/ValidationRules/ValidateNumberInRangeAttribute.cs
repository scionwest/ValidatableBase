using System;
using System.Globalization;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    /// <summary>
    /// Validates that a property's value is within a given range.
    /// </summary>
    public class ValidateNumberInRangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public string MaximumValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public string MinimumValue { get; set; }

        /// <summary>
        /// Gets or sets the type of the number data.
        /// </summary>
        /// <value>
        /// The type of the number data.
        /// </value>
        private ValidationNumberDataTypes numberDataType;

        /// <summary>
        /// Gets or sets the optional comparison property.
        /// This can be a child property within a property as long as the root property belongs to the sender provided to the Validate method.
        /// If a comparison property is specified, then MaximumLength property is ignored.
        /// Using this comparison is much slower than just specifying a MaximumLength.
        /// </summary>
        /// <value>
        /// The optional comparison property.
        /// </value>
        public string MaximumComparisonProperty { get; set; }

        /// <summary>
        /// Gets or sets the minimum comparison property.
        /// </summary>
        /// <value>
        /// The minimum comparison property.
        /// </value>
        public string MinimumComparisonProperty { get; set; }

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
            object alternateMaxProperty = null;
            object alternateMinProperty = null;
            if (!string.IsNullOrEmpty(this.MaximumComparisonProperty))
            {
                // Fetch the value of the secondary property specified.
                alternateMaxProperty = this.GetComparisonValue(sender, this.MaximumComparisonProperty);
            }

            if (!string.IsNullOrEmpty(this.MinimumComparisonProperty))
            {
                alternateMinProperty = this.GetComparisonValue(sender, this.MinimumComparisonProperty);
            }

            IValidationMessage result = null;
            if (this.numberDataType == ValidationNumberDataTypes.Short)
            {
                result = ValidateShortValueInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Int)
            {
                result = this.ValidateIntegerInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Long)
            {
                result = this.ValidateLongInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Float)
            {
                result = this.ValidateFloatInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Double)
            {
                result = this.ValidateDoubleInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
            }
            else if (this.numberDataType == ValidationNumberDataTypes.Decimal)
            {
                result = this.ValidateDecimalInRange(propertyValue, alternateMaxProperty, alternateMinProperty, validationMessage);
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
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateShortValueInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            short convertedValueFromProperty = 0;
            short convertedMaximumValue = 0;
            short convertedMinimumValue = 0;
            bool propertyConversionSucceeded = short.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = short.TryParse(this.MaximumValue, NumberStyles.Integer, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = short.TryParse(this.MinimumValue, NumberStyles.Integer, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            short maxValue = 0;
            if (alternateMaxProperty != null &&
                !short.TryParse(alternateMaxProperty.ToString(), NumberStyles.Integer, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            short minValue = 0;
            if (alternateMinProperty != null &&
                !short.TryParse(alternateMinProperty.ToString(), NumberStyles.Integer, null, out minValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                // If the conversion in the alternate property conditional check succeeded above, then minValue will not be zero
                // if it is zero, the conversion failed and we use our pre-set property.
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty))
                ? null
                : validationMessage;
        }

        /// <summary>
        /// Validates the minimum integer value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateIntegerInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            int convertedValueFromProperty = 0;
            int convertedMaximumValue = 0;
            int convertedMinimumValue = 0;
            bool propertyConversionSucceeded = int.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = int.TryParse(this.MaximumValue, NumberStyles.Integer, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = int.TryParse(this.MinimumValue, NumberStyles.Integer, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            int maxValue = 0;
            if (alternateMaxProperty != null &&
                !int.TryParse(alternateMaxProperty.ToString(), NumberStyles.Integer, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            int minValue = 0;
            if (alternateMinProperty != null &&
                !int.TryParse(alternateMinProperty.ToString(), NumberStyles.Integer, null, out minValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty) )
                ?  null
                : validationMessage;
        }

        /// <summary>
        /// Validates the minimum long value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateLongInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            long convertedValueFromProperty = 0;
            long convertedMaximumValue = 0;
            long convertedMinimumValue = 0;
            bool propertyConversionSucceeded = long.TryParse(propertyValue.ToString(), NumberStyles.Integer, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = long.TryParse(this.MaximumValue, NumberStyles.Integer, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = long.TryParse(this.MinimumValue, NumberStyles.Integer, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            long maxValue = 0;
            if (alternateMaxProperty != null &&
                !long.TryParse(alternateMaxProperty.ToString(), NumberStyles.Integer, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            long minValue = 0;
            if (alternateMinProperty != null)
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. When validating the range of two other member properties, the data types much all match.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty))
                ? null
                : validationMessage;
        }

        /// <summary>
        /// Validates the minimum float value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateFloatInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            float convertedValueFromProperty = 0;
            float convertedMaximumValue = 0;
            float convertedMinimumValue = 0;
            bool propertyConversionSucceeded = float.TryParse(propertyValue.ToString(), NumberStyles.Number, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = float.TryParse(this.MaximumValue, NumberStyles.Number, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = float.TryParse(this.MinimumValue, NumberStyles.Number, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            float maxValue = 0;
            if (alternateMaxProperty != null &&
                !float.TryParse(alternateMaxProperty.ToString(), NumberStyles.Number, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            float minValue = 0;
            if (alternateMinProperty != null &&
                !float.TryParse(alternateMinProperty.ToString(), NumberStyles.Number, null, out minValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty))
                ? null
                : validationMessage;
        }

        /// <summary>
        /// Validates the minimum double value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateDoubleInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            double convertedValueFromProperty = 0;
            double convertedMaximumValue = 0;
            double convertedMinimumValue = 0;
            bool propertyConversionSucceeded = double.TryParse(propertyValue.ToString(), NumberStyles.Number, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = double.TryParse(this.MaximumValue, NumberStyles.Number, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = double.TryParse(this.MinimumValue, NumberStyles.Number, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            double maxValue = 0;
            if (alternateMaxProperty != null &&
                !double.TryParse(alternateMaxProperty.ToString(), NumberStyles.Number, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            double minValue = 0;
            if (alternateMinProperty != null &&
                !double.TryParse(alternateMinProperty.ToString(), NumberStyles.Number, null, out minValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty))
                ? null
                : validationMessage;
        }

        /// <summary>
        /// Validates the minimum decimal value.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="alternateMaxProperty">The alternate property.</param>
        /// <param name="alternateMinProperty">The alternate minimum property.</param>
        /// <param name="validationMessage">The validation message.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">Validation failed due to invalid data being provided to the validator for conversion.</exception>
        private IValidationMessage ValidateDecimalInRange(object propertyValue, object alternateMaxProperty, object alternateMinProperty, IValidationMessage validationMessage)
        {
            decimal convertedValueFromProperty = 0;
            decimal convertedMaximumValue = 0;
            decimal convertedMinimumValue = 0;
            bool propertyConversionSucceeded = decimal.TryParse(propertyValue.ToString(), NumberStyles.Number, null, out convertedValueFromProperty);
            bool maxValueConversionSucceeded = decimal.TryParse(this.MaximumValue, NumberStyles.Number, null, out convertedMaximumValue);
            bool minValueConversionSucceeded = decimal.TryParse(this.MinimumValue, NumberStyles.Number, null, out convertedMinimumValue);

            if (!propertyConversionSucceeded && !maxValueConversionSucceeded && !minValueConversionSucceeded && alternateMaxProperty == null)
            {
                throw new InvalidCastException("Validation failed due to invalid data being provided to the validator for conversion.");
            }

            // Compare against our secondary property and the senders property value.
            decimal maxValue = 0;
            if (alternateMaxProperty != null &&
                !decimal.TryParse(alternateMaxProperty.ToString(), NumberStyles.Number, null, out maxValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (maxValue == 0)
            {
                maxValue = convertedMaximumValue;
            }

            decimal minValue = 0;
            if (alternateMinProperty != null &&
                !decimal.TryParse(alternateMinProperty.ToString(), NumberStyles.Number, null, out minValue))
            {
                var error = string.Format(
                    "The property data type is not the same as the data type ({0}) specified for validation checks. They must be the same Type.",
                    this.numberDataType.ToString());
                throw new ArgumentNullException(error);
            }
            else if (minValue == 0)
            {
                minValue = convertedMinimumValue;
            }

            // Compare the value to the maximum allowed by the attribute.
            return ((minValue <= convertedValueFromProperty) && (maxValue >= convertedValueFromProperty))
                ? null
                : validationMessage;
        }
    }
}

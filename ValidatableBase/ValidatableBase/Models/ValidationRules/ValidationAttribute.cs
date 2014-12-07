using Sullinger.ValidatableBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sullinger.ValidatableBase.Models.ValidationRules
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ValidationAttribute : Attribute, IValidationRule
    {
        /// <summary>
        /// Gets or sets the type that will be used to create a IMessage instance.
        /// </summary>
        /// <value>
        /// The type of the validation message.
        /// </value>
        public Type ValidationMessageType { get; set; }

        /// <summary>
        /// Gets or sets the failure message.
        /// </summary>
        /// <value>
        /// The failure message.
        /// </value>
        public string FailureMessage { get; set; }

        /// <summary>
        /// Gets or sets whether this validation will run. If the target property specified is true, then validation runs.
        /// </summary>
        /// <value>
        /// The enable validation from property boolean.
        /// </value>
        public string ValidateIfMemberValueIsValid { get; set; }

        /// <summary>
        /// Gets or sets the method delegate that can intercept validation.
        /// </summary>
        /// <value>
        /// The interception delegate.
        /// </value>
        public string InterceptionDelegate { get; set; }

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="property">The property that will its value validated.</param>
        /// <param name="sender">The sender who owns the property.</param>
        /// <returns>Returns a validation message if validation failed. Otherwise null is returned to indicate a passing validation.</returns>
        public abstract IValidationMessage Validate(PropertyInfo property, IValidatable sender);

        /// <summary>
        /// Determines if the value specified in the ValidateIfMemberValueIsValue is a valid value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Can not base validation off of a non-boolean property.</exception>
        public bool CanValidate(IValidatable sender)
        {
            if (string.IsNullOrEmpty(this.ValidateIfMemberValueIsValid))
            {
                return true;
            }

            string valueToParse = string.Empty;
            bool evaluateInverseValue = false;
            if (this.ValidateIfMemberValueIsValid.StartsWith("!"))
            {
                evaluateInverseValue = true;
                valueToParse = this.ValidateIfMemberValueIsValid.Substring(1);
            }
            else
            {
                valueToParse = this.ValidateIfMemberValueIsValid;
            }

            bool result = false;
            object valueToCompare = this.GetComparisonValue(sender, valueToParse);
            if (valueToCompare is bool)
            {
                bool.TryParse(valueToCompare.ToString(), out result);
            }
            else if (valueToCompare is string)
            {
                // We can not validate if the string is empty.
                result = !string.IsNullOrWhiteSpace(valueToCompare.ToString());
            }
            else if (valueToCompare is int || valueToCompare is short || valueToCompare is long || valueToCompare is double || valueToCompare is float || valueToCompare is decimal)
            {
                var numberGreaterThanRule = new ValidateNumberIsGreaterThanAttribute();
                numberGreaterThanRule.GreaterThanValue = "0";
                numberGreaterThanRule.ValidationMessageType = this.ValidationMessageType;
                numberGreaterThanRule.FailureMessage = this.FailureMessage;
                PropertyInfo property = this.GetAlternatePropertyInfo(sender, ValidateIfMemberValueIsValid);
                IValidationMessage validationMessage = numberGreaterThanRule.Validate(property, sender);

                // if we are greater than 0, then we hav a valid value and can validate.
                result = validationMessage == null;
            }
            else if (valueToCompare == null)
            {
                // We can not validate if the object is null.
                result = false;
            }
            else
            {
                result = true;
            }

            return evaluateInverseValue ? !result : result;
        }

        /// <summary>
        /// Runs the delegate specified to intercept validation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected IValidationMessage RunInterceptedValidation(IValidatable sender, PropertyInfo property, IValidationMessage message)
        {
            if (string.IsNullOrWhiteSpace(this.InterceptionDelegate))
            {
                return message;
            }

            var delegateValidationRule = new ValidateWithCustomHandlerAttribute
            {
                DelegateName = this.InterceptionDelegate,
                FailureMessage = message == null ? string.Empty : message.Message,
                ValidationMessageType = message == null ? null : message.GetType(),
            };

            return delegateValidationRule.Validate(property, sender);
        }

        /// <summary>
        /// Walks the senders Type tree to find the property (or sub-property) specified.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="alternateProperty">The secondary property.</param>
        /// <returns>
        /// Returns the value associated with the specified property.
        /// </returns>
        protected object GetComparisonValue(object sender, string alternateProperty)
        {
            if (!string.IsNullOrEmpty(alternateProperty))
            {
                string[] pathToProperty = alternateProperty.Split('.');
                PropertyInfo comparisonProperty = null;

                try
                {
                    comparisonProperty = sender.GetType().GetProperty(pathToProperty[0]);
                }
                catch (Exception)
                {
                    throw;
                }

                if (pathToProperty.Length == 1)
                {
                    return comparisonProperty.GetValue(sender, null);
                }
                else if (pathToProperty.Length > 1)
                {
                    // Walk down the tree to find the final value we are evaluating against.
                    object childSender = null;
                    for (int index = 1; index < pathToProperty.Length; index++)
                    {
                        try
                        {
                            childSender = comparisonProperty.GetValue(sender, null);
                            comparisonProperty = childSender.GetType().GetProperty(pathToProperty[index]);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    // Grab the length of this string.
                    return comparisonProperty.GetValue(childSender, null);
                }
            }

            return null;
        }

        protected PropertyInfo GetAlternatePropertyInfo(object sender, string alternateProperty)
        {
            if (!string.IsNullOrEmpty(alternateProperty))
            {
                string[] pathToProperty = alternateProperty.Split('.');
                PropertyInfo comparisonProperty = null;

                try
                {
                    comparisonProperty = sender.GetType().GetProperty(pathToProperty[0]);
                }
                catch (Exception)
                {
                    throw;
                }

                if (pathToProperty.Length == 1)
                {
                    return comparisonProperty;
                }
                else if (pathToProperty.Length > 1)
                {
                    // Walk down the tree to find the final value we are evaluating against.
                    object childSender = null;
                    for (int index = 1; index < pathToProperty.Length; index++)
                    {
                        try
                        {
                            childSender = comparisonProperty.GetValue(sender, null);
                            comparisonProperty = childSender.GetType().GetProperty(pathToProperty[index]);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                    // Grab the length of this string.
                    return comparisonProperty;
                }
            }

            return null;
        }
    }
}

using Sullinger.ValidatableBase.Models;
using Sullinger.ValidatableBase.Models.ValidationRules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SampleUniversalApp.Models
{
    public class Bank
    {
        public const decimal MinimumBalance = 2000M;

        [ValidateObjectHasValue(
            LocalizationKey = "Bank-IsOpen-Validation-Failure",
            ValidationMessageType = typeof(ValidationErrorMessage),
            InterceptionDelegate = "Validate its not sunday")]
        public bool IsOpen { get; set; }

        [ValidationCustomHandlerDelegate(DelegateName = "Validate its not sunday")]
        public IValidationMessage IsOpenValidationIntercept(IValidationMessage failureMessage, PropertyInfo property)
        {
            // Is Open is allowed to be false on Sundays.
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                return null;
            }

            // It's not sunday, so return the failure message generated
            // by the attribute.
            return failureMessage;
        }
    }
}

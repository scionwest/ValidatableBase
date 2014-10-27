using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Sullinger.ValidatableBase.Models;
using System.ComponentModel;
using Sullinger.ValidatableBase.Models.ValidationRules;
using System.Reflection;

namespace SampleUniversalApp.Models
{
    public class User : ValidatableBase, INotifyPropertyChanged
    {
        /// <summary>
        /// The Email backing field.
        /// </summary>
        private string email = string.Empty;

        /// <summary>
        /// The Password backing field.
        /// </summary>
        private string password = string.Empty;

        /// <summary>
        /// Gets the Email.
        /// </summary>
        /// <value>
        /// The Email.
        /// </value>
        [ValidateValueIsNotNull(FailureMessage = "E-Mail can not be left blank.", ValidationMessageType = typeof(ValidationErrorMessage))]
        [ValidateWithCustomHandler(DelegateName = "ValidateEmailFormat", ValidationMessageType = typeof(ValidationErrorMessage), FailureMessage = "Email address is not properly formatted.")]
        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                this.email = value;
                this.OnPropertyChanged("Email");
            }
        }
        /// <summary>
        /// Gets the Password.
        /// </summary>
        /// <value>
        /// The Password.
        /// </value>        
        [ValidateStringIsGreaterThan(GreaterThanValue = 6, ValidateIfMemberValueIsValid = "Email",  FailureMessage = "Password must be greater than 6 characters.", ValidationMessageType = typeof(ValidationErrorMessage))]
        [ValidateStringIsLessThan(LessThanValue = 20, ValidateIfMemberValueIsValid = "Email", FailureMessage = "Password must be less than 20 characters.", ValidationMessageType = typeof(ValidationErrorMessage))]
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.OnPropertyChanged("Password");
            }
        }

        /// <summary>
        /// Performs validation on the User.
        /// </summary>
        public void Validate()
        {
            // Validate all of our attribute based properties.
            this.ValidateAll();
        }

        [ValidationCustomHandlerDelegate(DelegateName = "ValidateEmailFormat")]
        private IValidationMessage ValidateEmailIsFormatted(IValidationMessage failureMessage, PropertyInfo property)
        {
            string[] addressParts = this.Email.Split('@');

            if (addressParts.Length < 2)
            {
                return failureMessage;
            }

            string[] domainPiece = addressParts.LastOrDefault().Split('.');
            if (domainPiece.Length < 2)
            {
                return failureMessage;
            }

            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

using Scionwest.Validatable.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SampleUniversalApp.Models
{
    public class User : ValidatableBase
    {
        /// <summary>
        /// The Email backing field.
        /// </summary>
        private string email = string.Empty;

        /// <summary>
        /// The Password backing field.
        /// </summary>
        private string password = string.Empty;

        public User()
        {
            this.RegisterProperty("Email", "Password");
        }

        /// <summary>
        /// Gets the Email.
        /// </summary>
        /// <value>
        /// The Email.
        /// </value>
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
        public override void Validate()
        {
            this.ValidateProperty(this.ValidateEmailIsNotEmpty, "Invalid Email Address.", "Email");
            this.ValidateProperty(this.ValidateEmailIsFormatted, "Email Address is not in the correct format.", "Email");
            this.ValidateProperty(this.ValidatePasswordIsNotEmpty, "Password can not be empty.", "Password");
            this.ValidateProperty(this.ValidatePasswordIsToShort, "Password must be greater than 8 characters.", "Password");
            this.ValidateProperty(this.ValidateIfPasswordContainsSpaces, "Password must not contain spaces.", "Password");

            base.Validate();
        }
        
        /// <summary>
        /// Validates that the email is not empty.
        /// </summary>
        /// <param name="failureMessage">The message to supply the error collection if validation fails.</param>
        /// <returns>Returns a ValidationErrorMessage if validation fails. Otherwise, null is returned.</returns>
        private IValidationMessage ValidateEmailIsNotEmpty(string failureMessage)
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                return new ValidationErrorMessage(failureMessage);
            }

            return null;
        }

        private IValidationMessage ValidateEmailIsFormatted(string failureMessage)
        {
            string[] addressParts = this.Email.Split('@');

            if (addressParts.Length < 2)
            {
                var msg = new ValidationErrorMessage(failureMessage);
                return msg;
            }

            string[] domainPiece = addressParts.LastOrDefault().Split('.');
            if (domainPiece.Length < 2)
            {
                var msg = new ValidationErrorMessage(failureMessage);
                return msg;
            }

            return null;
        }

        /// <summary>
        /// Validates that the password is not empty.
        /// </summary>
        /// <param name="failureMessage">The message to supply the error collection if validation fails.</param>
        /// <returns>Returns a ValidationErrorMessage if validation fails. Otherwise, null is returned.</returns>
        private IValidationMessage ValidatePasswordIsNotEmpty(string failureMessage)
        {
            if (string.IsNullOrEmpty(this.Password))
            {
                return new ValidationErrorMessage(failureMessage);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failureMessage">The message to supply the error collection if validation fails.</param>
        /// <returns>Returns a ValidationErrorMessage if validation fails. Otherwise, null is returned.</returns>
        private IValidationMessage ValidatePasswordIsToShort(string failureMessage)
        {
            if (this.Password.Length < 8)
            {
                return new ValidationErrorMessage(failureMessage);
            }

            return null;
        }

        /// <summary>
        /// Tests to see if the password contains any spaces.
        /// </summary>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        private IValidationMessage ValidateIfPasswordContainsSpaces(string failureMessage)
        {
            if (this.Password.Contains(' '))
            {
                return new ValidationErrorMessage(failureMessage);
            }

            return null;
        }
    }
}

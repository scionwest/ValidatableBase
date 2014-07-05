ValidatableBase
===============

Model Validation for Universal WinRT Apps. Since Universal WinRT apps targeting Windows 8.1 and Windows Phone 8.1 lack built in, easy to use data Validation, I wrote a quick model object that can be used to add validation to your apps.

An example model, providing validation making sure the name is not blank.

    public class ModelFixture : ValidatableBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override void Validate()
        {
            this.ValidateProperty((failureMessage) =>
                {
                    if (string.IsNullOrEmpty(this.Name))
                    {
                        return new ValidationErrorMessage(failureMessage);
                    }
                    return null;
                },
                failureMessage: "Name can not be blank!",
                propertyName: "Name");

            base.Validate();
        }
    }

## Advanced Example validating multiple business rules on a property, for multiple properties.

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

## View Model validation checks

This exposes validation methods to external objects as well. By inheriting from ValidatableBase, you can force validation checks on a model from within your view model.

        public bool CanExecute(object parameter)
        {
            // Perform validation on the user.
            this.AppUser.Validate();

            // Check if there are any errors.
            if (this.AppUser.HasValidationMessageType<ValidationErrorMessage>())
            {
                return false;
            }

            return true;
        }

## Binding to the View

Binding to the view is really easy, using one of the two provided converters.

    <Application.Resources>
        <converters:ValidationCollectionToSingleStringConverter x:Key="ValidationCollectionToSingleStringConverter" />
        <converters:IValidationMessageCollectionToStringCollectionConverter x:Key="CollectionConverter" />
    </Application.Resources>

### Bind to a single error for a property.

    <TextBlock x:Name="EmailValidationErrorTextBlock"
                Text="{Binding Path=AppUser.ValidationMessages[Email], 
								Converter={StaticResource ValidationCollectionToSingleStringConverter}}"
                Foreground="Red" />

### Bind to the entire collection of errors for a property

    <ItemsControl ItemsSource="{Binding Path=AppUser.ValidationMessages[Password], 
										Converter={StaticResource CollectionConverter}}">
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <TextBlock Text="{Binding}"
                            Foreground="Red" />
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>

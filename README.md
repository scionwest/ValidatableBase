ValidatableBase
===============

Model Validation for Universal WinRT Apps through delegate method invocation per-property or DataAnnotation styled attribute validation. Universal WinRT apps targeting Windows 8.1 and Windows Phone 8.1 lack built in, easy to use data Validation, so ValidatableBase was created.

An example model, providing validation making sure the email property is not blank.

    public class User : ValidatableBase, INotifyPropertyChanged
    {
        [ValidateObjectHasValue(FailureMessage = "E-Mail can not be left blank.", ValidationMessageType = typeof(ValidationErrorMessage))]
        public string Email { get; set; }

## Advanced Example validating multiple business rules on a property, for multiple properties.

In this example, the User model has been extended a bit. Now we validate that the E-mail address is not blank and that it is in a valid format. Once E-mail validation is successful, the model will validate its Password property.

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
        [ValidateObjectHasValue(FailureMessage = "E-Mail can not be left blank.", ValidationMessageType = typeof(ValidationErrorMessage))]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

## View Model validation checks

This exposes validation methods to external objects as well. By letting your model inherit from ValidatableBase, you can force validation checks on a model from within your view model.

        public void Execute(object parameter)
        {
            // Perform validation on the user model's built in validation.
            this.AppUser.ValidateAll();

            // Check if there are any errors.
            if (this.AppUser.HasValidationMessages<ValidationErrorMessage>())
            {
                return;
            }

            // Do stuff.
        }
        
## View Model imposed rules

You can perform cross-property validation within your view model as well. Our model does not contain a PasswordConfirmation property, since it does not belong in it. Instead for our example, the PasswordConfirmation property will exist in the View Model. We can execute the User model's validation, then execute additional validation on-top of it that the View Model owns.

In the following example, we perform validation injection on the model. The view model creates an anonymous method, and assigns the result of the validation to the Model's "Password" property. So any item data-bound to the Password validation collection on the model, will see the failed validation assigned by the view model.

        public void Execute(object parameter)
        {
            // Perform validation on the user's built in validation.
            this.AppUser.ValidateAll();

            // Piggy-back on top of the user default validation with an additional level of validation in our view model.
            // We ensure the Model's password and View Model's password confirmation matches.
            this.AppUser.ValidateProperty(
                () => PasswordConfirmation.Equals(this.AppUser.Password),
                new ValidationErrorMessage("Passwords do not match!"),
                    "Password");

            // Check if there are any errors.
            // We assign the reference to the view models, which contains an observable collection for the UI.
            if (this.AppUser.HasValidationMessages<ValidationErrorMessage>())
            {
                this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
                return;
            }

            // Do stuff.
            return;
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

## Attribute validation types

The library comes with support for validating minimum and maximum numeric values, ranges of numeric values, string length, null objects, empty collections, empty or null strings and custom delegate method invocation.

If you want to write your own attribute validation, you just need to implement `IValidationRule` and then decorate your object.

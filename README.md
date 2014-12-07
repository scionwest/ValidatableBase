ValidatableBase
===============
[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/scionwest/ValidatableBase?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Model Validation for Universal WinRT Apps through delegate method invocation per-property or DataAnnotation styled attribute validation. Universal WinRT apps targeting Windows 8.1 and Windows Phone 8.1 lack built in, easy to use data Validation, so ValidatableBase was created.

### Documentation added

Check out the [new documentation added to the wiki](https://github.com/scionwest/ValidatableBase/wiki). Basic and advance use-cases using ValidatableBase

ValidatableBase Version 2.1 Released
------------------------------------

Added validation interception support. You may now have the built-in validation rules invoke a delegate method. This provides you with a little more flexibility when performing validation. You may piggy back on top of existing rules for minor checks instead of resorting to a full delegate validation method.

An example model, providing validation making sure the email property is not blank.

    public class Bank
    {
        public const decimal MinimumBalance = 2000M;
        
        [ValidateObjectHasValue(
            FailureMessage = "The bank must be open.",
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
                return;
            }

            // Do stuff.
            return;
        }

## Binding to the View

The new version of ValidatableBase no longer stores the validation messages within an ObservableCollection. This is partly due to Portable Class Libraries (which allows ValidatableBase to be used on the widest range of platforms) does not support them. To work around this, I have provided an extension method that takes the model's validation message dictionary (`Dictionary<string, IEnumerable<IValidationMessage>>`) and converts it to a dictionary of observable messages (`Dictionary<string, ObservableCollection<IValidationMessage>>`).

Anytime validation on the model is changed (such as a `ValidateAll()` invocation) you can re-assign the view models validation collection using the extension method, notifying the UI of validation changes as needed.

        public MainPageViewModel()
        {
            this.AppUser = new User();
            this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
        }
                
        /// <summary>
        /// A wrapper around the non-observable collection of validation messages stored in our model.
        /// </summary>
        public Dictionary<string, ObservableCollection<IValidationMessage>> ValidationMessages
        {
            get
            {
                return this.validationMessages;
            }

            private set
            {
                this.validationMessages = value;
                this.OnPropertyChanged("ValidationMessages");
            }
        }
        
        public void Execute(object parameter)
        {
            // Perform validation on the user's built in validation.
            this.AppUser.ValidateAll();

            // Check if there are any errors.
            // We assign the reference to the view models, which contains an observable collection for the UI.
            if (this.AppUser.HasValidationMessages<ValidationErrorMessage>())
            {
                // Update our UI with new validation errors.
                this.ValidationMessages = this.AppUser.GetValidationMessages().ConvertValidationMessagesToObservable();
                return;
            }
            else
            {
                // Clear out each observable collection.
                this.ValidationMessages.AsParallel().ForAll(item => item.Value.Clear());
            }

            // Do stuff.
        }
        
The view is then data-bound to this property for fetching validation information.

Binding to the view is really easy, using one of the two provided converters.

    <Application.Resources>
        <converters:ValidationCollectionToSingleStringConverter x:Key="ValidationCollectionToSingleStringConverter" />
        <converters:IValidationMessageCollectionToStringCollectionConverter x:Key="CollectionConverter" />
    </Application.Resources>

### Bind to a single error for a property.

    <TextBlock x:Name="EmailValidationErrorTextBlock"
                Text="{Binding Path=ValidationMessages[Email], 
								Converter={StaticResource ValidationCollectionToSingleStringConverter}}"
                Foreground="Red" />

### Bind to the entire collection of errors for a property

    <ItemsControl ItemsSource="{Binding Path=ValidationMessages[Password], 
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

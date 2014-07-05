using Scionwest.Validatable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidatableBase.Tests.Fixtures
{
    /// <summary>
    /// A model that inherits from ValidatableBase for testing.
    /// </summary>
    public class ModelFixture : Scionwest.Validatable.Models.ValidatableBase
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
                "Name can not be blank!",
                "Name");

            base.Validate();
        }
    }
}

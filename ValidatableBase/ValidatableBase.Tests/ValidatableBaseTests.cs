using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ValidatableBase.Tests.Fixtures;
using Scionwest.Validatable.Models;

namespace ValidatableBase.Tests
{
    [TestClass]
    public class ValidatableBaseTests
    {
        [TestMethod]
        public void ExecuteAddValidationMessage_CollectionUpdated()
        {
            // Arrange
            var model = new ModelFixture();

            // Act
            model.AddValidationMessage(new ValidationErrorMessage("Test Error"), "Name");

            // Assert
            Assert.IsNotNull(model.ValidationMessages["Name"]);
            Assert.IsTrue(model.HasValidationMessageType<ValidationErrorMessage>("Name"));
            Assert.AreEqual(1, model.ValidationMessages["Name"].Count);
        }
    }
}

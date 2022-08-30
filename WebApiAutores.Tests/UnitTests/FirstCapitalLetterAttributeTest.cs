using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validations;

namespace WebApiAutores.Tests.UnitTests
{
    [TestClass]
    public class FirstCapitalLetterAttributeTest
    {
        [TestMethod]
        public void FirstLetterInLowerCaseReturnError()
        {
            // Preparation
            var firstLetterAttribute = new FirstCapitalLetterAttribute();
            var value = "franz kafka";
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstLetterAttribute.GetValidationResult(value, valContext);

            // Verification
            Assert.AreEqual("First letter must be uppercase", result.ErrorMessage);
        }

        [TestMethod]
        public void NullValueDoesNotReturnError()
        {
            // Preparation
            var firstLetterAttribute = new FirstCapitalLetterAttribute();
            string value = null;
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstLetterAttribute.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FirstLetterInUpperCaseDoesNotReturnError()
        {
            // Preparation
            var firstLetterAttribute = new FirstCapitalLetterAttribute();
            string value = "Franz Kafka";
            var valContext = new ValidationContext(new { Name = value });

            // Execution
            var result = firstLetterAttribute.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }
    }
}
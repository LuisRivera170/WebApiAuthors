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
    }
}
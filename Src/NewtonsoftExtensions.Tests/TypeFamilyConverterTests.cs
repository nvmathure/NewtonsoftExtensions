using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudNDevOps.Newtonsoft.Extensions.Tests
{
    [TestClass]
    public class TypeFamilyConverterTests
    {
        [TestMethod]
        public void TestConstructorWithNullFunction()
        {
            // Arrange
            var dictionary = new Dictionary<string, Type>
            {
                {"Individual", typeof(IndividualEntity<string>)},
                {"Organization", typeof(OrganizationEntity<string>)}
            };
            Action act = () =>
            {
                var unused = new TypeFamilyConverter<Entity<string>, string>(null!, dictionary);
            };

            // Act & Asset
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("typeValueFunc");
        }

        [TestMethod]
        public void TestConstructorWithNullDictionary()
        {
            // Arrange
            Action act = () =>
            {
                var unused = new TypeFamilyConverter<Entity<string>, string>(e => "Dummy", null!);
            };

            // Act & Asset
            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("typeLookupDictionary");
        }

        [TestMethod]
        public void TestReadJsonMethod()
        {
            // Arrange 
            const string inputString = "[ { \"Code\" : \"C1\", \"Classifier\" : \"Individual\", \"FirstName\" : \"John\", \"LastName\" : \"Doe\" }, " +
                                       "{ \"Code\" : \"C2\", \"Classifier\" : \"Organization\", \"Name\" : \"Some Organization\" } ]";
            var typeFamilyConverter = new TypeFamilyConverter<Entity<string>, string>(
                e => e.Classifier,
                new Dictionary<string, Type>
                {
                    { "Individual", typeof(IndividualEntity<string>) },
                    { "Organization", typeof(OrganizationEntity<string>) }
                });

            // Act
            var result = JsonConvert.DeserializeObject<List<Entity<string>>>(
                inputString,
                new JsonSerializerSettings
                {
                    Converters = { typeFamilyConverter }
                });

            // Assert
            result.Should().HaveCount(2);

            var fi = result.First(c => c.Code == "C1");
            fi.Should().BeAssignableTo<IndividualEntity<string>>();
            fi.Classifier.Should().Be("Individual");
            var fiIndividual = (IndividualEntity<string>)fi;
            fiIndividual.FirstName.Should().Be("John");
            fiIndividual.LastName.Should().Be("Doe");

            var si = result.First(c => c.Code == "C2");
            si.Should().BeAssignableTo<OrganizationEntity<string>>();
            si.Classifier.Should().Be("Organization");
            var siOrganization = (OrganizationEntity<string>)si;
            siOrganization.Name.Should().Be("Some Organization");

        }

        [TestMethod]
        public void TestReadJsonMethodWithInvalidClassifier()
        {
            // Arrange 
            const string inputString = "[ { \"Code\" : \"C1\", \"Classifier\" : \"Individual\", \"FirstName\" : \"John\", \"LastName\" : \"Doe\" }, " +
                                       "{ \"Code\" : \"C2\", \"Classifier\" : \"Organization\", \"Name\" : \"Some Organization\" } ]";
            var typeFamilyConverter = new TypeFamilyConverter<Entity<string>, string>(
                e => "Dummy",
                new Dictionary<string, Type>
                {
                    {"Individual", typeof(IndividualEntity<string>)},
                    {"Organization", typeof(OrganizationEntity<string>)}
                });
            Action act = () => JsonConvert.DeserializeObject<List<Entity<string>>>(
                inputString,
                new JsonSerializerSettings
                {
                    Converters = { typeFamilyConverter }
                });

            // Act & Assert
            act.Should().Throw<InvalidOperationException>()
                .And.Message.Should().Contain("Dummy");
        }
    }
}

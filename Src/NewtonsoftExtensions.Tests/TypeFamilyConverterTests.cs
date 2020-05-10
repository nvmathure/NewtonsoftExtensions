using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudNDevOps.Newtonsoft.Extensions.Tests
{
    [TestClass]
    public TypeFamilyConverterTests
    {
        [TestMethod]
    public void TestMethod1()
    {
        // Arrange 
        string inputString = "[ { \"Code\" : \"C1\", \"Classifier\" : \"Individual\", \"FirstName\" : \"John\", \"LastName\" : \"Doe\" }, " +
            "{ \"Code\" : \"C2\", \"Classifier\" : \"Organization\", \"Name\" : \"Some Organization\" } ]";
        var TypeFamilyConverter = new TypeFamilyConverter<Entity<string>, string>(
            new Func<Entity<string>, string>(e => e.Classifier),
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
                Converters = { tfc }
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
}
}

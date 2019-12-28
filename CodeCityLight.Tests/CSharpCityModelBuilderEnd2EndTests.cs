using CodeCityLight.Parser.CSharp;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCityLight.tests
{
    [TestClass]
    public class CSharpCityModelBuilderEnd2EndTests
    {
        /*
        Namespace: Petshop.Messaging.Email
        Namespace: Petshop.Api.Rest
        Namespace: Petshop.Ordering
        Namespace: Petshop.Shipping
        */
        private const string PetshopProjectFilePath = @"..\..\..\..\Petshop\Petshop.csproj";

        [AssemblyInitialize]
        public static void TestSetup(TestContext testContext)
        {
            MSBuildLocator.RegisterDefaults();
        }

        [TestMethod]
        public void PetshopDistricts()
        {
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            Assert.AreEqual(7, codeCity.Districts.Count);

            District rootByName = codeCity.GetDistrictByName("Petshop");
            Assert.IsNotNull(rootByName);
            Assert.AreEqual(4, rootByName.Districts.Count);

            District rootByParentNull = codeCity.GetRootDistricts()[0];
            Assert.AreEqual("Petshop", rootByParentNull.Name);
            Assert.AreEqual(4, rootByParentNull.Districts.Count);
            Assert.AreEqual(rootByName, rootByParentNull);

            District messaging = codeCity.GetDistrictByName("Petshop.Messaging");
            Assert.IsNotNull(messaging);
            Assert.AreEqual(1, messaging.Districts.Count);

            District email = codeCity.GetDistrictByName("Petshop.Messaging.Email");
            District rootFromEmail = email.Parent.Parent;
            Assert.IsNotNull(rootFromEmail);
            Assert.AreEqual(rootByName, rootFromEmail);
        }

        [TestMethod]
        public void PetshopEmailBuildings()
        {

            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            District email = codeCity.GetDistrictByName("Petshop.Messaging.Email");
            Assert.AreEqual(1, email.Buildings.Count);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfFields()
        {
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            District ordering = codeCity.GetDistrictByName("Petshop.Ordering");
            Building orderManagement = ordering.GetBuildingByName("OrderManagement");

            Assert.AreEqual(1, orderManagement.NumberOfFields);
        }

        [TestMethod]
        public void PetshopOrderNumberOfProperties()
        {
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            District ordering = codeCity.GetDistrictByName("Petshop.Ordering");
            Building order = ordering.GetBuildingByName("Order");

            Assert.AreEqual(1, order.NumberOfProperties);
        }

        [TestMethod]
        public void PetshopOrderingOutgoingDependencies()
        {
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            District ordering = codeCity.GetDistrictByName("Petshop.Ordering");
            Assert.AreEqual(1, ordering.OutgoingDependencies);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfMethods()
        {
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            CodeCity codeCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(codeCity).BuildFrom(project);

            District ordering = codeCity.GetDistrictByName("Petshop.Ordering");
            Building orderManagement = ordering.GetBuildingByName("OrderManagement");

            Assert.AreEqual(1, orderManagement.NumberOfMethods);
        }

    }
}

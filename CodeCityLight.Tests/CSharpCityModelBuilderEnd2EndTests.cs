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
        private static CodeCity petshopCity;

        [AssemblyInitialize]
        public static void TestSetup(TestContext testContext)
        {
            MSBuildLocator.RegisterDefaults();
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            petshopCity = new CodeCity("Petshop");
            new CSharpCityModelBuilder(petshopCity).BuildFrom(project);
        }

        [TestMethod]
        public void PetshopDistricts()
        {
            Assert.AreEqual(7, petshopCity.Districts.Count);

            District rootByName = petshopCity.GetDistrictByName("Petshop");
            Assert.IsNotNull(rootByName);
            Assert.AreEqual(4, rootByName.Districts.Count);

            District rootByParentNull = petshopCity.GetRootDistricts()[0];
            Assert.AreEqual("Petshop", rootByParentNull.Name);
            Assert.AreEqual(4, rootByParentNull.Districts.Count);
            Assert.AreEqual(rootByName, rootByParentNull);

            District messaging = petshopCity.GetDistrictByName("Petshop.Messaging");
            Assert.IsNotNull(messaging);
            Assert.AreEqual(1, messaging.Districts.Count);

            District email = petshopCity.GetDistrictByName("Petshop.Messaging.Email");
            District rootFromEmail = email.Parent.Parent;
            Assert.IsNotNull(rootFromEmail);
            Assert.AreEqual(rootByName, rootFromEmail);
        }

        [TestMethod]
        public void PetshopEmailBuildings()
        {
            District email = petshopCity.GetDistrictByName("Petshop.Messaging.Email");
            Assert.AreEqual(1, email.Buildings.Count);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfFields()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Building orderManagement = ordering.GetBuildingByName("OrderManagement");

            Assert.AreEqual(1, orderManagement.NumberOfFields);
        }

        [TestMethod]
        public void PetshopOrderNumberOfProperties()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Building order = ordering.GetBuildingByName("Order");

            Assert.AreEqual(1, order.NumberOfProperties);
        }

        [TestMethod]
        public void PetshopOrderingOutgoingDependencies()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Assert.AreEqual(1, ordering.OutgoingDependencies);
        }

        [TestMethod]
        public void PetshopOrderingIncomingDependencies()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Assert.AreEqual(1, ordering.IncomingDependencies);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfMethods()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Building orderManagement = ordering.GetBuildingByName("OrderManagement");

            Assert.AreEqual(2, orderManagement.NumberOfMethods);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfStatements()
        {
            District ordering = petshopCity.GetDistrictByName("Petshop.Ordering");
            Building orderManagement = ordering.GetBuildingByName("OrderManagement");

            Assert.AreEqual(7, orderManagement.NumberOfStatements);
        }

    }
}

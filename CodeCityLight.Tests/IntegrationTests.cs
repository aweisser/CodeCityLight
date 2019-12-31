using CodeCityLight.CSharp;
using CodeCityLight.CSharp.Parser;
using CodeCityLight.GoCity;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCityLight.tests
{
    [TestClass]
    public class IntegrationTests
    {
        /*
        Namespace: Petshop.Messaging.Email
        Namespace: Petshop.Api.Rest
        Namespace: Petshop.Ordering
        Namespace: Petshop.Shipping
        */
        private const string PetshopProjectFilePath = @"..\..\..\..\Petshop\Petshop.csproj";
        private static CodeModel codeModel;

        [AssemblyInitialize]
        public static void TestSetup(TestContext testContext)
        {
            MSBuildLocator.RegisterDefaults();
            var project = MSBuildWorkspace.Create().OpenProjectAsync(PetshopProjectFilePath).Result;
            codeModel = new CodeModel("Petshop");
            new CSharpCodeModelBuilder(codeModel).BuildFrom(project);
        }

        [TestMethod]
        public void PetshopNamespaces()
        {
            Assert.AreEqual(7, codeModel.Namespaces.Count);

            CCNamespace rootByName = codeModel.GetNamespaceByName("Petshop");
            Assert.IsNotNull(rootByName);
            Assert.AreEqual(4, rootByName.Namespaces.Count);

            CCNamespace rootByParentNull = codeModel.GetRootNamespaces()[0];
            Assert.AreEqual("Petshop", rootByParentNull.Name);
            Assert.AreEqual(4, rootByParentNull.Namespaces.Count);
            Assert.AreEqual(rootByName, rootByParentNull);

            CCNamespace messaging = codeModel.GetNamespaceByName("Petshop.Messaging");
            Assert.IsNotNull(messaging);
            Assert.AreEqual(1, messaging.Namespaces.Count);

            CCNamespace email = codeModel.GetNamespaceByName("Petshop.Messaging.Email");
            CCNamespace rootFromEmail = email.Parent.Parent;
            Assert.IsNotNull(rootFromEmail);
            Assert.AreEqual(rootByName, rootFromEmail);
        }

        [TestMethod]
        public void PetshopEmailClasses()
        {
            CCNamespace email = codeModel.GetNamespaceByName("Petshop.Messaging.Email");
            Assert.AreEqual(1, email.Classes.Count);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfFields()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            CCClass orderManagement = ordering.GetClassByName("OrderManagement");

            Assert.AreEqual(1, orderManagement.NumberOfFields);
        }

        [TestMethod]
        public void PetshopOrderNumberOfProperties()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            CCClass order = ordering.GetClassByName("Order");

            Assert.AreEqual(1, order.NumberOfProperties);
        }

        [TestMethod]
        public void PetshopOrderingOutgoingDependencies()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            Assert.AreEqual(1, ordering.OutgoingDependencies);
        }

        [TestMethod]
        public void PetshopOrderingIncomingDependencies()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            Assert.AreEqual(1, ordering.IncomingDependencies);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfMethods()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            CCClass orderManagement = ordering.GetClassByName("OrderManagement");

            Assert.AreEqual(2, orderManagement.NumberOfMethods);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfStatements()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            CCClass orderManagement = ordering.GetClassByName("OrderManagement");

            // Only statements, no blocks (but conditions inside a loop of if statement)
            Assert.AreEqual(19, orderManagement.NumberOfStatements);
        }

        [TestMethod]
        public void PetshopOrderManagementNumberOfIndependentPaths()
        {
            CCNamespace ordering = codeModel.GetNamespaceByName("Petshop.Ordering");
            CCClass orderManagement = ordering.GetClassByName("OrderManagement");

            // two methods, if statements, loops, catch clauses, switch cases.
            Assert.AreEqual(13, orderManagement.NumberOfIndependentPaths);
        }

        [TestMethod]
        public void SerializeCodeModelAsJson()
        {
            var json = codeModel.ToJson(true);
            Assert.IsTrue(json.Contains("Districts"));
            Assert.IsTrue(json.Contains("Buildings"));
            Assert.IsTrue(json.Contains("\"FullName\": \"Petshop.Ordering.OrderManagement\""));
        }

        [TestMethod]
        public void SerializeGoCityModelAsJson()
        {
            var goCityModel = CodeModel2GoCity.Convert(codeModel);
            var json = goCityModel.ToJson(true);
            System.Console.WriteLine(json);
            Assert.IsTrue(json.Contains("children"));
            Assert.IsTrue(json.Contains("numberOfLines"));
            Assert.IsTrue(json.Contains("\"name\": \"OrderManagement\""));
            Assert.IsTrue(json.Contains("\"url\": \"Petshop.Ordering.OrderManagement\""));
        }
    }
}

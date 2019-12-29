using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCityLight.Parser.CSharp
{
    public class CSharpCityModelBuilder : CSharpSyntaxWalker
    {
        private CodeCity codeCity;

        public CSharpCityModelBuilder(CodeCity codeCity)
        {
            this.codeCity = codeCity;
        }

        public void BuildFrom(Solution solution)
        {
            foreach (var project in solution.Projects)
            {
                BuildFrom(project);
            }
        }

        public void BuildFrom(Project project)
        {
            foreach (var document in project.Documents)
            {
                BuildFrom(document);
            }
        }

        public void BuildFrom(Document document)
        {
            Task<SyntaxNode> task = document.GetSyntaxRootAsync();
            task.ContinueWith(t =>
            {
                SyntaxNode syntaxRootOfDocument = t.Result;
                BuildFrom(syntaxRootOfDocument);
            });
            task.Wait();
        }

        public void BuildFrom(SyntaxNode syntaxRootOfDocument)
        {
            base.Visit(syntaxRootOfDocument);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            // Ensure that a district is created for this namespace
            string namespaceName = GetNamespaceName(node);
            District district = codeCity.EnsureDistrict(namespaceName);

            // Collect distinct usings from inside the namespace declaration and from the surrounding compilation unit.
            CompilationUnitSyntax cunit = GetCompilationUnit(node);
            HashSet<string> usings = new HashSet<string>();
            foreach (var u in node.Usings)
            {
                usings.Add(u.Name.ToString());
            }
            foreach (var u in cunit.Usings)
            {
                usings.Add(u.Name.ToString());
            }

            // Increment outgoing dependencies of the current namespace declaration
            district.OutgoingDependencies += usings.Count;

            // Increment incoming depedencies for each referenced namespace. 
            foreach(var uname in usings)
            {
                District d = codeCity.EnsureDistrict(uname);
                d.IncomingDependencies++;
            }

            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Ensure that a building is created for this class declaration
            string namespaceName = GetNamespaceName(node);
            string className = GetClassName(node);
            codeCity.EnsureBuilding(namespaceName, className);
            base.VisitClassDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // Increment number of fields
            Building building = GetBuilding(node);
            building.NumberOfFields++;
            base.VisitFieldDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Increment number of properties
            Building building = GetBuilding(node);
            building.NumberOfProperties++;
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            IncrementNumberOfMethodsForBuilding(node);
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            IncrementNumberOfMethodsForBuilding(node);
            base.VisitMethodDeclaration(node);
        }
        
        public override void Visit(SyntaxNode node)
        {
            // Increment number of statements
            if(node is StatementSyntax && !(node is BlockSyntax))
            {
                Building bulding = GetBuilding(node);
                bulding.NumberOfStatements++;
            }
            base.Visit(node);
        }

        #region privates
        private void IncrementNumberOfMethodsForBuilding(BaseMethodDeclarationSyntax node)
        {
            Building building = GetBuilding(node);
            building.NumberOfMethods++;
        }

        private Building GetBuilding(SyntaxNode node)
        {
            string namespaceName = GetNamespaceName(node);
            string className = GetClassName(node);
            return codeCity.GetBuildingByName(namespaceName, className);
        }

        private string GetNamespaceName(SyntaxNode node)
        {
            if(node == null)
            {
                return "";
            }
            if(node.IsKind(SyntaxKind.NamespaceDeclaration)) {
                return (node as NamespaceDeclarationSyntax).Name.ToString();
            }
            return GetNamespaceName(node.Parent);
        }

        private string GetClassName(SyntaxNode node)
        {
            if(node == null)
            {
                return "";
            }
            if(node.IsKind(SyntaxKind.ClassDeclaration))
            {
                return (node as ClassDeclarationSyntax).Identifier.ValueText;
            }
            return GetClassName(node.Parent);
        }
        private CompilationUnitSyntax GetCompilationUnit(SyntaxNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException();
            }
            if (node.IsKind(SyntaxKind.CompilationUnit))
            {
                return (node as CompilationUnitSyntax);
            }
            return GetCompilationUnit(node.Parent);
        }
        #endregion
    }
}

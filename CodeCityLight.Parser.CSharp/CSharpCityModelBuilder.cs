using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
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
            string namespaceName = GetNamespaceName(node);
            District district = codeCity.EnsureDistrict(namespaceName);
            district.OutgoingDependencies += node.Usings.Count;
            district.OutgoingDependencies += GetCompilationUnit(node).Usings.Count;
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            string namespaceName = GetNamespaceName(node);
            string className = GetClassName(node);
            codeCity.EnsureBuilding(namespaceName, className);
            base.VisitClassDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            Building building = GetBuilding(node);
            building.NumberOfFields++;
            base.VisitFieldDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Building building = GetBuilding(node);
            building.NumberOfProperties++;
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Building building = GetBuilding(node);
            building.NumberOfMethods++;
            base.VisitMethodDeclaration(node);
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
    }
}

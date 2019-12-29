using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCityLight.Parser.CSharp
{
    public class CSharpCodeModelBuilder : CSharpSyntaxWalker
    {
        private CodeModel codeModel;

        public CSharpCodeModelBuilder(CodeModel codeModel)
        {
            this.codeModel = codeModel;
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
            // Ensure that a CCNamespace is created for this namespace
            string namespaceName = GetNamespaceName(node);
            CCNamespace ns = codeModel.EnsureNamespace(namespaceName);

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
            ns.OutgoingDependencies += usings.Count;

            // Increment incoming depedencies for each referenced namespace. 
            foreach(var uname in usings)
            {
                CCNamespace d = codeModel.EnsureNamespace(uname);
                d.IncomingDependencies++;
            }

            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Ensure that a CCClass is created for this class declaration
            string namespaceName = GetNamespaceName(node);
            string className = GetClassName(node);
            codeModel.EnsureClass(namespaceName, className);
            base.VisitClassDeclaration(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // Increment number of fields
            GetClassFor(node).NumberOfFields++;
            base.VisitFieldDeclaration(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Increment number of properties
            GetClassFor(node).NumberOfProperties++;
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            IncrementNumberOfMethodsForClassOf(node);
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            IncrementNumberOfMethodsForClassOf(node);
            base.VisitMethodDeclaration(node);
        }
        
        public override void Visit(SyntaxNode node)
        {
            // Increment number of statements
            if(node is StatementSyntax && !(node is BlockSyntax))
            {
                GetClassFor(node).NumberOfStatements++;
            }
            base.Visit(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitForStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitForEachStatement(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitWhileStatement(node);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitDoStatement(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitCatchClause(node);
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitConditionalExpression(node);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitIfStatement(node);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitElseClause(node);
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            GetClassFor(node).NumberOfIndependentPaths++;
            base.VisitCaseSwitchLabel(node);
        }

        #region privates
        private void IncrementNumberOfMethodsForClassOf(BaseMethodDeclarationSyntax node)
        {
            CCClass c = GetClassFor(node);
            c.NumberOfMethods++;
            c.NumberOfIndependentPaths++;
        }

        private CCClass GetClassFor(SyntaxNode node)
        {
            string namespaceName = GetNamespaceName(node);
            string className = GetClassName(node);
            return codeModel.GetClassByName(namespaceName, className);
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

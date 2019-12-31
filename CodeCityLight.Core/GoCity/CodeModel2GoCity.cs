using CodeCityLight.CSharp;

namespace CodeCityLight.GoCity
{
    public static class CodeModel2GoCity
    {
        public static GoCityNode Convert(CodeModel codeModel)
        {
            var root = new GoCityNode
            {
                Name = codeModel.Name,
            };
            foreach (var ns in codeModel.GetRootNamespaces())
            {
                root.Children.Add(CreateGoCityNode(ns));
            }
            root.GenerateChildrenPosition();
            return root;
        }

        private static GoCityNode CreateGoCityNode(CCNamespace ns)
        {
            var nsNode = new GoCityNode
            {
                Name = ns.Name,
                NType = GoCityNodeType.PACKAGE,
            };
            foreach (var c in ns.Classes)
            {
                nsNode.Children.Add(CreateGoCityNode(c));
            }
            foreach (var n in ns.Namespaces)
            {
                nsNode.Children.Add(CreateGoCityNode(n));
            }
            return nsNode;
        }

        private static GoCityNode CreateGoCityNode(CCClass c)
        {
            return new GoCityNode
            {
                Name = c.Name,
                Url = c.FullName,
                NumberOfAttributes = c.NumberOfFields + c.NumberOfProperties,
                NumberOfLines = c.NumberOfStatements,
                NumberOfMethods = c.NumberOfMethods,
                NType = GoCityNodeType.FILE,
            };
        }
    }
}
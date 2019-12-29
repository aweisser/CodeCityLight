using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeCityLight.Parser.CSharp
{
    /// <summary>
    /// The <see cref="CodeModel"/> is a the top level element of the domain model.
    /// It just contains a unique name and and a list of <see cref="CCNamespace"/>s.
    /// Each <see cref="CCNamespace"/> has a unique (fully qualified) name and may contain <see cref="CCClass"/>es or further <see cref="CCNamespace"/>s.
    /// An instance of <see cref="CodeModel"/> provides convinient methods for adding and getting <see cref="CCNamespace"/>s and <see cref="CCClass"/>es to and from the hierarchy.
    /// </summary>
    public class CodeModel : NamedIdentity
    {
        public List<CCNamespace> Namespaces { get; } = new List<CCNamespace>();

        public CodeModel(string name) : base(name) { }

        public CCNamespace EnsureNamespace(string nameOfNamespace)
        {
            CCNamespace parent = null;
            foreach(var segment in Regex.Split(nameOfNamespace, "\\."))
            {
                string fullName = parent != null ? string.Join(".", parent.Name, segment) : segment;
                var ns = Namespaces.Find(d => d.Name.Equals(fullName));
                if (ns == null)
                {
                    ns = new CCNamespace(fullName, parent);
                    Namespaces.Add(ns);
                }
                parent = ns;
            }
            return GetNamespaceByName(nameOfNamespace);
        }

        public CCNamespace GetNamespaceByName(string name)
        {
            return Namespaces.Find(d => d.Name.Equals(name));
        }

        public List<CCNamespace> GetRootNamespace()
        {
            return Namespaces.FindAll(d => d.Parent == null);
        }

        public void EnsureClass(string nameOfNamespace, string nameOfClass)
        {
            CCNamespace ns = EnsureNamespace(nameOfNamespace);
            CCClass c = new CCClass(nameOfClass, ns);
            if(!ns.Classes.Contains(c))
            {
                ns.Classes.Add(c);
            }
        }

        public CCClass GetClassByName(string nameOfNamespace, string nameOfClass)
        {
            return GetNamespaceByName(nameOfNamespace).GetClassByName(nameOfClass);
        }
    }

    /// <summary>
    /// A <see cref="CCNamespace"/> is a node in a tree of namespaces where the leaves are <see cref="CCClass"/>es.
    /// It also contains some metrics.
    /// </summary>
    public class CCNamespace : NamedIdentity
    {
        public List<CCNamespace> Namespaces { get; } = new List<CCNamespace>();
        public List<CCClass> Classes { get; } = new List<CCClass>();
        public CCNamespace Parent { get; }
        public int OutgoingDependencies { get; set; } = 0;
        public int IncomingDependencies { get; set; } = 0;

        public CCNamespace(string name, CCNamespace parent) : base(name)
        {
            Parent = parent;
            if (parent != null)
            {
                parent.Namespaces.Add(this);
            }
        }

        public CCClass GetClassByName(string name)
        {
            return Classes.Find(b => b.Name.Equals(name));
        }
    }

    /// <summary>
    /// A <see cref="CCClass"/> represents a leave in a tree, where the nodes are <see cref="CCNamespace"/>s.
    /// It has a unique, simple name inside its <see cref="Parent"/> namespace.
    /// </summary>
    public class CCClass : NamedIdentity
    {
        public CCNamespace Parent { get; }
        public int NumberOfFields { get; set; } = 0;
        public int NumberOfProperties { get; set; } = 0;
        public int NumberOfMethods { get; set; } = 0;
        public int NumberOfStatements { get; set; } = 0; // better than lines of code.
        public int NumberOfIndependentPaths { get; set; } = 0; // aka CyclomaticComplexity
        /*
            public int Cohesion { get; set; } = 0;
        */
        public CCClass(string name, CCNamespace parent) : base(name)
        {
            if(parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            Parent = parent;
        }
    }

    public abstract class NamedIdentity
    {
        public string Name { get; private set; }

        protected NamedIdentity(string name)
        {
            this.Name = name;
        }

        public override bool Equals(object obj)
        {
            var identity = obj as NamedIdentity;
            return identity != null &&
                   Name == identity.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeCityLight.Parser.CSharp
{
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

    public class CodeCity : NamedIdentity
    {
        public List<District> Districts { get; } = new List<District>();

        public CodeCity(string name) : base(name) { }

        public District EnsureDistrict(string nameOfDistrict)
        {
            District parent = null;
            foreach(var segment in Regex.Split(nameOfDistrict, "\\."))
            {
                string fullName = parent != null ? string.Join(".", parent.Name, segment) : segment;
                var district = Districts.Find(d => d.Name.Equals(fullName));
                if (district == null)
                {
                    district = new District(fullName, parent);
                    Districts.Add(district);
                }
                parent = district;
            }
            return GetDistrictByName(nameOfDistrict);
        }

        public District GetDistrictByName(string name)
        {
            return Districts.Find(d => d.Name.Equals(name));
        }

        public List<District> GetRootDistricts()
        {
            return Districts.FindAll(d => d.Parent == null);
        }

        public void EnsureBuilding(string nameOfDistrict, string nameOfBuilding)
        {
            District district = EnsureDistrict(nameOfDistrict);
            district.Buildings.Add(new Building(nameOfBuilding, district));
        }

        public Building GetBuildingByName(string nameOfDistrict, string nameOfBuilding)
        {
            return GetDistrictByName(nameOfDistrict).GetBuildingByName(nameOfBuilding);
        }
    }

    public class District : NamedIdentity
    {
        public List<District> Districts { get; } = new List<District>();
        public List<Building> Buildings { get; } = new List<Building>();
        public District Parent { get; }
        public int OutgoingDependencies { get; set; } = 0;
        public int IncomingDependencies { get; set; } = 0;

        public District(string name, District parent) : base(name)
        {
            Parent = parent;
            if (parent != null)
            {
                parent.Districts.Add(this);
            }
        }

        public Building GetBuildingByName(string name)
        {
            return Buildings.Find(b => b.Name.Equals(name));
        }
    }

    public class Building : NamedIdentity
    {
        public District Parent { get; }
        public int NumberOfFields { get; set; } = 0;
        public int NumberOfProperties { get; set; } = 0;
        public int NumberOfMethods { get; set; } = 0;
        public int NumberOfStatements { get; set; } = 0;
        public int NumberOfIndependentPaths { get; set; } = 0; // aka CyclomaticComplexity
        /*
            public int Cohesion { get; set; } = 0;
        */
        public Building(string name, District parent) : base(name)
        {
            Parent = parent;
        }
    }

}
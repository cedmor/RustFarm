using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RustaFarmer
{
    [Serializable]
    public struct Allele
    {
        public const string W = "W", X = "X", Y = "Y", G = "G", H = "H", R = "R";
        public static string[] validAllele = { "W", "X", "Y", "G", "H" };

        public static bool IsValid(string allele)
        {
            return validAllele.Contains(allele);
        }
    }

    [Serializable]
    public class Gene : IEquatable<Gene>
    {
        public string allele = Allele.R;
        public bool isValid = false;
        public bool isDominant = false;

        public Gene(string allele)
        {
            this.allele = allele;
            isValid = Allele.IsValid(allele);
            isDominant = (allele == Allele.X || allele == Allele.W);
        }
        public Gene(char allele) : this(allele.ToString()) { }

        public bool Equals(Gene other)
        {
            return this.allele == other.allele;
        }
    }

}

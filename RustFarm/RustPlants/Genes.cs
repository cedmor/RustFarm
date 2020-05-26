using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustFarm.RustPlants
{
    class PlantEqualityComparer : IEqualityComparer<Plant>
    {
        public bool Equals(Plant a, Plant b)
        {
            if (Object.ReferenceEquals(a, null)) return false;
            if (Object.ReferenceEquals(null, b)) return false;
            if (Object.ReferenceEquals(b, a)) return true;

            int index = 0;
            foreach (var gene in a.genes)
            {
                if (gene.value != b.genes[index++].value)
                    return false;
            }
            return true;
        }

        public int GetHashCode(Plant plant)
        {
            int sum = 0;
            for (var i = 0; i < plant.genes.Count; i++)
            {
                int tempVal = 0;
                switch (plant.genes[i].value)
                {
                    case Allele.W: tempVal = 1; break;
                    case Allele.X: tempVal = 2; break;
                    case Allele.Y: tempVal = 3; break;
                    case Allele.G: tempVal = 4; break;
                    case Allele.H: tempVal = 5; break;
                    default:
                        break;
                }
                sum += tempVal * (int)Math.Pow(10, i);
            }
            return sum;
        }
    }

    [Serializable]
    public class Plant
    {
        public List<Gene> genes = new List<Gene>();
        public List<Plant> breededFrom = new List<Plant>();
        public static int cpt = 1;
        public int creationNumber = 0;
        public Genome genome;

        private Plant(Genome genome)
        {
            this.genome = genome;
        }

        Plant(List<Gene> genes) : this(CreateGenomeFromListOfGene(genes))
        {
            this.genes = genes;
        }
        static Genome CreateGenomeFromListOfGene(List<Gene> genes)
        {
            return new Genome(genes);
        }

        public Plant(string plant) : this(CreateGeneListFromPlantString(plant)) { }
        static List<Gene> CreateGeneListFromPlantString(string plant)
        {
            List<Gene> genes = new List<Gene>();
            foreach (char c in plant)
            {
                genes.Add(new Gene(c));
            }
            return genes;
        }

        public Plant(string[,] plants) : this(CreateGeneListFromPlantStringArray(plants))
        {
            creationNumber = cpt++;
            this.Print();
        }
        static List<Gene> CreateGeneListFromPlantStringArray(string[,] plants)
        {
            List<Gene> genes = new List<Gene>();
            for (int i = 0; i < plants.Length / 2; i++)
            {
                genes.Add(new Gene(plants[0, i], plants[1, i]));
            }
            return genes;
        }

        public Plant(List<Plant> breededFrom) : this(CreatePlantFromBreeding(breededFrom)) { this.breededFrom = breededFrom; }
        static List<Gene> CreatePlantFromBreeding(List<Plant> breededFrom)
        {
            // On assume que c'est toujours le premier qui prend les gènes des autres.
            List<List<Gene>> genePools = new List<List<Gene>>();
            List<Gene> createdPlant = new List<Gene>();

            for (var i = 0; i < 6; i++)
            {
                genePools.Add(new List<Gene>());
                foreach (var plant in breededFrom)
                {
                    genePools[i].Add(plant.genes[i]);
                }
            }
            foreach (var genePool in genePools)
            {
                createdPlant.Add(new Gene(genePool));
            }
            return createdPlant;
        }

        public bool IsValid()
        {
            if (genes.Count != 6) return false;
            foreach (var gene in genes)
            {
                if (!gene.IsValid()) return false;
            }
            return true;
        }

        int GetDistanceFrom(Plant plant)
        {
            return this.genome.GetDistanceFrom(plant.genome);
        }
        public void Print(string offset = "")
        {
            if (this.IsValid())
                Console.WriteLine(offset + "Plant is Valid, creation number: " + this.creationNumber);
            else
                Console.WriteLine(offset + "Plant is InValid, creation number: " + this.creationNumber);

            Console.Write(offset);
            foreach (var gene in this.genes)
            {
                switch (gene.value)
                {
                    case Allele.W:
                    case Allele.X:
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                    case Allele.Y:
                    case Allele.G:
                    case Allele.H:
                        Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        break;
                }
                Console.Write(gene.value + "\t");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine();
        }
    }

    [Serializable]
    public class Genome
    {
        int W = 0, X = 0, Y = 0, G = 0, H = 0;

        public Genome(List<Gene> genes)
        {
            foreach (var gene in genes)
            {
                switch (gene.value)
                {
                    case Allele.W: W++; break;
                    case Allele.X: X++; break;
                    case Allele.Y: Y++; break;
                    case Allele.G: G++; break;
                    case Allele.H: H++; break;
                    default: break;
                }
            }
        }

        Genome(Plant plant) : this(plant.genes) { }

        bool IsValid()
        {
            return W + X + Y + G + H == 6;
        }

        public int GetDistanceFrom(Genome genome)
        {
            return Math.Min(this.W, genome.W) +
                Math.Min(this.X, genome.X) +
                Math.Min(this.Y, genome.Y) +
                Math.Min(this.G, genome.G) +
                Math.Min(this.H, genome.H);
        }

        public int GetVirtualDistanceFrom(Genome genome)
        {
            return this.Y * 2 + this.G * 2 + this.H - this.X - this.W;
            //Math.Min(this.Y, genome.Y) +
            //    Math.Min(this.G, genome.G) +
            //    Math.Min(this.H, genome.H) - this.X - this.W;
        }

        public List<string> GetExistingAllele()
        {
            List<string> allele = new List<string>();
            if (this.W > 0) allele.Add(Allele.W);
            if (this.X > 0) allele.Add(Allele.X);
            if (this.Y > 0) allele.Add(Allele.Y);
            if (this.G > 0) allele.Add(Allele.G);
            if (this.H > 0) allele.Add(Allele.H);
            return allele;
        }
    }

    [Serializable]
    public class Gene : IEquatable<Gene>
    {
        public string value = string.Empty;

        Gene(string allele)
        {
            if (Allele.IsValidAllele(allele)) { this.value = allele; }
        }
        public Gene(char allele) : this(allele.ToString()) { }
        public Gene(string pos1, string pos2) : this(Allele.GetMostRecentValidAllele(pos1, pos2)) { }
        public Gene(List<Gene> genes) : this(FindDominantGene(genes)) { }
        static string FindDominantGene(List<Gene> genes)
        {
            string firstAllele = genes.First().value;
            //genes.RemoveAt(0);
            string dominant = string.Empty;

            int W = genes.Count(gene => gene.value == Allele.W);
            int X = genes.Count(gene => gene.value == Allele.X);
            int Y = genes.Count(gene => gene.value == Allele.Y);
            int G = genes.Count(gene => gene.value == Allele.G);
            int H = genes.Count(gene => gene.value == Allele.H);
            int max = Math.Max(W, Math.Max(X, Math.Max(Y, Math.Max(G, H))));

            if (max == W) dominant += "W";
            if (max == X) dominant += "X";
            if (max == Y) dominant += "Y";
            if (max == G) dominant += "G";
            if (max == H) dominant += "H";

            if (dominant.Length == 1) return dominant;
            if (dominant.Contains(Allele.X) && firstAllele == Allele.X) return Allele.X;
            if (dominant.Contains(Allele.W) && firstAllele == Allele.W) return Allele.W;
            if (dominant.Contains(Allele.X) && dominant.Contains(Allele.W)) return new Random().Next(1) == 1 ? Allele.X : Allele.W;
            if (dominant.Contains(Allele.X)) return Allele.X;
            if (dominant.Contains(Allele.W)) return Allele.W;

            if (dominant.Contains(Allele.Y) && firstAllele == Allele.Y) return Allele.Y;
            if (dominant.Contains(Allele.G) && firstAllele == Allele.G) return Allele.G;
            if (dominant.Contains(Allele.H) && firstAllele == Allele.H) return Allele.H;
            if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.G)) return new Random().Next(1) == 1 ? Allele.Y : Allele.G;
            if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.H)) return new Random().Next(1) == 1 ? Allele.Y : Allele.H;
            if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H)) return new Random().Next(1) == 1 ? Allele.G : Allele.H;
            if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H) && dominant.Contains(Allele.Y))
                return new Random().Next(2) == 2 ? Allele.Y : new Random().Next(1) == 1 ? Allele.H : Allele.G;

            Console.WriteLine("&");
            return "Y";
        }

        public bool IsValid()
        {
            return value != string.Empty;
        }

        public bool Equals(Gene other)
        {
            return this.value == other.value;
        }
    }

    [Serializable]
    public static class Allele
    {
        public const string W = "W", X = "X", Y = "Y", G = "G", H = "H";
        public static string[] validAllele = { W, X, Y, G, H };

        public static bool IsValidAllele(string allele)
        {
            return validAllele.Contains(allele);
        }
        public static string GetMostRecentValidAllele(string pos1, string pos2)
        {
            if (IsValidAllele(pos2)) { return pos2; }
            return pos1;
        }
    }

}

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace RustaFarmer
{
    class PlantEqualityComparer : IEqualityComparer<Plant>
    {
        public bool Equals(Plant a, Plant b)
        {
            if (Object.ReferenceEquals(a, null)) return false;
            if (Object.ReferenceEquals(null, b)) return false;
            if (Object.ReferenceEquals(b, a)) return true;

            int index = 0;
            if (!a.IsValid() || !b.IsValid()) return false;
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
        public int scanId = 0;
        public Genome genome;
        public int isValid = -1;

        public int ScanId { get { return scanId; } set { if (ScanId != value) { scanId = value; } } }
        public PackIconKind GetFirstGeneKind { get { return GetSpecificGeneKind(1); } }
        public PackIconKind GetSecondGeneKind { get { return GetSpecificGeneKind(2); } }
        public PackIconKind GetThirdGeneKind { get { return GetSpecificGeneKind(3); } }
        public PackIconKind GetFourthGeneKind { get { return GetSpecificGeneKind(4); } }
        public PackIconKind GetFifthGeneKind { get { return GetSpecificGeneKind(5); } }
        public PackIconKind GetSixthGeneKind { get { return GetSpecificGeneKind(6); } }
        public SolidColorBrush GetFirstGeneBrush { get { return GetSpecificGeneBrush(1); } }
        public SolidColorBrush GetSecondGeneBrush { get { return GetSpecificGeneBrush(2); } }
        public SolidColorBrush GetThirdGeneBrush { get { return GetSpecificGeneBrush(3); } }
        public SolidColorBrush GetFourthGeneBrush { get { return GetSpecificGeneBrush(4); } }
        public SolidColorBrush GetFifthGeneBrush { get { return GetSpecificGeneBrush(5); } }
        public SolidColorBrush GetSixthGeneBrush { get { return GetSpecificGeneBrush(6); } }

        private SolidColorBrush GetSpecificGeneBrush(int geneNumber)
        {
            if (IsValid())
            {
                switch (this.genes.ElementAt(geneNumber - 1).value.ToString())
                {
                    case "Y": return System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush;
                    case "G": return System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush;
                    case "H": return System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush;
                    case "W": return System.Windows.Application.Current.TryFindResource("SecondaryHueMidBrush") as SolidColorBrush;
                    case "X": return System.Windows.Application.Current.TryFindResource("SecondaryHueMidBrush") as SolidColorBrush;
                    default:
                        break;
                }
            }
            return System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush;
        }

        private PackIconKind GetSpecificGeneKind(int geneNumber)
        {
            if (IsValid())
            {
                switch (this.genes.ElementAt(geneNumber - 1).value.ToString())
                {
                    case "Y": return PackIconKind.AlphaYCircle;
                    case "G": return PackIconKind.AlphaGCircle;
                    case "H": return PackIconKind.AlphaHCircle;
                    case "W": return PackIconKind.AlphaWCircle;
                    case "X": return PackIconKind.AlphaXCircle;
                    default: return PackIconKind.Rabbit;
                }
            }
            else
            {
                return PackIconKind.Rabbit;
            }
        }



        private Plant(Genome genome)
        {
            scanId = 0;
            this.genome = genome;
        }

        public Plant(List<Gene> genes) : this(CreateGenomeFromListOfGene(genes))
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
            scanId = cpt++;
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

        public int GetComplexity()
        {
            int complexity = 0;
            if (breededFrom.Count != 0)
            {
                breededFrom.ForEach(p => { complexity = +p.GetComplexity(); });
                return complexity;
            }
            else return 1;

        }

        public bool IsValid()
        {
            if (isValid != -1)
                return isValid == 0 ? false : true;

            if (genes.Count != 6) return false;
            foreach (var gene in genes)
            {
                if (!gene.IsValid()) return false;
            }
            return true;
        }

        public int GetDistanceFrom(Plant plant)
        {
            return this.genome.GetDistanceFrom(plant.genome);
        }
        public void Print(string offset = "")
        {
            if (this.IsValid())
                Console.WriteLine(offset + "Plant is Valid, creation number: " + this.scanId);
            else
                Console.WriteLine(offset + "Plant is InValid, creation number: " + this.scanId);

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

        public bool IsBetterThan(Plant plant)
        {
            bool proofItsBetter = false;
            if (plant.IsValid())
            {
                foreach (var gene in this.genes)
                {
                    var correspondingGene = plant.genes.ElementAt(genes.IndexOf(gene));
                    if (!gene.Equals(correspondingGene))
                    {
                        if (!gene.isDominant() && correspondingGene.isDominant())
                        {
                            proofItsBetter = true;
                        }
                        else return false;
                    }
                }
                return proofItsBetter;
            }
            else { return false; }
        }

        public static List<Gene> GetRandomPlant(Random rng)
        {
            string W = "W", X = "X", Y = "Y", G = "G", H = "H";
            string[] validAllele = { W, X, Y, G, H };

            List<Gene> genes = new List<Gene>();
            for (int i = 0; i < 6; i++)
            {
                var alleleIndex = rng.Next(0, validAllele.Length);
                genes.Add(new Gene(validAllele[alleleIndex]));
            }
            return genes;
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

        public bool IsValid()
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
            //return this.Y * 2 + this.G * 2 + this.H - this.X - this.W;
            return Math.Min(this.Y, genome.Y) * 2 +
                Math.Min(this.G, genome.G) * 2 +
                Math.Min(this.H, genome.H) * 2 - this.X - this.W;
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
        public string value = "R";

        public Gene(string allele)
        {
            if (Allele.IsValidAllele(allele)) { this.value = allele; }
        }
        public Gene(char allele) : this(allele.ToString()) { }
        public Gene(string pos1, string pos2) : this(Allele.GetMostRecentValidAllele(pos1, pos2)) { }
        public Gene(List<Gene> genes) : this(FindDominantGene2(genes)) { }
        static string FindDominantGene2(List<Gene> genes)
        {
            string firstAllele = genes.First().value;
            genes.RemoveAt(0);
            string dominant = string.Empty;

            double W = genes.Count(gene => gene.value == Allele.W);
            double X = genes.Count(gene => gene.value == Allele.X);
            double Y = genes.Count(gene => gene.value == Allele.Y) * 0.6;
            double G = genes.Count(gene => gene.value == Allele.G) * 0.6;
            double H = genes.Count(gene => gene.value == Allele.H) * 0.6;
            double max = Math.Max(W, Math.Max(X, Math.Max(Y, Math.Max(G, H))));
            if (genes.Count(gene => gene.value == "R") > 0) return "R";
            if (max < 1) return firstAllele;

            if (max == W) dominant += "W";
            if (max == X) dominant += "X";
            if (max == Y) dominant += "Y";
            if (max == G) dominant += "G";
            if (max == H) dominant += "H";

            if (dominant.Length == 1) return dominant;
            return "R";

        }
        static string FindDominantGene(List<Gene> genes)
        {
            // TODO Rework avec nouvelle règle :
            //Not in all cases. If the plants you are using crossbreeding for dont have at least 2 same green genes or one red on the specific position, then the gene from base plant is not replaced.

            string firstAllele = genes.First().value;
            genes.RemoveAt(0);
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
            // A valider : Est-ce que si on a un breeding avec 1X et 1Y, la première plante est utilisé dans le calcul du gène ?
            if (dominant.Length == 1) return dominant;
            if (dominant.Contains(Allele.X) && firstAllele == Allele.X) return Allele.X;
            if (dominant.Contains(Allele.W) && firstAllele == Allele.W) return Allele.W;
            if (dominant.Contains(Allele.X) && dominant.Contains(Allele.W)) return "R";
            //if (dominant.Contains(Allele.X) && dominant.Contains(Allele.W)) return new Random().Next(1) == 1 ? Allele.X : Allele.W;

            if (dominant.Contains(Allele.X)) return Allele.X;
            if (dominant.Contains(Allele.W)) return Allele.W;

            if (dominant.Contains(Allele.Y) && firstAllele == Allele.Y) return Allele.Y;
            if (dominant.Contains(Allele.G) && firstAllele == Allele.G) return Allele.G;
            if (dominant.Contains(Allele.H) && firstAllele == Allele.H) return Allele.H;
            if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.G)) return "R";
            if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.H)) return "R";
            if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H)) return "R";
            //if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.G)) return new Random().Next(1) == 1 ? Allele.Y : Allele.G;
            //if (dominant.Contains(Allele.Y) && dominant.Contains(Allele.H)) return new Random().Next(1) == 1 ? Allele.Y : Allele.H;
            //if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H)) return new Random().Next(1) == 1 ? Allele.G : Allele.H;
            if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H) && dominant.Contains(Allele.Y))
                return "R";
            //if (dominant.Contains(Allele.G) && dominant.Contains(Allele.H) && dominant.Contains(Allele.Y))
            //    return new Random().Next(2) == 2 ? Allele.Y : new Random().Next(1) == 1 ? Allele.H : Allele.G;

            Console.WriteLine("&");
            return "P";
        }

        public bool isDominant()
        {
            return !(value == "Y" || value == "H" || value == "G");
        }

        public bool IsValid()
        {
            return value != string.Empty && value != "R";
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

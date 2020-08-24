using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using Process.NET.Assembly.CallingConventions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
            foreach (var gene in a.genes)
            {
                if (gene.allele != b.genes[index++].allele)
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
                switch (plant.genes[i].allele)
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
        public static int cpt = 1;

        public List<Gene> genes = new List<Gene>();
        public List<Plant> breededFrom = new List<Plant>();

        public string scanId;
        public bool isValid = false;

        #region UI methods
        public string ScanId { get { return scanId; } set { if (ScanId != value) { scanId = value; } } }
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
            return (this.genes.ElementAt(geneNumber - 1).allele.ToString()) switch
            {
                "Y" => System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush,
                "G" => System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush,
                "H" => System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush,
                "W" => System.Windows.Application.Current.TryFindResource("SecondaryHueMidBrush") as SolidColorBrush,
                "X" => System.Windows.Application.Current.TryFindResource("SecondaryHueMidBrush") as SolidColorBrush,
                _ => System.Windows.Application.Current.TryFindResource("PrimaryHueMidBrush") as SolidColorBrush,
            };
        }
        private PackIconKind GetSpecificGeneKind(int geneNumber)
        {
            return (this.genes.ElementAt(geneNumber - 1).allele.ToString())
                switch
            {
                "Y" => PackIconKind.AlphaYCircle,
                "G" => PackIconKind.AlphaGCircle,
                "H" => PackIconKind.AlphaHCircle,
                "W" => PackIconKind.AlphaWCircle,
                "X" => PackIconKind.AlphaXCircle,
                _ => PackIconKind.Rabbit,
            };
        }
        #endregion

        public Plant(List<Gene> genes, string scanId = "Created")
        {
            this.scanId = scanId;
            this.genes = genes;
            if (genes.All(g => g.isValid) && genes.Count == 6)
                this.isValid = true;
        }

        public Plant(string plant) : this(CreateGeneListFromPlantString(plant), cpt++.ToString()) { }
        static List<Gene> CreateGeneListFromPlantString(string plant)
        {
            List<Gene> genes = new List<Gene>();
            foreach (char allele in plant)
            {
                genes.Add(new Gene(allele));
            }
            return genes;
        }

        public Plant(List<Plant> breededFrom) : this(CreatePlantFromBreeding(breededFrom)) { this.breededFrom = breededFrom; }
        static List<Gene> CreatePlantFromBreeding(List<Plant> breededFrom)
        {
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
                createdPlant.Add(new Gene(Breed(genePool)));
            }
            return createdPlant;
        }
        static string Breed(List<Gene> genes)
        {
            // Do not evaluate if a gene is Random.
            // Maybe remove this test because non valid plants will get removed from analyze ?
            if (genes.Exists(g => !g.isValid)) return Allele.R;

            string firstAllele = genes.First().allele; genes.RemoveAt(0);
            Dictionary<string, double> breed = new Dictionary<string, double>();
            foreach (var g in genes)
            {
                double value = g.isDominant ? 1 : 0.6;
                //if (breed.ContainsKey(g.allele)) { breed[g.allele] += value; }
                double outvalue = 0;
                if (breed.TryGetValue(g.allele, out outvalue))
                {
                    breed[g.allele] = outvalue + value;
                }
                else { breed.Add(g.allele, value); }
            }
            var maxValueGene = breed.Max(x => x.Value);
            if (maxValueGene < 1) return firstAllele;
            else if (breed.Count(x => x.Value == maxValueGene) > 1) return Allele.R;
            else return breed.FirstOrDefault(x => x.Value == maxValueGene).Key;
        }

        public double GetScore()
        {
            double score = 0;
            foreach (var g in genes)
            {
                score += g.allele switch
                {
                    Allele.Y => 1,
                    Allele.G => 1,
                    Allele.H => 0.5,
                    Allele.X => 0,
                    Allele.W => -0.2,
                    _ => 1,
                };
            }
            return score;
        }

        public int GetDistanceFrom(Plant plant)
        {
            Dictionary<string, int> genomePlant = new Dictionary<string, int>();
            foreach (var g in plant.genes)
            {
                if (genomePlant.ContainsKey(g.allele)) { genomePlant[g.allele] += 1; }
                else { genomePlant.Add(g.allele, 1); }
            }

            Dictionary<string, int> genomeThis = new Dictionary<string, int>();
            foreach (var g in this.genes)
            {
                if (genomeThis.ContainsKey(g.allele)) { genomeThis[g.allele] += 1; }
                else { genomeThis.Add(g.allele, 1); }
            }

            int distance = 0;
            foreach (var key in genomePlant.Keys)
            {
                int value1; genomePlant.TryGetValue(key, out value1);
                int value2; genomeThis.TryGetValue(key, out value2);

                distance += Math.Min(value1, value2);
            }

            return distance;
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


        public bool IsBetterThan(Plant plant)
        {
            bool proofItsBetter = false;
            foreach (var gene in this.genes)
            {
                var correspondingGene = plant.genes.ElementAt(genes.IndexOf(gene));
                if (!gene.Equals(correspondingGene))
                {
                    if (!gene.isDominant && correspondingGene.isDominant)
                        proofItsBetter = true;
                    else return false;
                }
            }
            return proofItsBetter;
        }


        public bool IsBetterThanOrEqualTo(Plant plant)
        {
            bool proofItsBetter = true;
            foreach (var gene in this.genes)
            {
                var correspondingGene = plant.genes.ElementAt(genes.IndexOf(gene));
                if (!gene.Equals(correspondingGene))
                {
                    if (!(!gene.isDominant && correspondingGene.isDominant))
                        return false;
                }
            }
            return proofItsBetter;
        }
    }
}

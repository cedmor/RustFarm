using Rust.Farm.RustPlants;
using System;
using System.Collections.Generic;
using System.Linq;
using Rust.Utils;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Threading;
using System.Diagnostics;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Rust.Farm
{
    public class Analyzer
    {
        Random rng = new Random();

        public void Execute(List<Plant> plants, Plant wishPlant)
        {
            do
            {
                var rngnb = rng.Next(10, 100);
                for (var j = 0; j < 10; j++)
                {
                    var startingPlants = new List<Plant>();
                    for (int i = 0; i < rngnb; i++)
                    {
                        Plant p = new Plant(Plant.GetRandomPlant(rng));
                        startingPlants.Add(p);
                    }
                    var er = new ExecutionReport(startingPlants.Count);
                    Plant wish = StartGeneration(startingPlants, new List<Plant>(), wishPlant, 1, er);

                    //Plant wish = ExecuteRound(plants, wishPlant, 0, 0);
                    //Plant wish = StartGeneration(plants, new List<Plant>(), wishPlant, 1);

                    if (wish.genome.GetDistanceFrom(wishPlant.genome) == 6)
                    {
                        er.plantFound = true;
                        PrintResult(wish);
                    }
                    else
                    {
                        er.plantFound = false;
                        Console.WriteLine("404 - NotFound.");
                    }
                    //er.WriteReport(@"./execreport.txt");
                    er.WriteReport(@"./execreport5.txt");

                }
            } while (true);
        }

        public void PrintResult(Plant plant, string offset = "")
        {
            plant.Print(offset);
            if (plant.breededFrom.Count > 0) Console.WriteLine(offset + "Breeded from :");
            offset = offset += "\t";
            foreach (var p in plant.breededFrom)
            {
                PrintResult(p, offset);
            }
        }

        public List<Plant> StartBreeding<T>(IEnumerable<T> inputPlants, Plant wishPlant)
        {
            List<Plant> wishPlants = new List<Plant>();
            return wishPlants;
        }

        public Plant StartGeneration<T>(IEnumerable<T> newPlantsFromLastRound,
            IEnumerable<T> initPlantsFromLastRound, Plant wishPlant, int genNumber, ExecutionReport er)
        {
            er.nbOfGenerationForFinding = genNumber;
            if ((newPlantsFromLastRound.Count() == 0 && genNumber != 1) || genNumber > 1) { return new Plant("NotFound"); };

            Console.WriteLine("\nStarting Generation of breeding - number: " + genNumber);
            List<Plant> plantPool = new List<Plant>(initPlantsFromLastRound.Count()
                + newPlantsFromLastRound.Count());
            plantPool.AddRange((List<Plant>)initPlantsFromLastRound);
            plantPool.AddRange((List<Plant>)newPlantsFromLastRound);


            List<Plant> plantsToRemoveFromLastRound = new List<Plant>();
            //Console.WriteLine("Removing plants...");
            var countBeforeRemoving = plantPool.Count();
            foreach (var newPlant in newPlantsFromLastRound)
            {
                plantPool.RemoveAll(p => ((Plant)(object)newPlant).IsBetterThan(p));
            }
            Console.WriteLine(countBeforeRemoving - plantPool.Count() + " plants have been removed.");
            ((List<Plant>)newPlantsFromLastRound).RemoveAll(p => !plantPool.Contains(p, new PlantEqualityComparer()));


            List<Plant> newPlantPool = new List<Plant>();
            Console.WriteLine("Number of plants in input: " + plantPool.Count() + " / 15625 plants possible.");
            Console.WriteLine("Number of new plants in previous Generation: " + newPlantsFromLastRound.Count());

            foreach (var currentPlant in (List<Plant>)newPlantsFromLastRound)
            {
                for (var index = 1; index < 4; index++)
                {
                    foreach (IEnumerable<T> permutation in PermuteUtils.PermuteUnique<T>((IEnumerable<T>)plantPool, index))
                    {
                        List<Plant> permutedPlants = new List<Plant>();
                        permutedPlants.Add(currentPlant);

                        foreach (T i in permutation)
                        {

                            permutedPlants.Add((Plant)(object)i);
                        }
                        Plant newPlant = new Plant(permutedPlants);

                        if (newPlant.genome.GetDistanceFrom(wishPlant.genome) == 6)
                        {
                            return newPlant;
                        }
                        else if (!plantPool.Contains(newPlant, new PlantEqualityComparer())
                            && !newPlantPool.Contains(newPlant, new PlantEqualityComparer()))
                        {
                            newPlantPool.Add(newPlant);
                        }
                    }
                }
            }
            return StartGeneration(newPlantPool, plantPool, wishPlant, ++genNumber, er);
        }

        public int Combinations(int n, int k)
        {
            int count = n;

            for (int x = 1; x <= k - 1; x++)
            {
                count = count * (n - x) / x;
            }

            return count / k;
        }

        public Plant ExecuteRound<T>(IEnumerable<T> input, Plant wishPlant, int roundNb, int previousCount)
        {
            if (input.Count() == previousCount)
            {
                Console.WriteLine("Could not find wish plant with this gene pool, please find other seeds.");
                return new Plant("NOTVALID");
            }

            if (roundNb > 10)
            {
                Console.WriteLine("Stop!");
                return new Plant("NOTVALID");
            }

            Console.WriteLine("Starting Generation of breeding number: " + roundNb);
            Console.WriteLine("Number of plants in input: " + input.Count());

            List<Plant> plantPool = CleaningPlantPool(input, wishPlant, roundNb);
            Console.WriteLine("Number of plants after cleaning: " + plantPool.Count());

            List<Plant> newPlantPool = StartBreedingRound(plantPool, wishPlant, roundNb);

            foreach (var p in newPlantPool)
            {
                if (p.genome.GetDistanceFrom(wishPlant.genome) == 6)
                {
                    Console.WriteLine("Wish plant has been found.");
                    return p;
                }
            }

            Console.WriteLine("Number of plants after cleanning : " + plantPool.Count() + " (min. distance  : " + Math.Min(roundNb, 4) + ")");
            Console.WriteLine("Starting cross-breeding iteration number: ");

            plantPool.AddRange(newPlantPool);
            return ExecuteRound(plantPool, wishPlant, ++roundNb, input.Count());
        }

        public List<Plant> StartBreedingRound<T>(IEnumerable<T> plantPool, Plant wishPlant, int roundNb)
        {
            List<Plant> newPlantRound = new List<Plant>();
            //for (var index = 2; index < (6 - Math.Min(roundNb, 3)); index++)
            for (var index = 2; index < 5; index++)
            {
                Console.Write("\t" + (index - 1) + " - Number of new plants in this iteration  : ...");
                List<Plant> newPlantsIteration = new List<Plant>();
                int currentBestDistance = Math.Min(roundNb, 4);
                foreach (IEnumerable<T> permutation in PermuteUtils.PermuteUnique<T>((IEnumerable<T>)plantPool, index))
                {
                    List<Plant> permutedPlants = new List<Plant>();
                    foreach (T i in permutation)
                    {
                        permutedPlants.Add((Plant)(object)i);
                    }
                    Plant newPlant = new Plant(permutedPlants);
                    currentBestDistance = Math.Max(currentBestDistance, ((Plant)(object)newPlant).genome.GetVirtualDistanceFrom(wishPlant.genome));


                    newPlantsIteration.Add(newPlant);

                    //if (!permutedPlants.Contains(newPlant, new PlantEqualityComparer()) &&
                    //    newPlant.genome.GetVirtualDistanceFrom(wishPlant.genome) > currentBestDistance - 2)
                    //{
                    //    newPlantsIteration.Add(newPlant);
                    //}
                    if (newPlant.genome.GetDistanceFrom(wishPlant.genome) == 6) return newPlantsIteration;
                }
                newPlantsIteration = newPlantsIteration.Distinct(new PlantEqualityComparer()).ToList();

                Console.WriteLine(newPlantsIteration.Count());
                newPlantRound.AddRange(newPlantsIteration);
            }
            return newPlantRound;
        }

        public List<Plant> CleaningPlantPool<T>(IEnumerable<T> input, Plant wishPlant, int roundNb)
        {
            List<Plant> plantPool = (List<Plant>)input;
            plantPool.RemoveAll(plant => plant.IsValid() == false);
            plantPool = plantPool.Distinct(new PlantEqualityComparer()).ToList();

            List<String> wishAllele = wishPlant.genome.GetExistingAllele();
            int subPoolNumber = 12;
            List<Plant> subPoolList = new List<Plant>();
            for (var geneIdx = 0; geneIdx < 5; geneIdx++)
            {
                List<Plant> subPool = new List<Plant>();
                foreach (var allele in wishAllele)
                {
                    subPool.AddRange(plantPool.FindAll(plant => plant.genes[geneIdx].value == allele)
                        .OrderByDescending(plant => plant.genome.GetVirtualDistanceFrom(wishPlant.genome))
                        .Take(subPoolNumber).ToList());
                }
                if (subPool.Count == 0)
                {
                    Console.WriteLine("Missing allele from gene " + (geneIdx + 1) + " in Plant pool.");
                    return new List<Plant>();
                }
                subPoolList.AddRange(subPool);
            }
            return subPoolList;
        }
    }

    public class ExecutionReport
    {
        public Stopwatch sp;
        public int nbPlantsInPool;
        public int nbOfGenerationForFinding;
        public bool plantFound;

        public ExecutionReport(int nbPlants)
        {
            nbPlantsInPool = nbPlants;
            sp = new Stopwatch();
            sp.Start();
        }

        public void WriteReport(string fileName)
        {
            sp.Stop();
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(fileName, true))
            {
                file.WriteLine(string.Join(";", sp.ElapsedMilliseconds, nbPlantsInPool, nbOfGenerationForFinding, plantFound));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RustaFarmer
{
    public class Analyzer
    {
        Random rng = new Random();

        public ExecutionReport Execute(List<Plant> plants, Plant wishPlant, BackgroundWorker bgw)
        {
            var er = new ExecutionReport(plants, wishPlant, bgw);
            ObservableCollection<Plant> breedResults = StartGeneration(plants, new List<Plant>(), wishPlant, 1, er, bgw);
            er.AddResultPlants(breedResults);
            return er;
        }

        public ObservableCollection<Plant> StartGeneration<T>(IEnumerable<T> newPlantsFromLastRound,
            IEnumerable<T> initPlantsFromLastRound, Plant wishPlant, int genNumber, ExecutionReport er, BackgroundWorker bgw)
        {
            if ((newPlantsFromLastRound.Count() == 0 && genNumber != 1) || genNumber > 2)
            {
                ObservableCollection<Plant> lp = new ObservableCollection<Plant>();
                lp = new ObservableCollection<Plant>(((List<Plant>)initPlantsFromLastRound)
                    .OrderByDescending(p => p.GetDistanceFrom(wishPlant))
                    .ThenBy(p => p.GetComplexity()).Take(5).ToList());

                return lp;
            };

            List<Plant> plantPool = new List<Plant>(initPlantsFromLastRound.Count()
                + newPlantsFromLastRound.Count());
            plantPool.AddRange((List<Plant>)initPlantsFromLastRound);
            plantPool.AddRange(((IEnumerable<Plant>)newPlantsFromLastRound).ToList());


            List<Plant> plantsToRemoveFromLastRound = new List<Plant>();
            //Console.WriteLine("Removing plants...");
            var countBeforeRemoving = plantPool.Count();
            foreach (var newPlant in newPlantsFromLastRound)
            {
                plantPool.RemoveAll(p => !p.isValid);
                plantPool.RemoveAll(p => ((Plant)(object)newPlant).IsBetterThan(p));
            }
            ((IEnumerable<Plant>)newPlantsFromLastRound).ToList().RemoveAll(p => !plantPool.Contains(p, new PlantEqualityComparer()));



            List<Plant> newPlantPool = new List<Plant>();
            int cpt = 0;
            foreach (var currentPlant in ((IEnumerable<Plant>)newPlantsFromLastRound).ToList())
            {
                for (var index = 2; index < 5; index++)
                {
                    foreach (IEnumerable<T> permutation in PermuteUtils.PermuteUnique<T>((IEnumerable<T>)plantPool, index))
                    {
                        bgw.ReportProgress(cpt++);

                        List<Plant> permutedPlants = new List<Plant>();
                        permutedPlants.Add(currentPlant);

                        foreach (T i in permutation)
                        {
                            permutedPlants.Add((Plant)(object)i);
                        }
                        Plant newPlant = new Plant(permutedPlants);

                        if (!plantPool.Contains(newPlant, new PlantEqualityComparer())
                            && !newPlantPool.Contains(newPlant, new PlantEqualityComparer()))
                        {
                            newPlantPool.Add(newPlant);
                        }
                    }
                }
            }
            return StartGeneration(newPlantPool, plantPool, wishPlant, ++genNumber, er, bgw);
        }

    }

    public class ExecutionReport
    {
        public Plant wishPlant;
        public BackgroundWorker bgw;
        public List<Plant> initPlants;
        public ObservableCollection<Plant> resultPlants;

        public ExecutionReport(List<Plant> plants, Plant wishPlant, BackgroundWorker bgw)
        {
            this.initPlants = plants;
            this.wishPlant = wishPlant;
            this.bgw = bgw;
        }

        public void AddResultPlants(ObservableCollection<Plant> plants)
        {
            resultPlants = plants;
        }

        public static int Combinations(int n, int k)
        {
            int count = n;
            for (int x = 1; x <= k - 1; x++)
            {
                count = count * (n - x) / x;
            }
            return count / k;
        }
    }
}

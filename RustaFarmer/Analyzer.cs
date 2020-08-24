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

            // Aggregate Plants from Gen - 2 & Gen - 1 in PlantPool for current Gen
            List<Plant> plantPool = new List<Plant>(initPlantsFromLastRound.Count() + newPlantsFromLastRound.Count());
            plantPool.AddRange((List<Plant>)initPlantsFromLastRound);
            plantPool.AddRange(((IEnumerable<Plant>)newPlantsFromLastRound).ToList());

            // Remove non valid plants from first round
            if (genNumber == 1)
            {
                ((List<Plant>)newPlantsFromLastRound).RemoveAll(p => !p.isValid);
            }


            //// Prepare PlantPool for current Gen by removing Invalid & Worst plant
            //foreach (var newPlant in newPlantsFromLastRound)
            //{
            //    plantPool.RemoveAll(p => !p.isValid);
            //    plantPool.RemoveAll(p => ((Plant)(object)newPlant).IsBetterThanOrEqualTo(p));
            //}
            //// Invalid & Worst plant removed are also removed from newPlantsFromLastRound
            //((IEnumerable<Plant>)newPlantsFromLastRound).ToList().RemoveAll(p => !plantPool.Contains(p, new PlantEqualityComparer()));

            // Out condition : If gen > 3 or no more new plants
            if ((newPlantsFromLastRound.Count() == 0 && genNumber != 1) || genNumber > 1)
            {
                ObservableCollection<Plant> lp = new ObservableCollection<Plant>();
                lp = new ObservableCollection<Plant>(((List<Plant>)plantPool)
                    //.OrderByDescending(p => p.GetDistanceFrom(wishPlant))
                    .OrderByDescending(p => p.GetScore())
                    .ThenBy(p => p.GetComplexity()).Take(10).ToList());

                return lp;
            };

            List<Plant> newPlantPool = new List<Plant>();
            int cpt = 0;
            bgw.ReportProgress(ExecutionReport.Combinations(40, 5) * 40);
            foreach (var currentPlant in ((IEnumerable<Plant>)newPlantsFromLastRound).ToList())
            {
                for (var index = 2; index < 5; index++)
                {
                    foreach (IEnumerable<T> permutation in PermuteUtils.PermuteUnique<T>((IEnumerable<T>)plantPool, index))
                    {
                        //bgw.ReportProgress(cpt++);
                        List<Plant> permutedPlants = new List<Plant>();
                        permutedPlants.Add(currentPlant);

                        foreach (T i in permutation)
                        {
                            permutedPlants.Add((Plant)(object)i);
                        }
                        Plant newPlant = new Plant(permutedPlants);
                        if (newPlant.isValid && !plantPool.Exists(p => p.IsBetterThanOrEqualTo(newPlant))
                            && !newPlantPool.Exists(p => p.IsBetterThanOrEqualTo(newPlant)))
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

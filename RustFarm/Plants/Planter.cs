using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust.Farm.RustPlants
{
    public class Planter
    {
        List<PlanterSlot> planterSlots;

        Planter()
        {
            InstanciatePlanterSlots();
        }
        void InstanciatePlanterSlots()
        {
            planterSlots = new List<PlanterSlot>(9);
            for (var index = 0; index < 9; index++)
            {
                planterSlots.Add(new PlanterSlot(this, index + 1));
            }
            foreach (var planterSlot in planterSlots)
            {
                planterSlot.GetAdjacentsPlanterSlots();
            }
        }

        public PlanterSlot GetPlanterSlotAtPosition(int pos)
        {
            return planterSlots.Where(ps => ps.position == pos).ToList().First();
        }

        void Plants(List<Plant> plants)
        {


        }
    }

    public class PlanterSlot
    {
        Planter planter;

        List<PlanterSlot> adjacents;
        public int position;

        public PlanterSlot(Planter planter, int position)
        {
            this.planter = planter;
            this.position = position;
        }

        public void GetAdjacentsPlanterSlots()
        {
            adjacents = new List<PlanterSlot>();
            switch (position)
            {
                case 1:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(2));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(4));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    break;
                case 2:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(1));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(4));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(6));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(3));
                    break;
                case 3:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(2));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(6));
                    break;
                case 4:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(7));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(8));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(2));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(1));
                    break;
                case 5:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(7));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(8));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(9));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(4));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(6));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(1));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(2));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(3));
                    break;
                case 6:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(3));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(2));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(8));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(9));
                    break;
                case 7:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(8));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(4));
                    break;
                case 8:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(7));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(4));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(6));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(9));
                    break;
                case 9:
                    adjacents.Add(planter.GetPlanterSlotAtPosition(8));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(5));
                    adjacents.Add(planter.GetPlanterSlotAtPosition(6));
                    break;
                default:
                    break;
            }
        }
    }
}

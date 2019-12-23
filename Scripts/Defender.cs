using System;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.AI.Planner.DomainLanguage.TraitBased;

#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Actions.Defender;
using AI.Planner.Domains;
#endif

namespace DefenderNameSpace
{
#if PLANNER_DOMAIN_GENERATED
    [Serializable]
    public class Defender : BaseAgent<Defender, DomainObject, StateEntityKey, StateData, StateDataContext, ActionScheduler, NeedHeuristic, TerminationEvaluator, StateManager, ActionKey> //inherting from the base agent class provided by Dlls
    {
        public bool Dead { get; set; }
        public static ComponentType[] NeedFilter;
        public static ComponentType[] InventoryFilter;
        public static ComponentType[] PouchFilter;

        void Awake() //getting components
        {
            NeedFilter = new ComponentType[] { typeof(Need) };
            InventoryFilter = new ComponentType[] { typeof(Inventory) };
            PouchFilter = new ComponentType[] { typeof(Pouch) };

        }

        protected override void Update()
        {
            if (!Dead) //long as agent isn't dead update
                base.Update();
        }
    }

    public struct NeedHeuristic : IHeuristic<StateData> //heuristics basically set your goal and the planner optimizes as it goes
    {
        public float Evaluate(StateData stateData)
        {
            var totalNeedsUrgency = 0L;

            // Resources
            var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.Temp); //new list with intial capacity and memory allocation (from unity entites package)
            foreach (var (_, domainObjectIndex) in stateData.GetDomainObjects(domainObjects, Defender.NeedFilter))
            {
                var needTrait = stateData.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);
                totalNeedsUrgency += needTrait.Urgency;
            }

            float value = 50;

            // Score based on total urgency over all needs (0 -> 300).
            if (totalNeedsUrgency > 50)
                value = 0;
            if (totalNeedsUrgency > 100)
                value = -30;
            if (totalNeedsUrgency > 150)
                value = -50;
            if (totalNeedsUrgency > 200)
                value = -150;

            domainObjects.Clear();
            foreach (var (_, domainObjectIndex) in stateData.GetDomainObjects(domainObjects, Defender.InventoryFilter))
            {
                var inventoryTrait = stateData.GetTraitOnObjectAtIndex<Inventory>(domainObjectIndex);
                value += inventoryTrait.Amount * 10; //value for items in the trait so that the planner includes this in its calculations moving ahead 
            }
            domainObjects.Clear();
            foreach (var (_, domainObjectIndex) in stateData.GetDomainObjects(domainObjects, Defender.PouchFilter))
            {
                var pouchTrait = stateData.GetTraitOnObjectAtIndex<Pouch>(domainObjectIndex);
                value += pouchTrait.Amount * 10;
            }
            domainObjects.Dispose();

            return value;
        }
    }
#endif
}


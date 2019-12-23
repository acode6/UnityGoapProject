#if PLANNER_DOMAIN_GENERATED
using System;
using System.Collections.Generic;
using Unity.Collections;
using AI.Planner.Domains;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Entities;
using UnityEngine;
namespace AI.Planner.Actions.Defender
{
    public struct CustomPreNav : ICustomPrecondition<StateData>
    {

        const int k_agentIndex = 0;
        const int k_toIndex = 1;
        const int k_timeIndex = 2;
        static ComponentType[] harvestFilter = { typeof(HarvestableTrait) };
        public bool CheckCustomPrecondition(StateData originalState, ActionKey action)
        {
            Debug.Log("Checking Precondition");
            var DomainObjectBuffer = originalState.DomainObjects;
            var agentObject = DomainObjectBuffer[action[k_agentIndex]];
            var destinationObject = DomainObjectBuffer[action[k_toIndex]]; //i have to set this


            var locationBuffer = originalState.LocationBuffer; 
            var AgentLocation = locationBuffer[agentObject.LocationIndex];
            var DestinationLocation = locationBuffer[destinationObject.LocationIndex];
            var distance1 = Vector3.Distance(AgentLocation.Position, DestinationLocation.Position);
            float amount = 0;
            for (var obj = 0; obj < DomainObjectBuffer.Length; obj++)
            {
                var domainObject = DomainObjectBuffer[obj];
                if (domainObject.MatchesTraitFilter(harvestFilter))
                {
                    amount = amount + obj;
                  
                    //check all havestables location
                    var check = locationBuffer[domainObject.LocationIndex];

                    //compare their distance to agent
                    var distance = Vector3.Distance(AgentLocation.Position, check.Position);
                    //set 
                    if (distance1 == distance && distance1 < 1)
                    {

                        Debug.Log("Returning true");
                        Debug.Log("These many harvestables " + amount);
                        return true;
                    }
                    
                   
                        
                    
                }
            }
                 Debug.Log("These many harvestables " + amount);

            return false;

          
        }


    }
}
#endif

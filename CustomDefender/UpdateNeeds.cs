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
    public struct UpdateNeeds : ICustomActionEffect<StateData>
    {
        static ComponentType[] needFilter = { typeof(Need) };

        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {

            //Debug.Log("Needs updated");
            // Should only be one time object
            var previousTime = originalState.TimeBuffer[0];
            var newStateTime = newState.TimeBuffer[0];
            var deltaTime = newStateTime.Value - previousTime.Value;

            var needBuffer = newState.NeedBuffer; 
            var objectBuffer = newState.DomainObjects;
            for (var obj = 0; obj < objectBuffer.Length; obj++)
            {
                var domainObject = objectBuffer[obj];
                if (domainObject.MatchesTraitFilter(needFilter))
                {
                    var need = needBuffer[domainObject.NeedIndex];
                    need.Urgency += need.ChangePerSecond * deltaTime;
                    needBuffer[domainObject.NeedIndex] = need;
                }
            }
        }
    }
}
#endif

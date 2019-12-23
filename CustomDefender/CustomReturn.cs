#if PLANNER_DOMAIN_GENERATED
using System;
using AI.Planner.Domains;
using Unity.AI.Planner.DomainLanguage.TraitBased;

namespace AI.Planner.Actions.Defender
{
    public struct CustomReturn : ICustomActionEffect<StateData>, ICustomReward<StateData>
    {
        public void ApplyCustomActionEffectsToState(StateData originalState, ActionKey action, StateData newState)
        {
            new UpdateNeeds().ApplyCustomActionEffectsToState(originalState, action, newState);
        }

        public void SetCustomReward(StateData originalState, ActionKey action, StateData newState, ref float reward)
        {


            reward = 10;
        }
    }
}
#endif

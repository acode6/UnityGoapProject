#if PLANNER_ACTIONS_GENERATED
using Unity.AI.Planner.Agent;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using UnityEngine;
using UnityEngine.AI;
using DefenderNameSpace;
using Time = UnityEngine.Time;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;

public class ReturnToNest : DefenderAction
{
    static readonly int k_Nest = Animator.StringToHash("ReturnToNest");
    static readonly int k_Forward = Animator.StringToHash("Forward");
    static readonly int k_Turn = Animator.StringToHash("Turn");

    MotionController m_MotionController;
    NavMeshAgent m_NavMeshAgent;
    Transform m_DefenderTransform;
    Vector3 m_TargetPosition;
    bool m_Arrived;
    float m_TotalDistanceTravelled;
    float m_TotalTime;
    float m_PredictedDeltaTime;
    const float k_TurnStrength = 1.0f;

    void SetAnimationParams(bool atDestination = false) //here you set animation parameters
    {

        // Current position/orientation
        var currentDirection = m_DefenderTransform.forward;
        var currentPosition = m_DefenderTransform.position;

        // Turn
        var targetDirection = m_NavMeshAgent.nextPosition - currentPosition;
        var turnVal = Vector3.SignedAngle(currentDirection, targetDirection, Vector3.up) / 90.0f;
        var realTurnStrength = atDestination ? 0.0f : k_TurnStrength;
        m_Animator.SetFloat(k_Turn, turnVal * realTurnStrength);
        //Debug.Log("Turn Value: " + turnVal);

        // Forward
        var minForwardVelocity = atDestination ? 0.0f : 0.2f;

        var forwardVal = Mathf.Clamp((m_NavMeshAgent.nextPosition - currentPosition).magnitude, minForwardVelocity, 1f);
        m_Animator.SetFloat(k_Forward, forwardVal);
        //  Debug.Log(" Forward Value: " + forwardVal);

    }

    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
        base.BeginExecution(state, action, actor);
        // Trigger beginning of walk animation.
        m_Animator = actor.GetComponentInParent<Animator>();
        m_Animator.SetBool(k_Nest, true);

        m_TargetPosition = state.GetTraitOnObjectAtIndex<Location>(action[1]).Position; //getting the location of Nest
        m_DefenderTransform = actor.transform; //your transform

        // Grab nav mesh
        m_NavMeshAgent = actor.GetComponentInParent<NavMeshAgent>();

        // Motion Controller
        m_MotionController = actor.GetComponentInParent<MotionController>();
        m_MotionController.TargetPosition = m_TargetPosition;
        m_MotionController.StartMoving();
        SetAnimationParams();
        m_Arrived = false;

        var startPosition = state.GetTraitOnObjectAtIndex<Location>(action[0]).Position;
        var distance = Vector3.Distance(startPosition, m_TargetPosition);
        m_PredictedDeltaTime = Mathf.FloorToInt(distance / 0.47f + 1f);
    }
    public override void ContinueExecution(StateData state, ActionKey action, Defender actor)
    {
        base.ContinueExecution(state, action, actor);

        // Delay the execution of this animation until we've reached the Navigation state in the animator.
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Returning"))
        {
            var position = m_DefenderTransform.position;
            m_MotionController.TargetOrientation = m_NavMeshAgent.nextPosition - position;
            SetAnimationParams();

            // Check for arrival
            m_Arrived = Vector3.Distance(position, m_TargetPosition) <= 1;
            float debugDist = Vector3.Distance(position, m_TargetPosition);
             //Debug.Log("Arived at Nest: " + m_Arrived);
             // Debug.Log("Distance to Nest:" + debugDist);
        }
    }

    public override void EndExecution(StateData state, ActionKey action, Defender actor)
    {
        base.EndExecution(state, action, actor);

        // Set target orientation based on location orientation
        var forward = state.GetTraitOnObjectAtIndex<Location>(action[1]).Forward;
        m_MotionController.StopMoving(forward);

        // Effects
        // Update Defender position
        var agentDomainObjectIndex = action[0];
        var ememyDestinationLocation = state.GetTraitOnObjectAtIndex<Location>(action[1]);

        var loc = state.GetTraitOnObjectAtIndex<Location>(agentDomainObjectIndex);
        loc.Position = ememyDestinationLocation.Position;
        state.SetTraitOnObjectAtIndex(loc, agentDomainObjectIndex);

        // Trigger ending of walk animation.
        m_Animator.SetBool(k_Nest, false);
        SetAnimationParams(true);
    }

    public override OperationalActionStatus Status(StateData state, ActionKey action, Defender actor)
    {


        return m_Arrived ? OperationalActionStatus.Completed : OperationalActionStatus.InProgress;

    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_TargetPosition, 0.33f);



    }

}

#endif
#if PLANNER_ACTIONS_GENERATED
using Unity.AI.Planner.Agent;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using UnityEngine;
using UnityEngine.AI;
using DefenderNameSpace;
using Time = UnityEngine.Time;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
using Unity.Entities;
using Unity.Collections;
public class RandomNavigateAction : DefenderAction
{
    static readonly int k_Walk = Animator.StringToHash("Explore");
    static readonly int k_Forward = Animator.StringToHash("Forward");
    static readonly int k_Turn = Animator.StringToHash("Turn");

    MotionController m_MotionController;
    NavMeshAgent m_NavMeshAgent;
    Transform m_DefenderTransform;
    Vector3 m_TargetPosition;
    RandomPoint randl;
    bool m_Arrived;
    float m_TotalDistanceTravelled;
    float m_TotalTime;
    float m_PredictedDeltaTime;
    const float k_TurnStrength = 1.0f;

    //Patrol Point Stuff
    private float patrolRadius = 4;
    private float patrolRange = 30;
    private float patrolPointFrequency = 10;
    private float timer = 1;
    private NavMeshHit navHit;
    
    private Vector3 randomPoint; //random point that you get
    private Vector3 targetSpace = Vector3.zero; //how far the target space is to you 
    private Vector3 patrolTarget = Vector3.zero; //the point that you're going to
    public Vector3 target;
    public bool pointDisable = false;
    Defender defender;
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
      //  Debug.Log("Turn Value: " + turnVal);

        // Forward
        var minForwardVelocity = atDestination ? 0.0f : 0.2f;

        var forwardVal = Mathf.Clamp((m_NavMeshAgent.nextPosition - currentPosition).magnitude, minForwardVelocity, 1f);
        m_Animator.SetFloat(k_Forward, forwardVal);
        //  Debug.Log(" Forward Value: " + forwardVal);
       
        if(forwardVal < 0.21)
        {
            m_Arrived = true;
             
        }


    }

    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
       // Debug.Log("begin Random Navigation");
        base.BeginExecution(state, action, actor);
        // Trigger beginning of walk animation.
        m_Animator = actor.GetComponentInParent<Animator>();
        m_Animator.SetBool(k_Walk, true);

       // defender = actor;
        
      
       // m_TargetPosition = state.GetTraitOnObjectAtIndex<Location>(action[1]).Position;
        m_DefenderTransform = actor.transform; //your transform

        // Grab nav mesh
        m_NavMeshAgent = actor.GetComponentInParent<NavMeshAgent>();
        
        // Motion Controller
        m_MotionController = actor.GetComponentInParent<MotionController>();
        m_TargetPosition = state.GetTraitOnObjectAtIndex<Location>(action[1]).Position; //getting the location of Nest
        m_MotionController.TargetPosition = m_TargetPosition ; //at start set it to your location
       if(m_TargetPosition == Vector3.zero)
        {
            Debug.Log("Should be done");
            actor.CompleteAction();
        }
        m_MotionController.StartMoving();
        SetAnimationParams();
        m_Arrived = false;

        var startPosition = state.GetTraitOnObjectAtIndex<Location>(action[0]).Position;
        var distance = Vector3.Distance(startPosition, m_TargetPosition);
        m_PredictedDeltaTime = Mathf.FloorToInt(distance / 0.47f + 1f);
        
    }
    public override void ContinueExecution(StateData state, ActionKey action, Defender actor)
    {
        updatePatrolTimer();
        base.ContinueExecution(state, action, actor);
        Debug.DrawLine(m_DefenderTransform.position, m_MotionController.TargetPosition);
        Debug.DrawLine(m_DefenderTransform.position, targetSpace);
        targetSpace = m_DefenderTransform.position + m_DefenderTransform.forward * patrolRange;
        
        // Delay the execution of this animation until we've reached the Navigation state in the animator.
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explore"))
        {
            Debug.Log("continuing Random Navigation");
            var position = m_DefenderTransform.position;
            m_MotionController.TargetOrientation = m_NavMeshAgent.nextPosition - position;
            SetAnimationParams();

            // Check for arrival
           // m_Arrived = Vector3.Distance(position, m_TargetPosition) <= 1;
          //  Vector3 check = Vector3.Distance(position, m_TargetPosition);
            float debugDist = Vector3.Distance(position, m_TargetPosition);
             Debug.Log("Arived : " + m_Arrived);
             //Debug.Log("Distance :" + debugDist);
           
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
        m_Animator.SetBool(k_Walk, false);
        SetAnimationParams(true);
        Debug.Log("stop Random Navigation");
    }
    //generate new random point if timer is reached
    private void updatePatrolTimer()
    {
        timer += Time.deltaTime;


        Vector3 check;

        if (timer > patrolPointFrequency)
        {
            randomPoint = targetSpace + Random.insideUnitSphere * patrolRadius;
            if (checkPoint(randomPoint, patrolRange, out check))
            {
                randomPoint = check;

                check.y = m_DefenderTransform.position.y;
                m_MotionController.TargetPosition = check;



            }
            else
            {
                randomPoint = m_DefenderTransform.position + Random.insideUnitSphere * 150;
                if (checkPoint(randomPoint, patrolRange, out check))
                {

                    randomPoint = check;

                    check.y = m_DefenderTransform.position.y;
                    m_MotionController.TargetPosition = check;

                }
                else
                {

                    return;
                }
            }



            timer = 0;

        }

        //Gizmos.color = Color.black;
        //  Gizmos.DrawWireSphere(pointToCheck, 0.33f);
    }

    private bool checkPoint(Vector3 point, float range, out Vector3 result)
    {


        if (NavMesh.SamplePosition(point, out navHit, 1, NavMesh.AllAreas))
        {

            result = navHit.position;
            // Debug.Log("RESULT OF CHECK POINT SUCCESS:" + result);
            return true;
        }

        result = point;
        //Debug.Log("RESULT OF CHECK POINT FAIL:" + result);
        return false;
    }
    public override OperationalActionStatus Status(StateData state, ActionKey action, Defender actor)
    {


        return m_Arrived ? OperationalActionStatus.Completed : OperationalActionStatus.InProgress;

    }

   

}

#endif
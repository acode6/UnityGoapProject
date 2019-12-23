using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RandomPoint : MonoBehaviour
 {
        [Header("Adventure Settings")]
        [Tooltip("Set the radius of the patrol target space in front of character")]
        [SerializeField]
        private float patrolRadius = 4;


        [Tooltip("Distance between enemy and patrol target space")]
        [SerializeField]
        private float patrolRange = 4;


        [Tooltip("Set the rate at which you get a new patrol point")]
        [SerializeField]
        private float patrolPointFrequency = 1;
        public bool enableGizmos = false;
        private float timer = 0;
        private NavMeshHit navHit;
        //Points to move have
        private Vector3 randomPoint; //random point that you get
        private Vector3 targetSpace = Vector3.zero; //how far the target space is to you 
        private Vector3 patrolTarget = Vector3.zero; //the point that you're going to
        public Vector3 target;

        private Rigidbody myRigid;
        //disable or enable
        public bool patrolDisable = false;
    Animator anim;
  
    void Start()
        {
        myRigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        }
 

        void Update()
        {

            targetSpace = transform.position + transform.forward * patrolRange;
            updatePatrolTimer();

        }
        // Gizmos

        void OnDrawGizmos()
        {
         

            if (enableGizmos == true)
            {
                Gizmos.color = Color.white;
                // Gizmos.DrawLine(transform.position, transform.position + patrolTarget);

                // if (blackBox.my != null)
                /* {
                     Gizmos.color = Color.blue;
                     Gizmos.DrawLine(transform.position, transform.position + blackBox.Velocity);
                 }*/

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * patrolRange);
                Gizmos.DrawWireSphere(transform.position + transform.forward * patrolRange, patrolRadius);
                Gizmos.color = Color.magenta;
                 Gizmos.DrawWireSphere(randomPoint, .33f);
        }
        }

        //generate new random point if timer is reached
        private void updatePatrolTimer()
        {
            timer += Time.deltaTime;


            Vector3 check;

            if (timer > patrolPointFrequency)
            {
                randomPoint = targetSpace + Random.insideUnitSphere * patrolRadius;
                //Debug.Log("RANDOM POINT GENERATED" + randomPoint);
                if (checkPoint(randomPoint, patrolRange, out check))
                {
                    randomPoint = check;
                    target = randomPoint;
                  
                    //  patrolTarget.y = transform.position.y;
                    //check += targetSpace; //keep the target space in front of me
                  
                    // Debug.Log("ACTUAL RANDOM POINT" + target);
                    //  Debug.Log("VALID RANDOM POINT IS:" + randomPoint);
                    // Debug.Log("VALID BLACKBOX TARGET : " + blackBox.Target);

                }
                else
                {
                    randomPoint = transform.position + Random.insideUnitSphere * 50;
                    if (checkPoint(randomPoint, patrolRange, out check))
                    {

                        randomPoint = check;

                    target = randomPoint;


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
    }


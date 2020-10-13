using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class OpponentController : MonoBehaviour
{
    [SerializeField] float impactTime = 3f;
    [SerializeField] float impactForce = 2f;
    [SerializeField] float rotateSpeedOnThePlatform = 30f;
    [SerializeField] float checkAroundRadius = 1.5f;
    private Animator animator;
    private Rigidbody myBody;
    private NavMeshAgent agent;
    private bool impacted = false;
    private bool finished = false;
    private bool inRespawnCooldown = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private RigidbodyConstraints originalConstraints;
    private Transform finishPoint;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
    }

    private void Start() {
        originalConstraints = myBody.constraints;
        startPosition = transform.position;
        startRotation = transform.rotation;
        finishPoint = GameObject.FindGameObjectWithTag("FinishPoint").transform;
    }

    private void Update()
    {
        if (finished) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkAroundRadius);
        foreach (Collider collider in hitColliders)
        {
            //If half donut exist in given radius around, stop it. 
            if (collider.tag == "Half Donut") {
                animator.SetBool("isRunning", false);
                agent.isStopped = true;
                return;
            } else
                agent.isStopped = false;
        }

        RaycastHit hitDown;
        if (Physics.Raycast(transform.position, -transform.up, out hitDown, Mathf.Infinity)) {
            //Rotate with rotating platform 
            if (hitDown.transform.tag == "Rotating Platform") {
                int platformRotateDir = hitDown.transform.gameObject.GetComponent<RotatingPlatformController>().speed < 0 ? 1 : -1;
                transform.RotateAround(hitDown.transform.position, platformRotateDir * Vector3.forward, rotateSpeedOnThePlatform * Time.deltaTime);
            }
        }

        //If Impact or Getting Up animations active then set canMove to false
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Impact")) {
                agent.isStopped = true;
        }
        else {
            Move();
        }
    }

    private void OnTriggerEnter (Collider other) {
        // if (other.transform.tag == "DropChecker" && !inRespawnCooldown) {
        //     StartCoroutine(Respawn(0));
        //     inRespawnCooldown = true;
        // }

        // Reached to finish point.
        if (other.transform.tag == "FinishPoint" && !inRespawnCooldown) {
            UIController.finishedCount++;
            animator.SetBool("isRunning", false);
            agent.isStopped = true;
            finished = true;
        }
    }

    private void Move() {
        animator.SetBool("isRunning", true);
        agent.SetDestination(finishPoint.position);
    }

    private IEnumerator Respawn (float respawnTime) {
        agent.ResetPath();
        yield return new WaitForSeconds(respawnTime);

        animator.Rebind();
        agent.isStopped = false;
        agent.Warp(startPosition);
        myBody.velocity = Vector3.zero;
        myBody.constraints = originalConstraints;
        transform.rotation = startRotation;
        inRespawnCooldown = false;
    }

    private IEnumerator ImpactEffect(Vector3 dir) {
        impacted = true;
        animator.SetBool("impact", impacted);
        dir = -dir.normalized;
        myBody.AddForce(dir * impactForce, ForceMode.Impulse);
        myBody.constraints = RigidbodyConstraints.FreezeRotation;

        if (!inRespawnCooldown) {
            StartCoroutine(Respawn(impactTime));
            inRespawnCooldown = true;
        }
        yield return new WaitForSeconds(impactTime);
        impacted = false;
    }

    private void OnCollisionEnter(Collision other) {
       //Get any moving obstacles impact effect
       if ((other.transform.tag == "MovingObstacle" || other.transform.tag == "Half Donut") && !impacted) {
            //If Half Donut is static state then don't apply the impact effect
            if (other.transform.tag == "Half Donut") {
                Rigidbody halfDonutsBody = other.gameObject.GetComponent<Rigidbody>();
                if (Mathf.Approximately(0, halfDonutsBody.velocity.x))
                    return;
            }

            Vector3 dir = other.transform.position - transform.position;
            StartCoroutine(ImpactEffect(dir));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, checkAroundRadius);
    }

}

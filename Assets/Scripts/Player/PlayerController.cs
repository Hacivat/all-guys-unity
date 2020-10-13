using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 5f;
    [SerializeField] float impactTime = 3f;
    [SerializeField] float impactForce = 2f;
    [SerializeField] float rotateSpeedOnThePlatform = 30f;
    [SerializeField] GameObject gameManager = null;
    private Rigidbody myBody;
    private RigidbodyConstraints originalConstraints;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Animator animator;
    private bool isMoving = false;
    public bool canPaint = false;
    public bool inPaintingArea = false;
    private bool impacted = false;

    private void Awake() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        animator = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
        originalConstraints = myBody.constraints;
    }
    private void Update () {
        isMoving = InputDetected();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity)) {
            //Rotate with rotating platform 
            if (hit.transform.tag == "Rotating Platform") {
                int platformRotateDir = hit.transform.gameObject.GetComponent<RotatingPlatformController>().speed < 0 ? -1 : 1;
                transform.RotateAround(hit.transform.position, platformRotateDir * Vector3.forward, rotateSpeedOnThePlatform * Time.deltaTime);
            }

            // falling effect
            if (myBody.velocity.y <= -4f) {
                animator.SetBool("isFalling", true);
                isMoving = false;
            }
            else
                animator.SetBool("isFalling", false);
        }

        //If Impact or Getting Up animations active then set canMove to false
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Impact")) {
                isMoving = false;
        }
        else {
            if (InputDetected()){
                isMoving = true;
                FollowTheInput();
            }
        }

        animator.SetBool("isRunning", isMoving);
    }

    private void OnCollisionEnter(Collision other) {
        if ((other.transform.tag == "MovingObstacle" || other.transform.tag == "Half Donut") && !impacted) {
            //If Half Donut not animating then don't apply impact effect
            if (other.transform.tag == "Half Donut") {
                Rigidbody halfDonutsBody = other.gameObject.GetComponent<Rigidbody>();
                if (Mathf.Approximately(0, halfDonutsBody.velocity.x))
                    return;
            }

            Vector3 dir = other.transform.position - transform.position;
            dir = -dir.normalized;
            StartCoroutine(ImpactEffect(dir));
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.tag == "Painting Area") {
            StartCoroutine(WaitForPainting());
            UIController.finishedCount++;
        }

        if (other.transform.tag == "DropChecker") {
            StartCoroutine(Respawn(0));
        }
    }

    private void FollowTheInput() {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        Vector3 inputPosition = Input.mousePosition;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 offset = new Vector2(inputPosition.x - screenPoint.x, inputPosition.y - screenPoint.y);
        float rotationAngle = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
    }

    private IEnumerator WaitForPainting() {
        inPaintingArea = true;
        gameManager.GetComponent<UIController>().paintTheWallText.enabled = true;

        yield return new WaitForSeconds(1.5f);
        gameManager.GetComponent<UIController>().paintTheWallText.enabled = false;
        canPaint = true;
    }

    private IEnumerator ImpactEffect(Vector3 dir) {
        impacted = true;
        animator.SetBool("impact", impacted);
        myBody.AddForce(dir * impactForce, ForceMode.Impulse);
        myBody.constraints = RigidbodyConstraints.FreezeRotation;

        yield return new WaitForSeconds(impactTime);

        impacted = false;
        myBody.constraints = originalConstraints;
        animator.SetBool("impact", impacted);
    }

    private bool InputDetected() {
        if(inPaintingArea) return false;
        if(Input.GetMouseButton(0)) return true;

        return false;
    }

    private IEnumerator Respawn(float respawnTime) {
        yield return new WaitForSeconds(respawnTime);

        animator.Rebind();
        myBody.velocity = Vector3.zero;
        myBody.constraints = originalConstraints;
        transform.rotation = startRotation;
        transform.position = startPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HalfDonutStickController : MonoBehaviour
{
    [Range(2f, 10f)]
    [SerializeField] float animationInterval = 3;
    [SerializeField] float animationStartTime = 1;
    // [SerializeField] GameObject animateThat = null;
    private Animator animator;
    private float animationLength;
    private Vector3 startPosition;
    private Rigidbody myBody;
    private AudioSource audioData;

    private void Awake() {
        audioData = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        myBody = GetComponent<Rigidbody>();
        myBody.isKinematic = true;
    }

    private void Start() {
        InvokeRepeating("Animation", animationStartTime, animationInterval);
        startPosition = transform.position;
    }

    private void Animation() {
        StartCoroutine(ChangeState());
    }

    private IEnumerator ChangeState() {
        transform.position = startPosition;
        animator.SetBool("halfDonutActive", true);
        myBody.isKinematic = false;

        yield return new WaitForSeconds(1.5f);
        animator.SetBool("halfDonutActive", false);
        myBody.isKinematic = true;
    }

    private void OnCollisionEnter(Collision other) {
        if (Mathf.Approximately(0, myBody.velocity.x))
            return;

        if (!audioData.isPlaying)
            audioData.Play();
    }
}

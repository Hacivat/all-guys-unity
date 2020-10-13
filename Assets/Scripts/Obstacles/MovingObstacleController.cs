using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleController : MonoBehaviour
{
    [SerializeField] float speed = .2f;
    [SerializeField] float startingSecond = 0;
    private Vector3 direction;
    private Rigidbody myBody;
    private AudioSource audioData;

    private void Awake() {
        audioData = GetComponent<AudioSource>();
        myBody = GetComponent<Rigidbody>();
    }
    private void Start() {
        direction = Vector3.left;
    }

    private void Update() {
        Move();
    }

    private void Move() {
        if (Time.time > startingSecond) {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Turning Point") {
            direction = -direction;
        }
    }

     private void OnCollisionEnter(Collision other) {
        if (!audioData.isPlaying)
            audioData.Play();
    }
}

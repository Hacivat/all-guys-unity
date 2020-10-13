using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleObstacleController : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float width = 2f;
    [SerializeField] float height = 2f;
    private float timeCount = 0;
    private Vector3 startPosition;
    private Rigidbody myBody;
    private AudioSource audioData;

    private void Awake() {
        audioData = GetComponent<AudioSource>();
        myBody = GetComponent<Rigidbody>();
    }

    private void Start () {
        startPosition = transform.position;
    }
    private void Update () {
        timeCount += Time.deltaTime * speed;

        float x = Mathf.Cos(timeCount) * width;
        float y = startPosition.y;
        float z = startPosition.z + Mathf.Sin(timeCount) * height;

        transform.position = new Vector3(x, y, z);
    }

    private void OnCollisionEnter(Collision other) {
        if (!audioData.isPlaying)
            audioData.Play();
    }
}

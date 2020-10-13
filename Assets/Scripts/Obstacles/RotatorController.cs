using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorController : MonoBehaviour
{
    public float speed = 50;
    private void Update() {
        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
    }
}

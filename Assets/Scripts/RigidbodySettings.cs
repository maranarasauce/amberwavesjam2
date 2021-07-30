using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySettings : MonoBehaviour
{
    [SerializeField] Transform comOverride;
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.centerOfMass = comOverride.position;
    }
}

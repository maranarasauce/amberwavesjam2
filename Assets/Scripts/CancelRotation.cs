using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelRotation : MonoBehaviour
{
    [SerializeField] bool X;
    [SerializeField] bool Y;
    [SerializeField] bool Z;

    private void FixedUpdate()
    {
        Vector3 a = transform.rotation.eulerAngles;
        if (X)
            a.x = 0;
        if (Y)
            a.y = 0;
        if (Z)
            a.z = 0;
        transform.rotation = Quaternion.Euler(a);
    }
}

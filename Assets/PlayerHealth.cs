using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : DamageableObject
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            health -= 5;
        }
    }
}

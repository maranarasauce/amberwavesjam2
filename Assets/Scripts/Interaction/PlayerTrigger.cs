using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    static GameObject player;
    public UnityEvent PlayerEnter;
    public UnityEvent PlayerExit;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    bool playerIn;
    private void OnTriggerEnter(Collider other)
    {
        if (!playerIn && other.transform.root == player.transform)
        {
            playerIn = true;
            PlayerEnter?.Invoke();
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerIn && other.transform.root == player.transform)
        {
            playerIn = false;
            PlayerExit?.Invoke();
        }
            
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    public static MusicLooper instance;
    void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}

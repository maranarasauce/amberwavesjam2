using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    public Action executedAction;

    public void ExecuteAction()
    {
        executedAction?.Invoke();
    }
}

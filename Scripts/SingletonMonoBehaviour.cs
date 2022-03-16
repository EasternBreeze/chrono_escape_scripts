using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError($"{t} is not found");
                }
            }

            return instance;
        }
    }

    protected abstract void Awake();

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"Instance :{instance}");
            return true;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log($"Destroy :{instance}");
            return false;
        }
    }
}

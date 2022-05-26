using System;
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

    private void Awake()
    {
        if (CheckInstance())
        {
            Init();
        }
    }

    protected abstract void Init();

    private bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this);
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

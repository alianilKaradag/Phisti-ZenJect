using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogError($"Cannot find the instance of {typeof(T).FullName}");
                    return null;
                }
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    protected void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.LogError($"Duplicate instance of {typeof(T).FullName}");
        }
    }

    private void OnApplicationQuit()
    {
        instance = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BasePanel<T>: MonoBehaviour where T:BasePanel<T>
{
    private static T instance;
    public static T Instance => instance;

    protected virtual void Awake()
    {
        if(instance == null) instance = this as T;
    }
    public virtual void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    public virtual void HidePanel()
    {
        gameObject.SetActive(false);
    }
}

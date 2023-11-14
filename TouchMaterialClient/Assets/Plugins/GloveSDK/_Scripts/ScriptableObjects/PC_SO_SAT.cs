using UnityEngine;

public abstract class PC_SO_SAT<T> : ScriptableObject
{

    private System.Action<T> listeners;


    public void Register(System.Action<T> notifyMe)
    {
        listeners += notifyMe;
    }

    public void Unregister(System.Action<T> unnotifyMe)
    {
        listeners -= unnotifyMe;
    }

    public void TriggerEvent(T param)
    {
        listeners(param);
    }


}

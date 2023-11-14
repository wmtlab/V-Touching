using UnityEngine;

[CreateAssetMenu(fileName = "SO_TriggerEvent", menuName = "SO/Events/Simple")]
public class SO_TriggerEvent : ScriptableObject
{
    private System.Action listeners;

    public void Register(System.Action notifyMe)
    {
        listeners += notifyMe;
    }

    public void Unregister(System.Action unnotifyMe)
    {
        listeners -= unnotifyMe;
    }

    public void TriggerEvent()
    {
        listeners();
    }


}

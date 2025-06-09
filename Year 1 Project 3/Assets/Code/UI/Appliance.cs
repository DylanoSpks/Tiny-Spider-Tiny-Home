using UnityEngine;

public class Appliance : MonoBehaviour
{
    public bool IsOn { get; private set; } = true;
    public delegate void StateChanged(Appliance appliance, bool isOn);
    public event StateChanged OnStateChanged;

    public void Toggle()
    {
        IsOn = !IsOn;
        OnStateChanged?.Invoke(this, IsOn);
    }
}
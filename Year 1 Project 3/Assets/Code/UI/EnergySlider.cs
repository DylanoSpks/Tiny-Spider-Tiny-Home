using System;
using UnityEngine;
using UnityEngine.UI;

public class EnergySlider : MonoBehaviour
{
    private Slider _boltSlider;

    private void Start()
    {
        _boltSlider = GetComponent<Slider>();
    }

    public void SetMaxBolt(int maxBolt)
    {
        _boltSlider.maxValue = maxBolt;
        _boltSlider.value = maxBolt;
    }
    
    
    public void SetBolt(int bolt)
    {
        _boltSlider.value = bolt;
    }
}

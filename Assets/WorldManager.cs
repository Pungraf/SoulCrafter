using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Slider worldTimeSlider;

    private float fixedDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        worldTimeSlider.onValueChanged.AddListener(delegate { WorldTimeSpeedChange(); });
        this.fixedDeltaTime = Time.fixedDeltaTime;


    }

    // Update is called once per frame
    public void WorldTimeSpeedChange()
    {
        Time.timeScale = worldTimeSlider.value;

        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}

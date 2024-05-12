using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [SerializeField] private Slider worldTimeSlider;

    private float fixedDeltaTime;


    public System.Random rand = new System.Random();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one WorldManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

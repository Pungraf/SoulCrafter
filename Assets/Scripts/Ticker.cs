using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public static Ticker Instance { get; private set; }

    public static event EventHandler Tick_01;
    private float _tick_01Counter = 0.1f;

    public static event EventHandler Tick_05;
    private float _tick_05Counter = 0.5f;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Ticker! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        _tick_01Counter -= Time.deltaTime;
        _tick_05Counter -= Time.deltaTime;

        if (_tick_01Counter < 0)
        {
            Tick_01?.Invoke(this, EventArgs.Empty);
            _tick_01Counter = 0.1f;
        }
        if (_tick_05Counter < 0)
        {
            Tick_05?.Invoke(this, EventArgs.Empty);
            _tick_05Counter = 0.5f;
        }
    }



}

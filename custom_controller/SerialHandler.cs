using System;
using System.Globalization;
using System.IO.Ports;
using Unity.VisualScripting;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM1";
    [SerializeField] private int baudrate = 115200;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private PlayerController controller;
    [SerializeField] private float maxAngle;
    [SerializeField] private float walkAngleThreshold;
    [SerializeField] private float sprintAngleThreshold;
    [SerializeField] private float jumpSpeedThreshold;
    [SerializeField] private float jumpDelay;

    private bool _isDashPressed = false;
    private float _prevAcc = -1f;
    private float _allowNextJumpDelay = 0f;
    private float _zCorrection = 0f;
    private float _zAcc = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
        Begin();
        DamageTaken(UnitHealth.currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;

        // Trim leading and trailing whitespaces, makes it easier to handle different line endings.
        // Arduino uses \r\n by default with `.println()`.
        var message = _serial.ReadLine().Trim();
        
        if(string.IsNullOrEmpty(message)) return;
        
        string[] values = message.Split(" ");

        if (values.Length != 4)
        {
            Debug.Log(message);
            return;
        }

        float volume = int.Parse(values[0]) / 255f;
        bool dash = int.Parse(values[1]) == 1;

        float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var rawAcc);
        float.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var angle);
        //float acc = Mathf.Abs(rawAcc);
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

        if (_prevAcc < 0f)
            _prevAcc = rawAcc;

        Vector2 movement = new Vector2();
        if (Mathf.Abs(angle) >= walkAngleThreshold)
            movement.x = angle < 0 ? -1 : 1;

        if (Math.Abs(volume - audioSources[0].volume) != 0)
        {
            foreach (var audioSource in audioSources)
                audioSource.volume = volume;
        }
        
        //bool jump = Mathf.Abs(acc - _prevAcc) >= jumpSpeedThreshold && _allowNextJumpDelay <= Time.time;
        bool jump = _prevAcc > 0f && _prevAcc >= jumpSpeedThreshold && rawAcc < 0f && _allowNextJumpDelay <= Time.time;

        if (jump)
            _allowNextJumpDelay = Time.time + jumpDelay;
        
        controller.Move(movement, jump, Mathf.Abs(angle) >= sprintAngleThreshold, !_isDashPressed && dash);
        _isDashPressed = dash;
        _prevAcc = rawAcc;
    }

    public void DamageTaken(int hp)
    {
        _serial.WriteLine(hp.ToString());
    }

    public void Win()
    {
        _serial.WriteLine("W");
    }

    public void Begin()
    {
        _serial.WriteLine("S");
    }
    
    private void OnDestroy()
    {
        _serial.Close();
    }
}
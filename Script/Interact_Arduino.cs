using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

[System.Serializable]
public class AnalogPinValues
{
    public int a;
    public int b;
    public int c;
    public int d;
    public int e;
    public int f;
}

public class Interact_Arduino : MonoBehaviour
{
    static public SerialPort HapticDevicePort = new SerialPort("\\\\.\\COM99", 9600);
    private string preError = "\\\\.\\";
    public string HapticPort;
    private bool onceConnectedMessage = true;
    private int haha = 0;
    static public Interact_Arduino Instance;
    static public AnalogPinValues analogValues = new AnalogPinValues();
    static public List<string> analogBuffer = new List<string>();
    static public string analogbuffer = null;
    public List<string> powerBuffer = new List<string>();
    static private Coroutine lastRoutine = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        onceConnectedMessage = true;
        AnalogPinValues analogValues = new AnalogPinValues();
        if (HapticDevicePort.IsOpen)
        {
            StopAllFeedBack();
        }
        else
        {
            HapticDevicePort = new SerialPort(preError + HapticPort, 9600);

            HapticDevicePort.Open();
            Debug.Log(HapticDevicePort.ReadLine());
            Debug.Log(HapticDevicePort.ReadLine());

            lastRoutine = StartCoroutine(GetAnalogValues(.05f));
            StartCoroutine(HandleValue(.1f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!HapticDevicePort.IsOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            haha++;
            Debug.Log("" + haha + HapticDevicePort.PortName);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log(analogValues.a);
            Debug.Log(analogValues.b);
            Debug.Log(analogValues.c);
            Debug.Log(analogValues.d);
            Debug.Log(analogValues.e);
            Debug.Log(analogValues.f);
        }
        if (onceConnectedMessage)
        {
            onceConnectedMessage = false;
            Debug.Log("Haptic Port is connectd!!");
        }
    }

    private void FixedUpdate()
    {
        if (!HapticDevicePort.IsOpen)
        {
            return;
        }

    }
    private void OnApplicationQuit()
    {
        if (!HapticDevicePort.IsOpen)
        {
            return;
        }
        StopAllFeedBack();
        HapticDevicePort.Close();
    }

    static public void IntervalOutput(string output)
    {
        HapticDevicePort.Write(output);
    }

    private void InvokeOutput()
    {
        if (powerBuffer.Count != 0)
        {
            HapticDevicePort.Write(powerBuffer[powerBuffer.Count - 1]);
            powerBuffer.Clear();
        }
        lastRoutine = StartCoroutine(GetAnalogValues(.4f));

    }

    public void StopAllFeedBack()
    {
        HapticDevicePort.Write("aqbqcqd");
    }
    private IEnumerator GetAnalogValues(float waitTime)
    {
        while (true)
        {
            HapticDevicePort.Write("n");
            HapticDevicePort.BaseStream.Flush();

            yield return new WaitForSeconds(waitTime);
            analogbuffer = HapticDevicePort.ReadLine();
            yield return new WaitForSeconds(.01f);
            if (powerBuffer.Count != 0)
            {
                HapticDevicePort.Write(powerBuffer[powerBuffer.Count - 1]);
                HapticDevicePort.BaseStream.Flush();
                powerBuffer.Clear();
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    private IEnumerator HandleValue(float waitTime)
    {
        while (true)
        {
          
            if (HapticDevicePort.IsOpen && analogbuffer != null)
            {
                string tmp = analogbuffer;
                string[] tmp_array = tmp.Split(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'q' });
                if (tmp_array.Length == 13)
                {
                    analogValues.a = int.Parse(tmp_array[1]);
                    analogValues.b = int.Parse(tmp_array[3]);
                    analogValues.c = int.Parse(tmp_array[5]);
                    analogValues.d = int.Parse(tmp_array[7]);
                    analogValues.e = int.Parse(tmp_array[9]);
                    analogValues.f = int.Parse(tmp_array[11]);
                }
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}

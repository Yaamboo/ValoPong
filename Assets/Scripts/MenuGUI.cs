using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {


    public GUIStyle titleStyle;

    public string lightServerAddress;
    public string lightServerPort;
    public bool sendDataToServer;


    void Start()
    {
        lightServerAddress = PlayerPrefs.GetString("lightServerAddress", "melmacian.net");
        lightServerPort = PlayerPrefs.GetString("lightServerPort", "9909");
        int sendData = PlayerPrefs.GetInt("sendData", 1);
        if (sendData == 0)
            sendDataToServer = false;
        else
            sendDataToServer = true;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;
        int titH = 200;
        int margin = 10;

        // Title
        GUI.Box(new Rect(50, 50, w - 100, h - 50), "ValoPong", titleStyle);

        int heightLeft = h - titH;
        int elH = heightLeft / 4; // element height

        // Start game
        if (GUI.Button(new Rect(150, titH+margin, w - 300, elH - margin), "Start game"))
        {
            Application.LoadLevel(1);
        }

        GUI.Box(new Rect(150, elH + titH + margin, w - 300, elH - margin), "Settings");

        GUI.Label(new Rect(160, elH + titH+40+margin, (w - 320) / 2, 30), "Light server address");
        lightServerAddress = GUI.TextField(new Rect(w / 2, elH + titH + 40 + margin, (w - 320) / 2, 30), lightServerAddress);

        GUI.Label(new Rect(160, elH + titH+80+margin, (w - 320) / 2, 30), "Light server port");
        lightServerPort = GUI.TextField(new Rect(w / 2, elH+titH+80+margin, (w - 320) / 2, 30), lightServerPort);

        sendDataToServer = GUI.Toggle(new Rect(160, elH + titH + 120 + margin, w - 320, 30), sendDataToServer, "Send light data to server");

        if (GUI.Button(new Rect(160, elH + titH + 160 + margin, w - 320, 30), "Save settings"))
        {
            PlayerPrefs.SetString("lightServerAddress", lightServerAddress);
            PlayerPrefs.SetString("lightServerPort", lightServerPort);
            if (sendDataToServer)
                PlayerPrefs.SetInt("sendData", 1);
            else
                PlayerPrefs.SetInt("sendData", 0);
            PlayerPrefs.Save();
        }

        if (GUI.Button(new Rect(150, elH * 2 + titH + margin, w - 300, elH - margin), "Quit"))
        {
            Application.Quit();
        }
    }
}

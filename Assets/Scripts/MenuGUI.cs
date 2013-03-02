using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {


    public GUIStyle titleStyle;

    void OnGUI()
    {
        GUI.Box(new Rect(50, 50, Screen.width - 100, Screen.height - 100), "ValoPong", titleStyle);

        if (GUI.Button(new Rect(150, 200, Screen.width - 300, 200), "Start game"))
        {
            Application.LoadLevel(1);
        }
        if (GUI.Button(new Rect(150, 450, Screen.width - 300, 200), "Quit"))
        {
            Application.Quit();
        }
    }
}

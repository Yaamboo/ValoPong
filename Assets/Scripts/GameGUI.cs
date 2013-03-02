using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

    public GUIStyle pointsStyle;

    void OnGUI()
    {
        Debug.Log(this.gameObject. GetComponent<GameEngine>());
        GUI.Box(new Rect(20, 20, 100, 30), "P1: " + GetComponent<GameEngine>().p1Score, pointsStyle);
        GUI.Box(new Rect(Screen.width - 20 - 100, 20, 100, 30), "P2: " + GetComponent<GameEngine>().p2Score, pointsStyle);
    }
}

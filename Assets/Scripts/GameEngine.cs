using UnityEngine;
using System.Collections;

public class GameEngine : MonoBehaviour {

    public int p1Score;
    public int p2Score;

    public GUIStyle pointsStyle;

    public int p1PaddleID;
    public int p2PaddleID;

    public float ballStartSpeed;
    public float ballMaxSpeed;

    public float ballReturnMultiplier;

    private Hashtable eventLights;
    private Vector2 lightPosition;

    private System.DateTime p1Cooldown, p2Cooldown;
    private System.TimeSpan cooldownPeriod = new System.TimeSpan(500000);

    private bool isDead;

    private System.DateTime playRestartTime;

    private GameObject ExcellentText;

    private GameObject OhNoSound, PongSound, StartSound, WindSound;

    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 400, 30), "P1: " + p1Score, pointsStyle);
        GUI.Box(new Rect(Screen.width - 20 - 400, 20, 400, 30), "P2: " + p2Score, pointsStyle);

        //GUI.Box(new Rect(0, Screen.height - 50, Screen.width, 50), lightPosition.x + "," + lightPosition.y + "\n" + isDead +" " + playRestartTime);
    }

	// Use this for initialization
	void Start () {
        p1Score = 0;
        p2Score = 0;

        ExcellentText = gameObject.transform.FindChild("Excellent text").gameObject;

        OhNoSound = gameObject.transform.FindChild("OhNoSound").gameObject;
        PongSound = gameObject.transform.FindChild("PongSound").gameObject;
        StartSound = gameObject.transform.FindChild("StartSound").gameObject;
        WindSound = gameObject.transform.FindChild("WindSound").gameObject;

        Debug.Log(WindSound);

        // Create table of event lights , id => event light
        eventLights = new Hashtable();
        foreach (ChangeColour cc in FindObjectsOfType(typeof(ChangeColour)))
        {
            eventLights.Add(cc.lightId, cc.gameObject);
        }

        resetAnimations();
        isDead = true;
        playRestartTime = System.DateTime.Now + System.TimeSpan.FromSeconds(1);
        foreach (GameObject light in eventLights.Values)
        {
            light.animation.Stop();
            light.animation.CrossFade("DeadAnimation", 0.2f);
        }
        StartSound.audio.Play();

        //startBall();
	}

    private void resetAnimations()
    {
        foreach (GameObject light in eventLights.Values)
        {
            /*light.animation.Stop();
            light.animation.Stop("BlackAnimation");
            light.animation.Stop("DeadAnimation");
            light.animation.Play("BlackAnimation", PlayMode.StopAll);*/


            if (light.GetComponent<ChangeColour>().lightId < p1PaddleID || light.GetComponent<ChangeColour>().lightId > p2PaddleID)
            {
                light.animation.Play("LightAnimation");
                light.animation.name = "LightAnimation";
            }
            else if (light.GetComponent<ChangeColour>().lightId == p1PaddleID)
            {
                light.animation.Play("P1PaddleAnimation");
                light.animation.name = "P1PaddleAnimation";
            }
            else if (light.GetComponent<ChangeColour>().lightId == p2PaddleID)
            {
                light.animation.Play("P2PaddleAnimation");
                light.animation.name = "P2PaddleAnimation";
            }
            else
            {
                light.animation.Play("BlackAnimation");
                light.animation.name = "BlackAnimation";
            }
        }
    }

    private void startBall()
    {
        if (Random.value >= 0.5)
            lightPosition = new Vector2((p2PaddleID - p1PaddleID) / 2 + p1PaddleID, ballStartSpeed * Time.deltaTime);
        else
            lightPosition = new Vector2((p2PaddleID - p1PaddleID) / 2 + p1PaddleID, ballStartSpeed * -Time.deltaTime);
        WindSound.audio.Play();
    }
	
	// Update is called once per frame
	void Update () {

        if (!isDead)
        {
            /*
            if (lightPosition.x < p1PaddleID - 0.5)
                lightPosition.y = 0.10f;
            if (lightPosition.x > p2PaddleID + 0.5)
                lightPosition.y = -0.10f;
            */
            lightPosition.x += lightPosition.y;


            if (lightPosition.x > 35.5)
                lightPosition.x -= 36;
            if (lightPosition.x <= -0.50)
                lightPosition.x += 36;

            
            float P1marginOfError = lightPosition.x - p1PaddleID;
            //Debug.Log("P1 margin of error: " + P1marginOfError);
            if (P1marginOfError < 0)
            {
                // fail
                isDead = true;
            }
            
            float P2marginOfError = p2PaddleID - lightPosition.x;
            //Debug.Log("P2 margin of error: " + P2marginOfError);
            if (P2marginOfError < 0)
            {
                // fail
                isDead = true;
            }

            if (Input.GetButtonDown("Player 1 paddle") && !isDead)
            {
                if (p1Cooldown <= System.DateTime.Now)
                {

                    
                    if (P1marginOfError <= 0.5)
                    {
                        // Eksölent
                        if (Mathf.Abs(lightPosition.y) < ballMaxSpeed)
                            lightPosition.y *= ballReturnMultiplier;
                        lightPosition.y *= -1;
                        p1Score += 1000;

                        PongSound.audio.Play();
                        ExcellentText.animation.Play();
                        ExcellentText.audio.Play();
                    }
                    else if (P1marginOfError <= 2)
                    {
                        // good
                        lightPosition.y *= -1;
                        lightPosition.y *= 1.15f;
                        p1Score += 500;
                        PongSound.audio.Play();
                    }
                    else
                    {
                        // do nothing
                    }
                }
                p1Cooldown = System.DateTime.Now + cooldownPeriod;

            }
            if (Input.GetButtonDown("Player 2 paddle") && !isDead)
            {
                if (p2Cooldown <= System.DateTime.Now)
                {
                    if (P2marginOfError <= 0.5)
                    {
                        // Eksölent
                        if (Mathf.Abs(lightPosition.y) < ballMaxSpeed)
                            lightPosition.y *= ballReturnMultiplier;
                        lightPosition.y *= -1;
                        p2Score += 1000;

                        PongSound.audio.Play();
                        ExcellentText.animation.Play();
                        ExcellentText.audio.Play();

                    }
                    else if (P2marginOfError <= 2)
                    {
                        // good
                        lightPosition.y *= -1;
                        lightPosition.y *= 1.15f;
                        p2Score += 500;
                        PongSound.audio.Play();
                    }
                    else
                    {
                        // do nothing
                    }
                }

                p2Cooldown = System.DateTime.Now + cooldownPeriod;

            }
            

            if (Input.GetKey(KeyCode.Escape))
            {
                isDead = true;
                p1Score = 0;
                p2Score = 0;
                Application.LoadLevel(0);
            }

            if (isDead == true)
            {
                // fail
                playRestartTime = System.DateTime.Now + System.TimeSpan.FromSeconds(3);

                WindSound.audio.Stop();

                OhNoSound.audio.Play();
                
                foreach (GameObject light in eventLights.Values)
                {
                    light.animation.Stop();
                    light.animation.CrossFade("DeadAnimation", 0.2f);
                }
            }

            setLightColors();
        }
        if (isDead && playRestartTime < System.DateTime.Now)
        {
            resetAnimations();
            startBall();
            isDead = false;
        }
	}

    GameObject getLight(int id)
    {
        if (eventLights.ContainsKey(id))
            return (GameObject)eventLights[id];
        else
            return null;
    }

    void setLightColors()
    {
        if (isDead == true && playRestartTime < System.DateTime.Now)
        {
            
        }
        else
        {


            int currentLight = (int)Mathf.Round(lightPosition.x);
            
            int prevLight = currentLight - 1;
            if (prevLight < 0)
                prevLight += 36;
            int nextLight = currentLight + 1;
            if (nextLight > 35)
                nextLight -= 36;

            if (lightPosition.y >= 0 && nextLight < p2PaddleID)
                getLight(nextLight).animation.CrossFade("LightPositionAnimation", 0.2f);
            else if (lightPosition.y < 0 && prevLight > p1PaddleID)
                getLight(prevLight).animation.CrossFade("LightPositionAnimation", 0.2f);
        }
    }
}

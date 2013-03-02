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

    private GameObject OhNoSound, PongSound;

    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 400, 30), "P1: " + p1Score, pointsStyle);
        GUI.Box(new Rect(Screen.width - 20 - 400, 20, 400, 30), "P2: " + p2Score, pointsStyle);

        GUI.Box(new Rect(0, Screen.height - 50, Screen.width, 50), lightPosition.x + "," + lightPosition.y + "\n" + isDead +" " + playRestartTime);
    }

	// Use this for initialization
	void Start () {
        p1Score = 0;
        p2Score = 0;

        ExcellentText = gameObject.transform.FindChild("Excellent text").gameObject;

        OhNoSound = gameObject.transform.FindChild("OhNoSound").gameObject;
        PongSound = gameObject.transform.FindChild("PongSound").gameObject;

        // Create table of event lights , id => event light
        eventLights = new Hashtable();
        foreach (ChangeColour cc in FindObjectsOfType(typeof(ChangeColour)))
        {
            eventLights.Add(cc.lightId, cc.gameObject);
        }
        resetAnimations();

        startBall();
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
        lightPosition = new Vector2((p2PaddleID - p1PaddleID) / 2 + p1PaddleID, ballStartSpeed * Time.deltaTime);
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

            if (Input.GetKey(KeyCode.Space) && !isDead)
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
                    else if (P1marginOfError <= 1)
                    {
                        // good
                        lightPosition.y *= -1;
                        p1Score += 500;
                        this.audio.Play();
                    }
                    else
                    {
                        // do nothing
                    }
                }
                p1Cooldown = System.DateTime.Now + cooldownPeriod;

            }
            if (Input.GetKey(KeyCode.KeypadEnter) && !isDead)
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
                    else if (P2marginOfError <= 1)
                    {
                        // good
                        lightPosition.y *= -1;
                        p2Score += 500;
                        this.audio.Play();
                    }
                    else
                    {
                        // do nothing
                    }
                }

                p2Cooldown = System.DateTime.Now + cooldownPeriod;

            }

            if (isDead == true)
            {
                // fail
                playRestartTime = System.DateTime.Now + System.TimeSpan.FromSeconds(3);
                
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
            /*
            float curIntensity, prevIntensity, nextIntensity;

            if (lightPosition.x <= currentLight)
            {
                curIntensity = 1 - (currentLight - lightPosition.x);
                prevIntensity = 1 - (lightPosition.x - prevLight);
                nextIntensity = 0;
            }
            else
            {
                curIntensity = 1 - (lightPosition.x - currentLight);
                prevIntensity = 0;
                nextIntensity = 1 - (nextLight - lightPosition.x);
            }

            // ugly hack
            if (nextIntensity > 35)
                nextIntensity -= 36;
            if (prevIntensity > 35)
                prevIntensity -= 36;


            if (currentLight > 35)
                currentLight -= 36;
            if (currentLight < 0)
                currentLight += 36;

            //Debug.Log(prevLight + ", " + currentLight + ", " + nextLight);

            //Debug.Log(prevIntensity + ", " + curIntensity + ", " + nextIntensity);

            /*getLight(prevLight).renderer.material.color = new Color(prevIntensity, prevIntensity, prevIntensity);
            getLight(currentLight).renderer.material.color = new Color(curIntensity, curIntensity, curIntensity);
            getLight(nextLight).renderer.material.color = new Color(nextIntensity, nextIntensity, nextIntensity);*/

            if (lightPosition.y >= 0 && nextLight < p2PaddleID)
                getLight(nextLight).animation.Play("LightPositionAnimation");
            else if (lightPosition.y < 0 && prevLight > p1PaddleID)
                getLight(prevLight).animation.Play("LightPositionAnimation");
        }
    }
}

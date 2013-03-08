using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class UDPLightSender : MonoBehaviour {

    private IPAddress lightServerIPAddress;// = IPAddress.Parse("194.79.19.181");
	private Socket lightServerConnection;
    private IPEndPoint lightServerEndpoint;

    public string lightServerNickname;
    public string lightServerAddress;
    public int lightServerPort; // = 9909;

    public bool ActuallySendToServer;

    private SphereCollider[] spheres;

	/// <summary>
	/// Initialisation of the light server client.
	/// </summary>
	void Start ()
    {
        // get all the light spheres from the children
        spheres = gameObject.GetComponentsInChildren<SphereCollider>();

        // Load settings for the server
        loadServerSettings();

        // Set the ip address
        lightServerIPAddress = findLightServerIPAddress();

        // random nickname
        lightServerNickname += (int)Random.Range(1000, 9999);


        Debug.Log("Light server address: " + lightServerAddress.ToString() + "\nLight server port: " + lightServerPort);
        if (!ActuallySendToServer)
            Debug.LogWarning("Not sending light information to light server (ActuallySendToServer = false)");

		try {

            if (lightServerIPAddress != null && lightServerPort >= 1024 && lightServerPort <= 65535)
            {
                lightServerEndpoint = new IPEndPoint(lightServerIPAddress, lightServerPort);
                lightServerConnection = new Socket(lightServerEndpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            }
            else
            {
                Debug.LogError("There's something wrong with the light server address or port.");
            }

		} catch (System.Exception e) {
            Debug.LogException(e);
		}
	}

    /// <summary>
    /// Loads the settings set by the player.
    /// </summary>
    private void loadServerSettings()
    {
        lightServerAddress = PlayerPrefs.GetString("lightServerAddress", "melmacian.net");
        try
        {
            lightServerPort = int.Parse(PlayerPrefs.GetString("lightServerPort", "9909"));
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            lightServerPort = 0;
        }
        int sendData = PlayerPrefs.GetInt("sendData", 1);
        if (sendData == 0)
            ActuallySendToServer = false;
        else
            ActuallySendToServer = true;
    }

    /// <summary>
    /// Finds the light server ip address from its hostname.
    /// </summary>
    /// <returns>the IPAddress of the light server</returns>
    private IPAddress findLightServerIPAddress()
    {
        try
        {
            IPAddress[] ips = Dns.GetHostAddresses(lightServerAddress);
            foreach (IPAddress ip in ips)
            {
                return ip;
            }
            return null;
            //lightServerIPAddress = IPAddress.Parse(lightServerAddress);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
	
	/// <summary>
    /// Send the light information to the light server
	/// </summary>
    void Update () {

        try
        {
            if (lightServerConnection != null)
            {
                byte[] packet = createLightUpdatePacket();

                if (ActuallySendToServer)
                    lightServerConnection.SendTo(packet, lightServerEndpoint);
                //Debug.Log("Sent data");

            }
            else
            {
                Debug.LogError("There's something wrong with the connection to the light server.");
            }
        } catch (System.Exception e) {
            Debug.LogException(e);
        }
	}

    /// <summary>
    /// Creates a light update packet. The format can be found for example at https://github.com/epeli/effectserver/blob/master/examples/python.py
    /// </summary>
    /// <returns>byte array containing all the information needed by the server</returns>
    private byte[] createLightUpdatePacket()
    {
        
                              // 3 bits + nickname length + 7 bits / light
        int byteArrayLength = 3 + lightServerNickname.Length + 6 * spheres.Length;
        byte[] b = new byte[byteArrayLength];
        b[0] = 1;   // always 1
        b[1] = 0;   // nickname separator

        for (int j = 0; j < lightServerNickname.Length; j++)
        {
            b[j + 2] = (byte)lightServerNickname[j];
        }
        
        int i = 0;
        int startPos = lightServerNickname.Length + 2;
        b[startPos] = 0;
        startPos++;
        foreach (SphereCollider sc in spheres)
        {

            int red, green, blue;
            red = (int)(sc.renderer.material.color.r * 255);
            if (red < 0)
                red = 0;
            if (red > 255)
                red = 255;
            green = (int)(sc.renderer.material.color.g * 255);
            if (green< 0)
                green = 0;
            if (green > 255)
                green = 255;
            blue = (int)(sc.renderer.material.color.b * 255);
            if (blue < 0)
                blue = 0;
            if (blue > 255)
                blue = 255;


            b[startPos + 0] = 1; // always 1
            b[startPos + 1] = (byte)(sc.GetComponent<ChangeColour>().lightId); // light id
            b[startPos + 2] = 0; // always 0
            b[startPos + 3] = (byte)red;
            b[startPos + 4] = (byte)green;
            b[startPos + 5] = (byte)blue;
            i++;
            startPos += 6;
        }
        //printPacket(b); // uncomment to debug
        return b;
    }

    /// <summary>
    /// A debug method for printing the sent light packets
    /// </summary>
    /// <param name="byteArray">the light packet to be printed</param>
    private void printPacket(byte[] byteArray)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder("Sending following bytes to server:\n");
        foreach (byte b in byteArray)
        {
            sb.Append(b + ", ");
        }

        Debug.Log(sb);
    }
}

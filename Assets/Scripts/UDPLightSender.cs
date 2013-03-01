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

    public bool DEBUGSendToServer;
	
	// Initialises the light server client.
	void Start () {

        Debug.Log("Light server address: " + lightServerAddress.ToString() + "\nLight server port: " + lightServerPort);
        if (!DEBUGSendToServer)
            Debug.LogWarning("Not sending light information to light server (DEBUGSendToServer = false)");

		try {

            lightServerIPAddress = IPAddress.Parse(lightServerAddress);

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
	
	// Send the light information to the light server
	void Update () {

        try
        {
            if (lightServerConnection != null)
            {
                if (DEBUGSendToServer)
                    lightServerConnection.SendTo(createLightUpdatePacket(), lightServerEndpoint);
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

    // Creates a light update packet.
    private byte[] createLightUpdatePacket()
    {
        // Todo: tämä kaikki voitaisiin tehdä vain kerran, jos halutaan optimoida
        SphereCollider[] spheres = gameObject.GetComponentsInChildren<SphereCollider>();
                              // 3 bits + nickname length + 7 bits / light
        int byteArrayLength = 3 + lightServerNickname.Length + 6 * spheres.Length;
        byte[] b = new byte[byteArrayLength];
        b[0] = 1;   // always 1
        b[1] = 0;   // nickname separator

        for (int j = 0; j < lightServerNickname.Length; j++)
        {
            b[j + 2] = (byte)lightServerNickname[j];
        }
//        b[lightServerNickname.Length + 2] = 0; // nickname separator
        
        int i = 0;
        int startPos = lightServerNickname.Length + 2;
        b[startPos] = 0;
        startPos++;
        foreach (SphereCollider sc in spheres)
        {
            //b[startPos] = 0; // light separator
            b[startPos + 0] = 1; // always 1
            b[startPos + 1] = (byte)(sc.GetComponent<ChangeColour>().lightId); // light id
            b[startPos + 2] = 0; // always 0
            b[startPos + 3] = (byte)(sc.renderer.material.color.r * 255);
            b[startPos + 4] = (byte)(sc.renderer.material.color.g * 255);
            b[startPos + 5] = (byte)(sc.renderer.material.color.b * 255);
            i++;
            startPos += 6;
        }
        /*
        if (spheres.Length > 1)
        {
            //Debug.Log("jou");
            firstSphere = spheres[1];
            b = new byte[] { 1, 0, 111, 0, 1, 0, 0, (byte)(firstSphere.renderer.material.color.r * 255), (byte)(firstSphere.renderer.material.color.g * 255), (byte)(firstSphere.renderer.material.color.b * 255) };
        }
        else
        {
        }
        */
        printPacket(b);
        return b;
    }

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

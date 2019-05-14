using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Menu : MonoBehaviour
{
    MyNetworkDiscovery myNetworkDiscovery;

    void Awake()
    {
        myNetworkDiscovery = FindObjectOfType<MyNetworkDiscovery>();
    }

    void Start()
    {
        StopDiscovery();
    }


    public void CrearUnaPartida()
    {
        StopDiscovery();
        myNetworkDiscovery.StartAsServer();
        NetworkManager.singleton.StartHost();
    }

    private void StopDiscovery()
    {
        if (myNetworkDiscovery.running)
        {
            myNetworkDiscovery.StopBroadcast();
        }
    }

    public void UnirseUnaPartida()
    {
        StopDiscovery();
        myNetworkDiscovery.StartAsClient();

    }
}

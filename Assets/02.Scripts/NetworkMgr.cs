using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMgr : MonoBehaviour {

    private const string ip = "127.0.0.1";

    private const int port = 30000;

    private bool _useNat = false;

    public GameObject player;

    private void OnGUI()
    {
        // 현재 사용자의 네트워크에 접속 여부 판단
        if(Network.peerType == NetworkPeerType.Disconnected)
        {
            // 게임 서버 생성 버튼
            if(GUI.Button(new Rect(20, 20, 200, 25), "Start Server"))
            {
                // 게임 서버 생성 : InitializeServer(접속자, 포트번, NAT사용여부) 
				Network.InitializeServer(20, port, _useNat);
            }

            if(GUI.Button(new Rect(20, 50, 200, 25), "Connect to Server"))
            {
                Network.Connect(ip, port);
            }
        } else {
            // 서버일 때 메시지 출력
            if(Network.peerType == NetworkPeerType.Server)
            {
                GUI.Label(new Rect(20, 20, 200, 25), "Initialization Server...");
                GUI.Label(new Rect(20, 50, 200, 25), "Client Count = " + Network.connections.Length.ToString());
            }

            // 클라이언트로 접속했을 때의 메시지 출력
            if(Network.peerType == NetworkPeerType.Client)
            {
                GUI.Label(new Rect(20, 20, 200, 25), "Connect to Server");
            }
        }
    }

    void OnServerInitialized()
    {
        CreatePlayer();
    }

    void OnConnectedToServer()
    {
        CreatePlayer();
    }

    void CreatePlayer()
    {
        Vector3 pos = new Vector3(Random.Range(-20.0f, 20.0f), 0.0f, Random.Range(-20.0f, 20.0f));
        Network.Instantiate(player, pos, Quaternion.identity, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string gameVersion = "0.0.1";
    [SerializeField]
    private byte maxPlayerPerRoom = 4;

    [SerializeField]
    private string nickname = string.Empty;

    [SerializeField]
    private Button connectButton = null;
    [SerializeField]
    private Button joinRandomButton = null;


    private void Awake()
    {
        // 마스터가 PhotonNetwork.LoadLevel()을 호출하면,
        // 모든 플레이어가 동일한 레벨을 자동으로 로드
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = true;
        joinRandomButton.interactable = false;
    }

    // ConnectButton이 눌러지면 호출
    public void Connect()
    {
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.Log("NickName is empty");
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            Debug.LogErrorFormat("IsConnected: {0}", PhotonNetwork.IsConnected);
            //PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            PhotonNetwork.GameVersion = gameVersion;
            // 포톤 클라우드에 접속을 시작하는 지점
            // 접속에 성공하면 OnConnectedToMaster 메서드 호출
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRandomRoom()
    {
        Debug.LogError("JoinRandomRoom");
        PhotonNetwork.JoinRandomRoom();
        joinRandomButton.interactable = false;
    }

    // InputField_NickName과 연결해 닉네임을 가져옴
    public void OnValueChangedNickName(string _nickname)
    {
        nickname = _nickname;
        Debug.LogErrorFormat("Nickname: {0}", nickname);
        PhotonNetwork.NickName = nickname;
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("Connected to Master: {0}", nickname);

        connectButton.interactable = false;
        joinRandomButton.interactable = true;

        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;
        joinRandomButton.interactable = false;

        // 방을 생성하면 OnJoinedRoom 호출
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        SceneManager.LoadScene("Room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        if(PhotonNetwork.IsConnected)
            connectButton.interactable = false;

        joinRandomButton.interactable = true;

        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}

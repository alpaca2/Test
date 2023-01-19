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
        // �����Ͱ� PhotonNetwork.LoadLevel()�� ȣ���ϸ�,
        // ��� �÷��̾ ������ ������ �ڵ����� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        connectButton.interactable = true;
        joinRandomButton.interactable = false;
    }

    // ConnectButton�� �������� ȣ��
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
            // ���� Ŭ���忡 ������ �����ϴ� ����
            // ���ӿ� �����ϸ� OnConnectedToMaster �޼��� ȣ��
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRandomRoom()
    {
        Debug.LogError("JoinRandomRoom");
        PhotonNetwork.JoinRandomRoom();
        joinRandomButton.interactable = false;
    }

    // InputField_NickName�� ������ �г����� ������
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

        // ���� �����ϸ� OnJoinedRoom ȣ��
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

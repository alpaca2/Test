using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerPrefab = null;
    [SerializeField]
    private TextMeshProUGUI nicknameUiPrefab = null;

    private List<GameObject> playerGoList = new List<GameObject>();
    private GameObject[] playerGoArr = new GameObject[4];


    private void Start()
    {
        if (playerPrefab != null)
        {
            GameObject go = PhotonNetwork.Instantiate(
                playerPrefab.name, 
                new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f)), 
                Quaternion.identity, 
                0);

            for(int i = 0; i < playerGoArr.Length; ++i)
            {
                if(playerGoArr[i] == null)
                {
                    playerGoArr[i] = go;
                    go.GetComponent<PlayerControl>().SetMaterial(i + 1);
                    break;
                }
            }
        }
    }


    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        SceneManager.LoadScene("Launcher");
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Entered Room: {0}", otherPlayer.NickName);

        // RPC(Remote Procedure Call): All(본인 포함 전체), Other(본인 제외 전체)
    }


    public void ApplyPlayerList()
    {
        // 전체 클라이언트에서 함수 호출
        photonView.RPC("RPCApplyPlayerList", RpcTarget.AllBuffered);
    }
    public void SetPlayersColor()
    {
        photonView.RPC("RPCSetPlayersColor", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void RPCApplyPlayerList()
    {
        int playerCnt = PhotonNetwork.CurrentRoom.PlayerCount;
        // 플레이어 리스트가 최신이라면 건너뜀
        if (playerCnt == playerGoList.Count) return;

        // Array
        int count = 0;
        for(int i = 0; i < playerGoArr.Length; i++)
        {
            if (playerGoArr[i] != null)
                ++count;
        }
        Debug.LogErrorFormat("playerCnt: {0}, count: {1}", playerCnt, count);
        if (playerCnt == count) return;

        // 현재 방에 접속해 있는 플레이어의 수
        Debug.LogError("CurrentRoom PlayerCount : " + playerCnt);

        // 현재 생성되어 있는 모든 포톤뷰 가져오기
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // 매번 재정렬을 하는게 좋으므로 플레이어 게임오브젝트 리스트를 초기화
        playerGoList.Clear();
        System.Array.Clear(playerGoArr, 0, playerGoArr.Length);

        // 현재 생성되어 있는 포톤뷰 전체와
        // 접속중인 플레이어들의 액터넘버를 비교해,
        // 플레이어 게임오브젝트 리스트에 추가
        for (int i = 0; i < playerCnt; ++i)
        {
            // 키는 0이 아닌 1부터 시작
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // 만약 PhotonNetwork.Instantiate를 통해서 생성된 포톤뷰가 아니라면 넘김
                if (photonViews[j].isRuntimeInstantiated == false) continue;
                // 만약 현재 키 값이 딕셔너리 내에 존재하지 않는다면 넘김
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // 포톤뷰의 액터넘버
                int viewNum = photonViews[j].Owner.ActorNumber;
                // 접속중인 플레이어의 액터넘버
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                // 액터넘버가 같은 오브젝트가 있다면,
                if (viewNum == playerNum)
                {
                    // 게임오브젝트 이름도 알아보기 쉽게 변경
                    photonViews[j].gameObject.name = "Player_" + photonViews[j].Owner.NickName;
                    // 실제 게임오브젝트를 리스트에 추가
                    playerGoList.Add(photonViews[j].gameObject);
                    playerGoArr[i] = photonViews[j].gameObject;

                }
            }
        }

        // 디버그용
        PrintPlayerList();
    }


    [PunRPC]
    public void RPCSetPlayersColor()
    {
        for (int i = 0; i < playerGoArr.Length; i++)
        {
            if (playerGoArr[i] != null)
            {
                PlayerControl pc = playerGoArr[i].GetComponent<PlayerControl>();
                pc.SetMaterial(i + 1);
            }
        }
    }

    [PunRPC]
    public void RPCShowNickname()
    {
        int playerCnt = PhotonNetwork.CountOfPlayersInRooms;
        Player[] playerList = PhotonNetwork.PlayerList;

        for(int i = 0; i < playerList.Length; ++i)
        {
            int actNum = playerList[i].ActorNumber;
            string nickname = playerList[i].NickName;
            string uid = playerList[i].UserId;
            Debug.LogFormat("ActNum: {0}, Nickname: {1}, UID: {2}", actNum, nickname, uid);
            playerList[i].
        }
    }

    private void PrintPlayerList()
    {
        foreach (GameObject go in playerGoList)
        {
            if (go != null)
            {
                Debug.LogError(go.name);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}", otherPlayer.NickName);
    }

    public void LeaveRoom()
    {
        Debug.Log("Leave Room");
        PhotonNetwork.LeaveRoom();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace yj.script
{
    public class NetworkSystem : MonoBehaviourPunCallbacks
    {
        public Image FadeImage;
        public InputField NickNameInput;
        public float aniTime;
        private float timer = 0f;

        [Header("Panel")]
        public GameObject DisconnectPanel;
        public GameObject RespawnPanel;
        public GameObject CharacterPanel;
        public GameObject DescriptionPanel;

        [Header("LobbyPanel")]
        public GameObject LobbyPanel;
        public Text WelcomeText;
        public InputField RoomInput;
        List<RoomInfo> myList = new List<RoomInfo>();
        public Button[] CellBtn;
        public Button PreviousBtn;
        public Button NextBtn;
        int currentPage = 1, maxPage, multiple;

        [Header("GameScene")]
        public GameObject ChatPanel;
        public InputField ChatInput;
        public Text[] ChatText;
        public Button SendBnt;

        public Button ExitBnt;
        public Button GoLobbyBnt;

        public GameObject PlayerInfoPanel;
        public Text PlayerInfoText;
        public Sprite[] Profile;
        public Image CharacterInfoImage;
        public GameObject SetGamePanel;
        public GameObject SetBnt;
        public Animator AN;

        public Button SetExitBnt;

        private PlayerScript ps;

        public PhotonView PV;

        public string playertype;

        //Init & Update
        private void Awake()
        {
            Screen.SetResolution(1920, 1080, false);
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.SerializationRate = 30;

            Color color = FadeImage.color;
            color.a = 1f;
            FadeImage.color = color;
        }
        private void Start()
        {
        }
        private void Update()
        {
            FadeImageStart();

            if (Input.GetKeyDown(KeyCode.F))
            {
                ChatInput.Select();
            }
        }
        private void FadeImageStart()
        {
            Color color = FadeImage.color;

            timer += Time.deltaTime / aniTime;
            color.a = Mathf.Lerp(1f, 0f, timer);
            FadeImage.color = color;

            if (color.a == 0f) FadeImage.enabled = false;

        }

        //Photon
        public void Connect() => PhotonNetwork.ConnectUsingSettings();
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
            PhotonNetwork.JoinLobby();
        }
        public override void OnJoinedLobby()
        {
            CharacterPanel.SetActive(false);
            LobbyPanel.SetActive(true);
            PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;

            WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
            myList.Clear();
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            DisconnectPanel.SetActive(true);
            RespawnPanel.SetActive(false);
            ChatPanel.SetActive(false);
        }

        //Panel Active Buttons
        public void OnSelectionPanel()
        {
            DisconnectPanel.SetActive(false);
            CharacterPanel.SetActive(true);
        }
        public void OnDescriptionPanel()
        {
            DescriptionPanel.SetActive(true);
        }
        public void ExitDescriptionPanel()
        {
            DescriptionPanel.SetActive(false);
        }

        //Room
        #region 방리스트
        public void MyListClick(int num) //방 클릭시
        {
            LobbyPanel.SetActive(false);
            //RoomPanel.SetActive(true);
            if (num == -2) --currentPage;
            else if (num == -1) ++currentPage;
            else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            MyListRenewal();
        }
        private void MyListRenewal()
        {
            // 최대페이지
            maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

            // 이전, 다음버튼
            PreviousBtn.interactable = (currentPage <= 1) ? false : true;
            NextBtn.interactable = (currentPage >= maxPage) ? false : true;

            // 페이지에 맞는 리스트 대입
            multiple = (currentPage - 1) * CellBtn.Length;
            for (int i = 0; i < CellBtn.Length; i++)
            {
                CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
                CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
                CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
            }
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            int roomCount = roomList.Count;
            for (int i = 0; i < roomCount; i++)
            {
                if (!roomList[i].RemovedFromList)
                {
                    if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                    else myList[myList.IndexOf(roomList[i])] = roomList[i];
                }
                else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
            }
            MyListRenewal();
        }
        #endregion

        #region 방만들기
        public void CreateRoom()
        {
            LobbyPanel.SetActive(false);
            PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 6 });
        }
        public void JoinRandomRoom()
        {
            LobbyPanel.SetActive(false);
            //RoomPanel.SetActive(true);
            PhotonNetwork.JoinRandomRoom(); //랜덤방 입장
        }
        public void LeaveRoom() => PhotonNetwork.LeaveRoom(); //방 나가기

        public override void OnJoinedRoom()
        {
            CharacterPanel.SetActive(false);
            PlayerInfoPanel.SetActive(true);

            StartCoroutine("DestroyBullet");
            PlayerSpawn();


            ChatInput.text = "";
            for (int i = 0; i < ChatText.Length; i++)
            {
                ChatText[i].text = "";
            }//채팅창 초기화 (스크롤뷰에 text 배치 해둠)

            PlayerInfoText.text = PhotonNetwork.LocalPlayer.NickName + " 님";

            switch (playertype)
            {
                case "Player":
                    CharacterInfoImage.sprite = Profile[0];
                    break;
                case "BlackPlayer":
                    CharacterInfoImage.sprite = Profile[1];
                    break;
                case "WhitePlayer":
                    CharacterInfoImage.sprite = Profile[2];
                    break;
            }
        }

        public void BackLobby()
        {
            LeaveRoom();
            PlayerInfoPanel.SetActive(false);
            SetGamePanel.SetActive(false);
            RoomInput.text = "";
            OnJoinedLobby();
        }
        #endregion

        //Player & Bullet Init
        public void PlayerSpawn()
        {
            PhotonNetwork.Instantiate(playertype, new Vector3(Random.Range(-5f, 3f), -2, 0), Quaternion.identity);
            RespawnPanel.SetActive(false);
            ChatPanel.SetActive(true);
        }
        public void Respawn()
        {
            RespawnPanel.SetActive(false);
            PlayerInfoPanel.SetActive(true);
            PlayerSpawn();
        }

        private IEnumerator DestroyBullet()
        {
            yield return new WaitForSeconds(0.2f);
            foreach (GameObject GO in GameObject.FindGameObjectsWithTag("Bullet"))
            {
                if (GO != null)
                {
                    GO.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
                }
            }
        }
        public void ExitGame()
        {
            PhotonNetwork.Disconnect();
            PlayerInfoPanel.SetActive(false);
            SetGamePanel.SetActive(false);
        }

        public void OnSettingPanel()
        {
            AN.SetBool("click", true);
            SetGamePanel.SetActive(true);
        }
        public void ExitSetClick()
        {
            SetGamePanel.SetActive(false);
            AN.SetBool("click", false);
        }

        //Chat
        #region 채팅기능
        public void SendText()
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName 
                + " : " + ChatInput.text);
            ChatInput.text = "";
        }

        [PunRPC] // 동기화 함수
        private void ChatRPC(string msg)
        {
            bool isInput = false;
            for (int i = 0; i < ChatText.Length; i++)
                if (ChatText[i].text == "")
                {
                    isInput = true;
                    ChatText[i].text = msg;
                    break;
                }
            if (!isInput) // 꽉차면 한칸씩 위로 올림
            {
                for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
                ChatText[ChatText.Length - 1].text = msg;
            }
        }
        #endregion

    }

}
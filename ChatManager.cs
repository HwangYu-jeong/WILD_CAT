using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class ChatManager : MonoBehaviourPunCallbacks
{
    public Player player;
    public PhotonView photonView;
    public GameObject BubbleSpeechObject;
    public Text UpdateText;

    private InputField ChatInputField;
    private bool DisableSend;

    private void Awake()
    {
        ChatInputField = GameObject.Find("ChatInput").GetComponent<InputField>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            //if (DisableSend && ChatInputField.isFocused)
            //{
                if (ChatInputField.text != "" && ChatInputField.text.Length > 0 && Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    photonView.RPC("SendMassage", RpcTarget.All, ChatInputField.text);
                   
                    ChatInputField.text = "";
                 
                }
            //}
        }
    }

    [PunRPC]
    private void SendMassage(string msg)
    {
        DisableSend = true;
        BubbleSpeechObject.SetActive(true);
        UpdateText.text = msg;
        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        BubbleSpeechObject.SetActive(false);
        DisableSend = false;
    }

}

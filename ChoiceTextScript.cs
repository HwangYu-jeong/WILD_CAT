using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceTextScript : MonoBehaviour
{
    public Text Info;
    public Button PlayBnt;

    public string playertype;

    public void Update()
    {
        switch (playertype)
        {
            case "Player":
                Info.text = "���� ���� ����";
                break;
            case "BlackPlayer":
                Info.text = "�׷� ���� ����";
                break;
            case "WhitePlayer":
                Info.text = "��Ű ���� ����";
                break;
        }

        if (playertype == null)
        {
            PlayBnt.interactable = false;
        }

        else
            PlayBnt.interactable = true;
    }

}

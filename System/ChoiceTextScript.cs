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
                Info.text = "포비 냥이 선택";
                break;
            case "BlackPlayer":
                Info.text = "네로 냥이 선택";
                break;
            case "WhitePlayer":
                Info.text = "핑키 냥이 선택";
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

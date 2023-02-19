using UnityEngine;

namespace yj.script
{
    public class CharacterSelection : MonoBehaviour
    {
        private NetworkSystem sn;
        private ChoiceTextScript ct;
        public string PlayerType;
        public Animator anim;
        public CharacterSelection[] chars;


        public void Awake()
        {
            sn = GameObject.Find("NetworkSystem").GetComponent<NetworkSystem>();
            ct = GameObject.Find("CharacterSelectionPanel").GetComponent<ChoiceTextScript>();
        }

        private void Start()
        {
            if (ct.playertype == PlayerType) OnSelect();
            else OnDeSelect();
        }

        public void OnClick()
        {
            sn.playertype = PlayerType;
            ct.playertype = PlayerType;
            OnSelect();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != this) chars[i].OnDeSelect();
            }
        }

        private void OnDeSelect()
        {
            anim.SetBool("typewalk", false);
        }

        private void OnSelect()
        {
            anim.SetBool("typewalk", true);
        }
    }
}
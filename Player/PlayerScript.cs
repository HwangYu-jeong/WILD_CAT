using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Cinemachine;

namespace yj.script {
    public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
    {
        public Rigidbody2D rigid;
        public Animator ani;
        public SpriteRenderer spriteRenderer;
        public PhotonView PV;
        public Text playerName;
        public Image playerHealth;
        public GameObject bubbleSpeechObject;
        public Text updateText;

        public InputField chatInputField;
        public GameObject networkSystem;

        private bool isGround;
        private Vector3 curenrPos;
        private NetworkSystem ns;

        private void Awake()
        {
            // �г���
            playerName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
            playerName.color = PV.IsMine ? Color.green : Color.red;

            ani.SetBool("jump", false);

            if (PV.IsMine)
            {
                // 2D ī�޶�
                var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
                CM.Follow = transform;
                CM.LookAt = transform;
            }

            chatInputField = GameObject.Find("ChatInput").GetComponent<InputField>();
            ns = GameObject.Find("NetworkSystem").GetComponent<NetworkSystem>();
        }

        private void Update()
        {
            if (PV.IsMine)
            {
                // �� �� �̵�
                float axis = Input.GetAxisRaw("Horizontal");
                rigid.velocity = new Vector2(4 * axis, rigid.velocity.y);

                if (Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.D))
                {
                    axis = 0;
                }

                if (axis != 0)
                {
                    ani.SetBool("walk", true);
                    PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis); 
                    // �����ӽ� filpX�� ����ȭ���ֱ� ���ؼ� AllBuffered
                }
                else ani.SetBool("walk", false);

                // alt ����, �ٴ�üũ
                isGround = Physics2D.OverlapCircle((Vector2)transform.position 
                    + new Vector2(0, -1.0f), 0.23f, 1 << LayerMask.NameToLayer("Ground"));

                if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround)
                {
                    Debug.Log("Player Jump!");
                    ani.SetBool("jump", true);
                    PV.RPC("JumpRPC", RpcTarget.All);
                }
                else ani.SetBool("jump", false);

                // x �Ѿ� �߻�
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(spriteRenderer.flipX ? 0.0f : 0.7f, 0.2f, 0) + new Vector3(0, -0.35f, 0), Quaternion.identity)
                        .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, spriteRenderer.flipX ? -1 : 1);
                    ani.SetTrigger("shot");
                }
            }

            // IsMine�� �ƴ� �͵��� �ε巴�� ��ġ ����ȭ
            else if ((transform.position - curenrPos).sqrMagnitude >= 100) transform.position = curenrPos;
            else transform.position = Vector3.Lerp(transform.position, curenrPos, Time.deltaTime * 10);

            if (base.photonView.IsMine)
            {
                if (chatInputField.text != "" && chatInputField.text.Length > 0 && Input.GetKeyDown(KeyCode.S))
                {
                    base.photonView.RPC("SendMassage", RpcTarget.All, chatInputField.text);

                    ns.SendText();

                    chatInputField.text = "";
                }
            }
        }

        [PunRPC]
        private void FlipXRPC(float axis) => spriteRenderer.flipX = axis == -1;

        [PunRPC]
        private void JumpRPC()
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * 550);
        }
        public void Hit()
        {
            playerHealth.fillAmount -= 0.1f;
            if (playerHealth.fillAmount <= 0)
            {
                GameObject.Find("Canvas").transform.Find("DiePanel").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("GameUIPanel").gameObject.SetActive(false);
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered); 
                // ���� ���� ������ ���� AllBuffered�� ����
            }
        }

        public void EatPortion()
        {
            playerHealth.fillAmount = 1.0f;
        }

        [PunRPC]
        public void DestroyRPC() => Destroy(gameObject);

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(playerHealth.fillAmount);
            }
            else
            {
                curenrPos = (Vector3)stream.ReceiveNext();
                playerHealth.fillAmount = (float)stream.ReceiveNext();
            }
        }

        [PunRPC]
        private void SendMassage(string msg)
        {
            bubbleSpeechObject.SetActive(true);
            updateText.text = PhotonNetwork.NickName + " : " + msg;
            StartCoroutine("Remove");
        }

        IEnumerator Remove()
        {
            yield return new WaitForSeconds(4f);
            bubbleSpeechObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere((Vector2)transform.position
                    + new Vector2(0, -1.0f), 0.23f);
        }
    }
}
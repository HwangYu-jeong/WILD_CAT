using UnityEngine;
using Photon.Pun;

namespace yj.script
{
    public class Bullet : MonoBehaviourPunCallbacks
    {
        public PhotonView PV;
        private int dir;

        private void Start() => Destroy(gameObject, 3.5f);

        private void Update() => transform.Translate(Vector3.right * 7 * Time.deltaTime * dir);

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == "Ground") PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            if (!PV.IsMine && col.tag == "Player" && col.GetComponent<PhotonView>().IsMine) 
                // �����ʿ� ���缭 Hit ����
            {
                col.GetComponent<PlayerScript>().Hit(); 
                // �Ѿ��� ���� Player�� PlayerScript�� �ҷ��� Hit ����
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        public void DirRPC(int dir) => this.dir = dir;
         
        [PunRPC]
        public void DestroyRPC() => Destroy(gameObject);
    }
}
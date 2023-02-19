using UnityEngine;
using Photon.Pun;

namespace yj.script
{
    public class Portion : MonoBehaviourPunCallbacks
    {
        public PhotonView PV;
        int dir;

        void OnTriggerEnter2D(Collider2D col) // col�� RPC�� �Ű������� �Ѱ��� �� ����
        {
            if (col.tag == "Player")
            {
                col.GetComponent<PlayerScript>().EatPortion();
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void DirRPC(int dir) => this.dir = dir;

        [PunRPC]
        private void DestroyRPC() => Destroy(gameObject);
    }
}
using UnityEngine;
using Photon.Pun;

namespace yj.script
{
    public class Portion : MonoBehaviourPunCallbacks
    {
        public PhotonView PV;
        int dir;

        void OnTriggerEnter2D(Collider2D col) // col을 RPC의 매개변수로 넘겨줄 수 없다
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
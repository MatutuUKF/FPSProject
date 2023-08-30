using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;

    int kills;
    int deaths;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController() {

        Transform spawnpoint = SpawnManager.instance.GetSpawnPoint();
        Debug.Log(spawnpoint.position);
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID});
        
    }

    public void Die() {
        //First edit score die++
        ScoreBoardDieEdit();

        PhotonNetwork.Destroy(controller);
        CreateController();
        


    }

    private void ScoreBoardDieEdit() {
        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void GetKill() {

        PV.RPC(nameof(RPC_GetKill),PV.Owner);
    
    }

    [PunRPC]
    void RPC_GetKill() {

        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills",kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    
    }
    public static PlayerManager Find(Player player) {

        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    
    }
}

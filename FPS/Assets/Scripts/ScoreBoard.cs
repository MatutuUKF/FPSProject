using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{

    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player, ScoreBoardItem> scoreBoardItems = new Dictionary<Player, ScoreBoardItem>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList) {

            AddScoreBoardItem(player);
        
        }
    }

   

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreBoardItem((Player)newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreBoard(otherPlayer);
    }

    void RemoveScoreBoard(Player player) {
        Destroy(scoreBoardItems[player].gameObject);
        scoreBoardItems.Remove(player);
    }
 void AddScoreBoardItem(Player player) {

        ScoreBoardItem item = Instantiate(scoreBoardItemPrefab, container).GetComponent<ScoreBoardItem>();
        item.Initialize(player);
        scoreBoardItems[player] = item;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {

            canvasGroup.alpha = 1f;

        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            
            canvasGroup.alpha = 0f;
        } 
    }
}

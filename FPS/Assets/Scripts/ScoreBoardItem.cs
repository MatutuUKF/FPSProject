using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    Player player;

    public void Initialize(Player player) {
        this.player = player;
        usernameText.text = player.NickName;
        UpdateStats();
    }
    void UpdateStats() {


        if (player.CustomProperties.TryGetValue("kills", out object kills)) {

            killsText.text = kills.ToString();
        
        }
        if (player.CustomProperties.TryGetValue("deaths", out object deaths))
        {

            deathsText.text = deaths.ToString();

        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == player) {

            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths")) {
                UpdateStats();
            }
        
        }    
    }
}

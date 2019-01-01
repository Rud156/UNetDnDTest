using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UNetUI.Extras;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public Button joinButton;
    public Text playerName;
    public Text joinButtonText;

    private Transform _lobbyPlayerHolder;

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        _lobbyPlayerHolder = GameObject.FindGameObjectWithTag(TagManager.LobbyPlayerHolder)?.transform;
        transform.SetParent(_lobbyPlayerHolder);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
            SetupLocalPlayer();
        else
            SetupOtherPlayer();
    }

    // Called From External Button
    public void OnJoinButtonClick() => SendReadyToBeginMessage();

    private void SetupLocalPlayer()
    {
        if (!isLocalPlayer)
            return;

        playerName.text = "Player";
        joinButton.enabled = true;
        joinButtonText.text = "JOIN";
    }

    private void SetupOtherPlayer()
    {
        playerName.text = "Other Player";
        joinButton.enabled = false;
        joinButtonText.text = "...";
    }
}
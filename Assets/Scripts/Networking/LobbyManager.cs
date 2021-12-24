using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public NetworkObject ServerPlayer;

    public List<BeardedManStudios.Forge.Networking.NetworkingPlayer> BlueTeam = new List<BeardedManStudios.Forge.Networking.NetworkingPlayer>();
    public List<BeardedManStudios.Forge.Networking.NetworkingPlayer> RedTeam = new List<BeardedManStudios.Forge.Networking.NetworkingPlayer>();
        
    
    public Transform RedSpawn;
    public Transform BlueSpawn;

    public GameObject RedFlag;
    public GameObject BlueFlag;

    public int blueScore = 0;
    public int redScore = 0;

    private void Awake() {
        Instance = this;
    }

    IEnumerator Start() 
    {
        while(NetworkManager.Instance == null) yield return null;
        while(NetworkManager.Instance.Networker.IsConnected == false) yield return null;

        if(NetworkManager.Instance.Networker.IsServer)
        {
            NetworkManager.Instance.Networker.playerDisconnected += (BeardedManStudios.Forge.Networking.NetworkingPlayer player, NetWorker networker) => {
                var redTeamIndex = RedTeam.IndexOf(player);
                var blueTeamIndex = BlueTeam.IndexOf(player);
                if(redTeamIndex != -1)
                {
                    RedTeam.RemoveAt(redTeamIndex);
                }
                else if(blueTeamIndex != -1)
                {
                    BlueTeam.RemoveAt(blueTeamIndex);
                }
            };
        }
    }

    public char GetTeam(BeardedManStudios.Forge.Networking.NetworkingPlayer player)
    {
        if(RedTeam.Contains(player))
        {
            return 'R';
        }
        else if(BlueTeam.Contains(player))
        {
            return 'B';
        }
        else
        {
            return 'N';
        }
    }

    public void AddNewPlayer(char team, BeardedManStudios.Forge.Networking.Generated.NetworkedPlayerBehavior networkedPlayer)
    {
        if(team == 'R')
        {
            RedTeam.Add(networkedPlayer.networkObject.Owner);
            networkedPlayer.gameObject.transform.position = RedSpawn.position;
            networkedPlayer.gameObject.transform.rotation = RedSpawn.rotation;
            Debug.Log("Spawning Red Player");
        }
        else
        {
            BlueTeam.Add(networkedPlayer.networkObject.Owner);
            networkedPlayer.gameObject.transform.position = BlueSpawn.position;
            networkedPlayer.gameObject.transform.rotation = BlueSpawn.rotation;
            Debug.Log("Spawning Blue Player");
        }
    }

    public void BlueScored()
    {
        blueScore++;
        GameObject.Find("b_score_text").GetComponent<TextMeshProUGUI>().text = blueScore.ToString();
    }

    public void RedScored()
    {
        redScore++;
        GameObject.Find("r_score_text").GetComponent<TextMeshProUGUI>().text = redScore.ToString();
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}

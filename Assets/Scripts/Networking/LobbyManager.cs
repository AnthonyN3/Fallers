using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public NetworkObject ServerPlayer;

    public List<BeardedManStudios.Forge.Networking.NetworkingPlayer> BlueTeam = new List<BeardedManStudios.Forge.Networking.NetworkingPlayer>();
    public List<BeardedManStudios.Forge.Networking.NetworkingPlayer> RedTeam = new List<BeardedManStudios.Forge.Networking.NetworkingPlayer>();
        
    
    public Transform RedSpawn;
    public Transform BlueSpawn;

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
}

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

    public List<uint> BlueTeam = new List<uint>();
    public List<uint> RedTeam = new List<uint>();
        
    
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
            NetworkManager.Instance.Networker.playerDisconnected += (player, sender) =>
            {
                MainThreadManager.Run(() =>
                {
                    //Loop through all players and find the player who disconnected, store all it's networkobjects to a list
                    List<NetworkObject> toDelete = new List<NetworkObject>();
                    foreach (var no in sender.NetworkObjectList)
                    {
                        if (no.Owner == player)
                        {
                            //Found him
                            toDelete.Add(no);
                        }
                    }

                    //Remove the actual network object outside of the foreach loop, as we would modify the collection at runtime elsewise. (could also use a return, too late)
                    if (toDelete.Count > 0)
                    {
                        for (int i = toDelete.Count - 1; i >= 0; i--)
                        {
                            // Remove that players id from the list
                            BlueTeam.Remove(toDelete[i].NetworkId);
                            RedTeam.Remove(toDelete[i].NetworkId);
                            
                            sender.NetworkObjectList.Remove(toDelete[i]);
                            toDelete[i].Destroy();
                        }
                    }
                });
            };
        }
    }

    public char GetTeam(uint playerNetworkId)
    {
        if(RedTeam.Contains(playerNetworkId))
        {
            return 'R';
        }
        else if(BlueTeam.Contains(playerNetworkId))
        {
            return 'B';
        }
        else
        {
            return 'N';
        }
    }

    public void AddNewPlayer(char team, NetworkingPlayer player, uint playerNetworkId)
    {
        if(team == 'R')
        {
            RedTeam.Add(playerNetworkId);
            player.transform.position = RedSpawn.position;
            player.transform.rotation = RedSpawn.rotation;
            Debug.Log("Spawning Red Player");
        }
        else
        {
            BlueTeam.Add(playerNetworkId);
            player.transform.position = BlueSpawn.position;
            player.transform.rotation = BlueSpawn.rotation;
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

    public void PrintList()
    {
        foreach(var id in BlueTeam)
            Debug.Log("blue: " + id);
        foreach(var id in RedTeam)
            Debug.Log("red: " + id);
    }
}

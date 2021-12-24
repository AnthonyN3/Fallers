using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using System.Collections;

public class ExampleProximitySpawn : MonoBehaviour
{
	private IEnumerator Start()
	{
		Debug.Log("Waiting for network manager");
		while(NetworkManager.Instance == null) yield return null;
		Debug.Log("Network manager found");
		Debug.Log("Waiting for networker to be connected");
		while(NetworkManager.Instance.Networker.IsConnected == false) yield return null;
		Debug.Log("Networker connected");

		NetworkManager.Instance.InstantiateNetworkedPlayer(0, transform.position, transform.rotation, true);
	}
}
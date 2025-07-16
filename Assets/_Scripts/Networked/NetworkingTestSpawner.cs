using Unity.Netcode;
using UnityEngine;

public class NetworkingTestSpawner : NetworkBehaviour
{
    [SerializeField] GameObject gameObjectPref;
    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            GameObject temp = Instantiate(gameObjectPref);
            temp.GetComponent<NetworkObject>().Spawn();
        }
    }
}

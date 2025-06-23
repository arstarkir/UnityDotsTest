using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button serverBtn;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");
            NetworkManager.Singleton.StartHost();
        });

        clientBtn.onClick.AddListener(() =>
        {
            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetConnectionData("127.0.0.1", 7777, "0.0.0.0");
            NetworkManager.Singleton.StartClient();
        });

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
    }
}

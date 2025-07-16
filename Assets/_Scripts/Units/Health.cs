using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float maxHealth = 100;
    public NetworkVariable<float> curHealth = new NetworkVariable<float>(100);
    public float regenSpeed = 5;
    public float regenDelayTime = 3;
    public float timeSinceDmg = 0;

    private void Update()
    {
        if(IsOwner)
            RequestPassiveReganServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestDealDmgServerRpc(float amount)
    {
        curHealth.Value -= amount;
        if(curHealth.Value <= 0)
        {
            this.transform.GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
        }
        timeSinceDmg = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestHealServerRpc(float amount)
    {
        curHealth.Value += amount;
        if(curHealth.Value > maxHealth)
            curHealth.Value = maxHealth;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestPassiveReganServerRpc()
    {
        timeSinceDmg += Time.deltaTime;
        if (timeSinceDmg >= regenDelayTime)
            return;

        if (curHealth.Value <= maxHealth)
            curHealth.Value += regenSpeed * Time.deltaTime;
    }
}

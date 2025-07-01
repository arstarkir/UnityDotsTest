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
        timeSinceDmg += Time.deltaTime;
        if (timeSinceDmg >= regenDelayTime )
            return;

        if (curHealth.Value <= maxHealth)
            curHealth.Value += regenSpeed * Time.deltaTime;
    }

    public void DealDmg(float amount)
    {
        curHealth.Value -= amount;
        if(curHealth.Value <= 0)
        {
            this.transform.GetComponent<NetworkObject>().Despawn();
            Destroy(this.gameObject);
        }
        timeSinceDmg = 0;
    }

    public void Heal(float amount)
    {
        curHealth.Value += amount;
        if(curHealth.Value > maxHealth)
            curHealth.Value = maxHealth;
    }
}

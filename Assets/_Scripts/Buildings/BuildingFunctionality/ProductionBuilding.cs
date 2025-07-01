using System.Collections;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ProductionBuilding : CoreBuilding
{
    public ResTypes rType;
    public AnimationCurve prodOverTime;
    public float prodTime;
    float curTime = 0;
    PlayerResources player;

    void Start()
    {
        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            player = FindAnyObjectByType<PlayerResources>();
            StartCoroutine(Produce());
        }
    }

    IEnumerator Produce()
    {
        while (true)
        {
            yield return new WaitForSeconds(prodTime);
            curTime += prodTime;
            player.AddRes(MathF.Round(prodOverTime.Evaluate(curTime),2), rType);
        }
    }

    private void OnDestroy()
    {
        if(this.enabled)
            StopAllCoroutines();
    }
}

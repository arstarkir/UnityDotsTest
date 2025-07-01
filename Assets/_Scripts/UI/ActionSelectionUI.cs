using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject actionSelection;
    [SerializeField] GameObject actionButtonPref;
    [SerializeField] GameObject player;
    List<string> shortcuts = new List<string>();

    UnitRegister unitRegister;
    PlayerResources pResources;
    
    List<GameObject> curActions = new List<GameObject>();

    void Start()
    {
        pResources = player.GetComponent<PlayerResources>();
        unitRegister = Resources.Load<UnitRegister>("SO/MainUnitRegister");
    }

    public void PopulateActionUI(List<int> unitId, List<BuildingAction> bAction)
    {
        ClearActions();
        for (int i = 0; i < unitId.Count; i++) 
        {
            GameObject temp = Instantiate(actionButtonPref,actionSelection.transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200 + 125 * i, 0);
            temp.GetComponent<Image>().sprite = unitRegister.unitDatas[unitId[i]].sprite;
            ButtonActionData bAD = temp.AddComponent<ButtonActionData>();
            bAD.unitID = unitId[i];
            temp.GetComponent<Button>().onClick.AddListener(() => UnitSelected(bAD, bAction));
            curActions.Add(temp);
        }
    }

    public void ClearActions()
    {
        foreach (GameObject temp in curActions)
            Destroy(temp);
        curActions.Clear();
    }

    public void UnitSelected(ButtonActionData bAD, List<BuildingAction> bAction)
    {
        foreach (BuildingAction bA in bAction)
        {
            UnitBuilding uB = bA.gameObject.GetComponent<UnitBuilding>();
            if (pResources.HasRes(unitRegister.unitDatas[bAD.unitID].cost))
            {
                pResources.PayCost(unitRegister.unitDatas[bAD.unitID].cost);
                uB.RequestRequestSpawnUnit(bAD.unitID);
            }
        }
        //player.GetComponent<Builder>().buildID = buildingID;
    }
}

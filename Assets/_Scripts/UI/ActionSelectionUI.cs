using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject actionSelection;
    [SerializeField] GameObject actionButtonPref;
    [SerializeField] GameObject player;
    List<string> shortcuts = new List<string>();

    List<GameObject> curActions = new List<GameObject>();
    public void PopulateActionUI(List<int> unitId, BuildingAction bAction)
    {
        ClearActions();
        UnitRegister unitRegister = Resources.Load<UnitRegister>("SO/MainUnitRegister");
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

    public void UnitSelected(ButtonActionData bAD, BuildingAction bAction)
    {
        bAction.gameObject.GetComponent<UnitBuilding>().RequestRequestSpawnUnit(bAD.unitID);
        //player.GetComponent<Builder>().buildID = buildingID;
    }
}

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
    public void PopulateActionUI(List<int> unitId)
    {
        ClearActions();
        UnitRegister unitRegister = Resources.Load<UnitRegister>("SO/MainUnitRegister");
        for (int i = 0; i < unitId.Count; i++) 
        {
            GameObject temp = Instantiate(actionButtonPref,actionSelection.transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(-200 + 125 * i, 0);
            temp.GetComponent<Image>().sprite = unitRegister.unitDatas[unitId[i]].sprite;
            temp.GetComponent<Button>().onClick.AddListener(() =>  UnitSelected(0));
            curActions.Add(temp);
        }
    }

    public void ClearActions()
    {
        foreach (GameObject temp in curActions)
            Destroy(temp);
        curActions.Clear();
    }

    public void UnitSelected(int unitID)
    {
        //player.GetComponent<Builder>().buildID = buildingID;
    }
}

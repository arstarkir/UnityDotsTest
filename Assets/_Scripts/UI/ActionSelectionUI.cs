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
            GameObject temp = Instantiate(actionButtonPref,
                new Vector3(200, 0, 0) + i*(new Vector3(-125,0,0)),Quaternion.identity,actionSelection.transform);
            temp.transform.position = new Vector3 (temp.transform.position.x, 0,0);
            temp.GetComponent<Image>().sprite = unitRegister.unitDatas[unitId[i]].sprite;
            curActions.Add(temp);
            //temp.GetComponent<Button>().onClick.AddListener();
        }
    }

    public void ClearActions()
    {
        foreach (GameObject temp in curActions)
            Destroy(temp);
        curActions.Clear();
    }

    //public void BuildingSelected(int buildingID)
    //{
    //    player.GetComponent<Builder>().buildID = buildingID;
    //}
}

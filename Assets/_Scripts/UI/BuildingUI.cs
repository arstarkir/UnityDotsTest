using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    [SerializeField] GameObject typeSelection;
    [SerializeField] GameObject rSelection;
    [SerializeField] GameObject mSelection;
    [SerializeField] GameObject htSelection;
    [SerializeField] GameObject player;

    private void Start()
    {
        OpenTypeS();
    }
    
    private void Update()
    {
        Shortcuts();
    }

    public void OpenTypeS()
    {
        typeSelection.SetActive(true);
        rSelection.SetActive(false);
        mSelection.SetActive(false);
        htSelection.SetActive(false);
    }

    public void OpenRS()
    {
        typeSelection.SetActive(false);
        rSelection.SetActive(true);
        mSelection.SetActive(false);
        htSelection.SetActive(false);
    }
    
    public void OpenMS()
    {
        typeSelection.SetActive(false);
        rSelection.SetActive(false);
        mSelection.SetActive(true);
        htSelection.SetActive(false);
    }

    public void OpenHTS()
    {
        typeSelection.SetActive(false);
        rSelection.SetActive(false);
        mSelection.SetActive(false);
        htSelection.SetActive(true);
    }

    public void BuildingSelected(int buildingID)
    {
        player.GetComponent<Builder>().buildID = buildingID;
    }

    void Shortcuts()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(typeSelection.activeSelf)
                OpenRS();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (typeSelection.activeSelf)
                OpenMS();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (typeSelection.activeSelf)
                OpenHTS();
        }
    }
}

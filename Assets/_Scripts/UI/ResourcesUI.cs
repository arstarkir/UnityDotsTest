using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] GameObject mCounter;
    [SerializeField] GameObject fCounter;
    [SerializeField] GameObject pCounter;
    [SerializeField] GameObject mCurAmount;
    [SerializeField] GameObject fCurAmount;
    [SerializeField] GameObject pCurAmount;

    public void UpdateMaxRes(float maxRes, ResTypes rType)
    {
        if(rType == ResTypes.Minirals)
        {
            mCounter.GetComponent<Slider>().maxValue = maxRes;
        }
        if (rType == ResTypes.Food)
        {
            fCounter.GetComponent<Slider>().maxValue = maxRes;
        }
        if (rType == ResTypes.Power)
        {
            pCounter.GetComponent<Slider>().maxValue = maxRes;
        }
    }

    public void UpdateCurRes(float amount, ResTypes rType)
    {
        if (rType == ResTypes.Minirals)
        {
            mCounter.GetComponent<Slider>().value = amount;
            mCurAmount.GetComponent<TMP_Text>().text = amount.ToString();
        }
        if (rType == ResTypes.Food)
        {
            fCounter.GetComponent<Slider>().value = amount;
            fCurAmount.GetComponent<TMP_Text>().text = amount.ToString();
        }
        if (rType == ResTypes.Power)
        {
            pCounter.GetComponent<Slider>().value = amount;
            pCurAmount.GetComponent<TMP_Text>().text = amount.ToString();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour
{
    [SerializeField] GameObject mCounter;
    [SerializeField] GameObject fCounter;
    [SerializeField] GameObject pCounter;

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
        }
        if (rType == ResTypes.Food)
        {
            fCounter.GetComponent<Slider>().value = amount;
        }
        if (rType == ResTypes.Power)
        {
            pCounter.GetComponent<Slider>().value = amount;
        }
    }
}

using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public class CameraControls : MonoBehaviour
{
    [SerializeField] AnimationCurve scrollStrengthCurve;
    float scrolingTime = 0;
    [SerializeField] float borderSize = 3;

    [HideInInspector] public GameObject active;

    private void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (!world.IsClient())
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void OnGUI()
    {
        transform.position -= Vector3.up * Input.GetAxis("Mouse ScrollWheel");
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (!Application.isFocused)
            return;

        EditorWindow fw = EditorWindow.focusedWindow;
        if (fw == null || fw.GetType().Name != "GameView")
            return;
#endif

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mousePos.y;
        mousePos.y = 0;

        mousePos.x -= Screen.width / 2;
        mousePos.z -= Screen.height / 2;
        if (Mathf.Abs(mousePos.x) > Screen.width / borderSize || Mathf.Abs(mousePos.z) > Screen.height / borderSize)
        {
            transform.position += mousePos * 0.01f * scrollStrengthCurve.Evaluate(scrolingTime) * Time.deltaTime;
            scrolingTime += Time.deltaTime;
        }
        else
        {
            scrolingTime = 0;
        }
    }
}
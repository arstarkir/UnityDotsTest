using System;
using System.Linq;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class CameraControls : NetworkBehaviour
{
    [SerializeField] AnimationCurve scrollStrengthCurve;
    float scrolingTime = 0;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform[] areas;

    [HideInInspector] public GameObject active;

    private void Start()
    {
        if (!IsOwner)
            gameObject.SetActive(false);
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (!e.capsLock)
            transform.position -= Vector3.up * 10 * Input.GetAxis("Mouse ScrollWheel");
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

        if (!IsMouseInsideAny())
        {
            transform.position += mousePos * 0.01f * scrollStrengthCurve.Evaluate(scrolingTime) * Time.deltaTime;
            scrolingTime += Time.deltaTime;
        }
        else
        {
            scrolingTime = 0;
        }
    }

    public bool IsMouseInsideAny()
    {
        Vector2 mouse = Input.mousePosition;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay? null : canvas.worldCamera;

        return areas.Any(rt =>RectTransformUtility.RectangleContainsScreenPoint(rt, mouse, cam));
    }
}
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public List<ulong> SelectedIds => _ids;
    readonly List<ulong> _ids = new();

    Camera _cam;
    Vector2 _start;
    bool _drag;

    void Awake() => _cam = Camera.main;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            _start = Input.mousePosition;
            _drag = false;
        }

        if (Input.GetMouseButton(0))
            _drag |= (Input.mousePosition - (Vector3)_start).sqrMagnitude > 4f;

        if (Input.GetMouseButtonUp(0))
        {
            if (_drag) BoxSelect(RectFrom(_start, Input.mousePosition));
            else ClickSelect(Input.mousePosition);
        }
    }

    void ClickSelect(Vector2 screen)
    {
        Ray ray = _cam.ScreenPointToRay(screen);
        if (!Physics.Raycast(ray, out var hit)) return;

        FlowFollower unit = hit.collider.transform.GetComponent<FlowFollower>();
        if (unit == null) return;
        if (!unit.IsOwner) return;

        ClearAll();
        Add(unit);
        Debug.Log(unit.gameObject.name);
    }

    void BoxSelect(Rect box)
    {
        ClearAll();

        foreach (var obj in NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(NetworkManager.Singleton.LocalClientId))
        {
            if (!obj.TryGetComponent(out FlowFollower unit)) 
                continue;

            Vector2 sp = _cam.WorldToScreenPoint(unit.transform.position);
            if (box.Contains(sp))
            {
                Add(unit);
                Debug.Log(unit.gameObject.name);
            } 
        }
    }

    void Add(FlowFollower u)
    {
        _ids.Add(u.NetworkObjectId);
        ToggleRing(u, true);
    }

    void ClearAll()
    {
        foreach (ulong id in _ids)
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects
                                         .TryGetValue(id, out var o) &&
                o.TryGetComponent(out FlowFollower u))
                ToggleRing(u, false);

        _ids.Clear();
    }

    static void ToggleRing(Component c, bool on)
    {
        Transform ring = c.transform.Find("SelectionRing");
        if (ring) 
            ring.gameObject.SetActive(on);
    }

    static Rect RectFrom(Vector2 a, Vector2 b) =>
        new Rect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y),
                 Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
}

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

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
        ClearAll();

        BuildingSelect(screen);
        if(_ids.Count == 0)
            UnitSelect(screen);
    }

    void UnitSelect(Vector2 screen)
    {
        Ray ray = _cam.ScreenPointToRay(screen);
        if (!Physics.Raycast(ray, out var hit)) return;

        FlowFollower unit = hit.collider.transform.GetComponent<FlowFollower>();
        if (unit == null) return;
        if (!unit.IsOwner) return;

        Add(unit.GetComponent<NetworkObject>());
        Debug.Log(unit.gameObject.name);
    }

    void BuildingSelect(Vector2 screen)
    {
        Ray ray = _cam.ScreenPointToRay(screen);
        if (!Physics.Raycast(ray, out var hit)) return;

        CoreBuilding building = hit.collider.transform.GetComponent<UnitBuilding>();
        if (building == null) 
            return;
        if (!building.IsOwner)
            return;

        Add(building.GetComponent<NetworkObject>());
        Debug.Log(building.gameObject.name);
    }

    void BoxSelect(Rect box)
    {
        ClearAll();

        foreach (var obj in NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(NetworkManager.Singleton.LocalClientId))
        {
            if (obj.TryGetComponent(out CoreBuilding building))
            {
                if(!HasSelectedIdsBuilding())
                    ClearAll();
                BuildingBoxSelect(box, building.transform);
                continue;
            }

            if (!HasSelectedIdsBuilding() && obj.TryGetComponent(out FlowFollower unit))
            {
                UnitBoxSelect(box, unit.transform);
                continue;
            }
        }
    }

    void UnitBoxSelect(Rect box, Transform tr)
    {
        Vector2 sp = _cam.WorldToScreenPoint(tr.position);
        if (box.Contains(sp))
        {
            Add(tr.GetComponent<NetworkObject>());
            Debug.Log(tr.gameObject.name);
        }
    }

    void BuildingBoxSelect(Rect box, Transform tr)
    {
        Vector2 sp = _cam.WorldToScreenPoint(tr.position);
        if (box.Contains(sp))
        {
            Add(tr.GetComponent<NetworkObject>());
            Debug.Log(tr.gameObject.name);
        }
    }

    void Add(NetworkObject nb)
    {
        _ids.Add(nb.NetworkObjectId);
        ToggleRing(nb, true);
    }

    void ClearAll()
    {
        foreach (ulong id in _ids)
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var o) && o.TryGetComponent(out NetworkObject u))
            {
                ToggleRing(u, false);
            }

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

    public List<ulong> GetUnitSelectedIds()
    {
        return _ids.FindAll(id => NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].TryGetComponent<FlowFollower>(out FlowFollower ff));
    }

    public List<ulong> GetBuildingsSelectedIds()
    {
        return _ids.FindAll(id => NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].TryGetComponent<CoreBuilding>(out CoreBuilding cb));
    }

    public bool HasSelectedIdsBuilding()
    {
         return 0 != _ids.FirstOrDefault(id => NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].TryGetComponent<CoreBuilding>(out CoreBuilding cb));
    }
}

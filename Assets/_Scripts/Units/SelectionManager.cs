using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : NetworkBehaviour
{
    [Header("UI")]
    [Tooltip("An Image with a transparent sprite whose RectTransform you want to stretch while dragging.")]
    public Image dragRect;

    [Header("Input")]
    public KeyCode addKey = KeyCode.LeftShift;
    public KeyCode toggleKey = KeyCode.LeftControl;

    public List<ulong> CurrentUnitIds => _selectedIds;
    readonly List<ulong> _selectedIds = new();

    Camera _cam;
    Vector2 _dragStartScreen;
    bool _isDragging;

    static readonly Rect _empty = new Rect();

    void Awake()
    {
        _cam = Camera.main;
        if (dragRect != null) dragRect.gameObject.SetActive(false);
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _dragStartScreen = Input.mousePosition;
            _isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            float sqrDrag = (Input.mousePosition - (Vector3)_dragStartScreen).sqrMagnitude;
            if (!_isDragging && sqrDrag > 4f)
            {
                _isDragging = true;
                if (dragRect != null) dragRect.gameObject.SetActive(true);
            }

            if (_isDragging && dragRect != null)
                UpdateDragRect(_dragStartScreen, Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_isDragging)
            {
                SelectByRectangle(ScreenRect(_dragStartScreen, Input.mousePosition));
            }
            else
            {
                SelectByClick(Input.mousePosition);
            }

            if (dragRect != null)
            {
                dragRect.rectTransform.anchoredPosition = Vector2.zero;
                dragRect.rectTransform.sizeDelta = Vector2.zero;
                dragRect.gameObject.SetActive(false);
            }
        }
    }

    void SelectByClick(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out var hit)) return;

        FlowFollower unit = hit.collider.GetComponentInParent<FlowFollower>();
        if (unit == null || !unit.IsOwner) return;

        bool add = Input.GetKey(addKey);
        bool toggle = Input.GetKey(toggleKey);

        ulong id = unit.NetworkObjectId;

        if (toggle)
        {
            if (_selectedIds.Contains(id)) Deselect(unit, id);
            else AddSelect(unit, id);
        }
        else
        {
            if (!add) ClearAll();
            AddSelect(unit, id);
        }
    }

    void SelectByRectangle(Rect screenRect)
    {
        bool add = Input.GetKey(addKey);
        bool toggle = Input.GetKey(toggleKey);

        if (!add && !toggle) ClearAll();

        foreach (NetworkObject obj in NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(OwnerClientId))
        {
            if (!obj.TryGetComponent(out FlowFollower unit)) continue;

            Vector2 sp = _cam.WorldToScreenPoint(unit.transform.position);
            bool inside = screenRect.Contains(sp);

            ulong id = obj.NetworkObjectId;

            if (toggle)
            {
                if (inside)
                {
                    if (_selectedIds.Contains(id)) Deselect(unit, id);
                    else AddSelect(unit, id);
                }
            }
            else if (inside)
            {
                AddSelect(unit, id);
            }
        }
    }

    void AddSelect(FlowFollower unit, ulong id)
    {
        if (_selectedIds.Contains(id)) return;

        _selectedIds.Add(id);
        SetHighlight(unit, true);
    }

    void Deselect(FlowFollower unit, ulong id)
    {
        if (!_selectedIds.Remove(id)) return;

        SetHighlight(unit, false);
    }

    void ClearAll()
    {
        foreach (ulong id in _selectedIds)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(id, out var obj) &&
                obj.TryGetComponent(out FlowFollower u))
            {
                SetHighlight(u, false);
            }
        }
        _selectedIds.Clear();
    }

    static void SetHighlight(Component unit, bool on)
    {
        Transform ring = unit.transform.Find("SelectionRing");
        Debug.Log("Name is " + unit.gameObject.name + " " + ring != null);
        if (ring != null) 
            ring.gameObject.SetActive(on);
    }

    static Rect ScreenRect(Vector2 p1, Vector2 p2)
    {
        float xMin = Mathf.Min(p1.x, p2.x);
        float yMin = Mathf.Min(p1.y, p2.y);
        float xMax = Mathf.Max(p1.x, p2.x);
        float yMax = Mathf.Max(p1.y, p2.y);
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    void UpdateDragRect(Vector2 start, Vector2 end)
    {
        Rect r = ScreenRect(start, end);

        var rt = dragRect.rectTransform;
        rt.anchoredPosition = r.position;
        rt.sizeDelta = r.size;
    }
}

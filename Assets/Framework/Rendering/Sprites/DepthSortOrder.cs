using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class DepthSortOrder : MonoBehaviour
{

    public static float ScaleY = 100;

    public int SortingOrderOffset;
    public bool EnabledAtRuntime = false;
    public bool EnabledInEditor = false;
    public bool UpdateAtRuntime = false;
    public bool ApplyToChildren = false;

    private Renderer _renderer;
    private bool _dirty = true;
    private bool _playing = false;

    private List<Renderer> _children;
    private List<int> _offsets;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _playing = Application.isPlaying;

        // Locate child sorters if needed.
        if (ApplyToChildren)
        {
            _children = new List<Renderer>(
                GetComponentsInChildren<Renderer>()
                .Where(x => x.transform != transform));

            _offsets = new List<int>();
            foreach (var child in _children)
                _offsets.Add(child.sortingOrder);
        }
    }

    void OnEnable()
    {
        _dirty = true;
    }

    void Update()
    {
        if (_playing && !EnabledAtRuntime)
            return;
        if (!_playing && !EnabledInEditor)
            return;

        if (_dirty)
            UpdateSortingOrder();
    }

    private void UpdateSortingOrder()
    {
        if (_renderer != null)
            _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * ScaleY) + SortingOrderOffset;

        if (_playing && ApplyToChildren)
            UpdateChildSortingOrders(_renderer.sortingOrder);

        _dirty = !_playing || UpdateAtRuntime;
    }

    private void UpdateChildSortingOrders(int order)
    {
        var n = _children.Count;
        for (var i = 0; i < n; i++)
            _children[i].sortingOrder = order + _offsets[i];
    }

}

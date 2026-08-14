using Alchemy.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[AlchemySerialize]
public partial class VirtualMarbleGrid : MonoBehaviour
{

    public int ActiveCount => _activeCells.Count;
    public int CountInPool => _pool.CountInactive;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform viewport;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] Vector2 cellSize = new Vector2(397, 1027);
    [SerializeField] Vector2 spacing = new Vector2(10, 10);

    public List<MarbleDetail> _data = new List<MarbleDetail>();
    private ObjectPool<GameObject> _pool;


    public Dictionary<int, GameObject> _activeCells = new Dictionary<int, GameObject>();

    private int _columns;
    private float _prefabHeight;
    private float _prefabWidth;
    private float _startX;
    private float _contentHeight;
    private float _viewportHeight;

    private int _currentMaxSize = 10;

    private void OnEnable()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);

        // If data was assigned while this object was disabled, build it now that we are enabled.
        if (_data.Count > 0)
        {
            RebuildGrid();
        }
    }

    private void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(OnScroll);
        ClearActiveCells();
    }

    private void EnsurePoolInitialized(int desiredMaxSize)
    {
        // If the pool exists and the size hasn't changed, do nothing
        if (_pool != null && _currentMaxSize == desiredMaxSize) return;

        // If the pool exists but the screen rotated/resized, clear the old pool first
        if (_pool != null)
        {
            ClearActiveCells();
            _pool.Clear(); // Empties the pool
        }

        _currentMaxSize = desiredMaxSize;

        _pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                Debug.Log($"[Pool] CREATE (total created so far)");
                return Instantiate(tilePrefab);
            },
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
                Debug.Log($"[Pool] GET / ADD active. Active count after = {_activeCells.Count + 1}");
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
                Debug.Log($"[Pool] RELEASE. Active count before = {_activeCells.Count}");
            },
            actionOnDestroy: (obj) =>
            {
                Debug.Log("[Pool] DESTROY (overflowed maxSize)");
                Destroy(obj);
            },
            collectionCheck: true,
            defaultCapacity: desiredMaxSize, // Pre-allocate memory for this many
            maxSize: desiredMaxSize
        );
    }

    // 1. Called by MarbleFilterationController
    public void InitializeGrid(List<MarbleDetail> marbleDetails)
    {
        // Just store the data. 
        _data = marbleDetails != null ? marbleDetails : new List<MarbleDetail>();

        // If the UI is currently visible, rebuild immediately.
        if (gameObject.activeInHierarchy)
        {
            RebuildGrid();
        }
    }

    // 2. Does the actual clearing and layout calculation
    private void RebuildGrid()
    {
        if (_pool != null) ClearActiveCells();

        // Force the Content RectTransform to Top-Center
        scrollRect.content.anchorMin = new Vector2(0.5f, 1f);
        scrollRect.content.anchorMax = new Vector2(0.5f, 1f);
        scrollRect.content.pivot = new Vector2(0.5f, 1f);

        // Destroy any layout groups
        if (scrollRect.content.TryGetComponent(out LayoutGroup lg)) Destroy(lg);
        if (scrollRect.content.TryGetComponent(out ContentSizeFitter csf)) Destroy(csf);

        scrollRect.normalizedPosition = new Vector2(0, 1);

        _prefabWidth = cellSize.x + spacing.x;
        _prefabHeight = cellSize.y + spacing.y;

        LayoutRebuilder.ForceRebuildLayoutImmediate(viewport);
        float viewportWidth = viewport.rect.width;
        _viewportHeight = viewport.rect.height;

        // Fallback if width is still 0 for some reason
        if (viewportWidth <= 0) viewportWidth = 1000;

        _columns = Mathf.Max(1, Mathf.FloorToInt(viewportWidth / _prefabWidth));

        int visibleRows = Mathf.CeilToInt(_viewportHeight / _prefabHeight) + 1; // +1 for partial rows
        int bufferSize = _columns * 3; // 2 extra rows for smooth fast scrolling
        int calculatedMaxSize = (visibleRows * _columns) + bufferSize;

        // 2. Initialize the pool with the correct size!
        EnsurePoolInitialized(calculatedMaxSize);
        // ---------------------------------


        // Handle empty data
        if (_data.Count == 0)
        {
            _contentHeight = _viewportHeight;
            scrollRect.content.sizeDelta = new Vector2(viewportWidth, _contentHeight);
            return;
        }


        int rows = Mathf.CeilToInt((float)_data.Count / _columns);
        _contentHeight = (rows * _prefabHeight) + spacing.y;
        scrollRect.content.sizeDelta = new Vector2(viewportWidth, _contentHeight);

        float totalGridWidth = _columns * _prefabWidth - spacing.x;
        _startX = -totalGridWidth / 2f + (cellSize.x / 2f);

        UpdateVisibleCells();
    }

    private void ClearActiveCells()
    {
        if (_activeCells.Count == 0) return;

        // Release all active cells back to the pool
        List<GameObject> cellsToRelease = new List<GameObject>(_activeCells.Values);
        foreach (var cell in cellsToRelease)
        {
            _pool.Release(cell);
        }
        _activeCells.Clear();
    }

    private void OnScroll(Vector2 pos)
    {
        UpdateVisibleCells();
    }

    private void UpdateVisibleCells()
    {
        if (_data == null || _data.Count == 0) return;
        if (_pool == null) return;

        float maxScroll = Mathf.Max(0, _contentHeight - _viewportHeight);
        float yOffset = (1f - scrollRect.normalizedPosition.y) * maxScroll;
        yOffset = Mathf.Clamp(yOffset, 0, maxScroll);

        int firstVisibleRow = Mathf.FloorToInt(yOffset / _prefabHeight);
        int lastVisibleRow = Mathf.CeilToInt((yOffset + _viewportHeight) / _prefabHeight);

        int maxRows = Mathf.CeilToInt((float)_data.Count / _columns) - 1;

        firstVisibleRow = Mathf.Clamp(firstVisibleRow, 0, maxRows);
        lastVisibleRow = Mathf.Clamp(lastVisibleRow, 0, maxRows);

        // 1. Release cells that scrolled out of view
        List<int> keysToRemove = new List<int>();
        foreach (var kvp in _activeCells)
        {
            int row = kvp.Key / _columns;
            if (row < firstVisibleRow || row > lastVisibleRow)
            {
                Debug.Log($"RELEASE index={kvp.Key} row={row}");
                _pool.Release(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove) _activeCells.Remove(key);

        // 2. Get/Create cells that just scrolled into view
        int startIdx = firstVisibleRow * _columns;
        int endIdx = Mathf.Min(_data.Count - 1, (lastVisibleRow * _columns) + (_columns - 1));

        for (int i = startIdx; i <= endIdx; i++)
        {
            if (i < 0 || i >= _data.Count) continue;

            GameObject cell;
            if (!_activeCells.ContainsKey(i))
            {
                Debug.Log($"ADD index={i} row={i / _columns} col={i % _columns}");
                cell = _pool.Get();
                cell.transform.SetParent(scrollRect.content, false);
                cell.transform.localScale = Vector3.one;

                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                if (cell.TryGetComponent(out LayoutGroup cellLg)) Destroy(cellLg);
                if (cell.TryGetComponent(out ContentSizeFitter cellCsf)) Destroy(cellCsf);

                ShowMarbleDetails marble = cell.GetComponent<ShowMarbleDetails>();
                marble.UpdateData(_data[i]);

                _activeCells.Add(i, cell);
            }
            else
            {
                cell = _activeCells[i];
            }

            // Force position and size every frame
            RectTransform cellRt = cell.GetComponent<RectTransform>();
            cellRt.sizeDelta = cellSize;

            int row = i / _columns;
            int col = i % _columns;

            float xPos = _startX + (col * _prefabWidth);
            float yPos = -spacing.y - (row * _prefabHeight) - (cellSize.y / 2f);

            cellRt.anchoredPosition = new Vector2(xPos, yPos);
        }
    }
}
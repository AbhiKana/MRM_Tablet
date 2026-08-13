using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

public class VirtualMarbleGrid : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform viewport;
    [SerializeField] GameObject tilePrefab;

    [SerializeField] Vector2 cellSize = new Vector2(397, 1027);
    [SerializeField] Vector2 spacing = new Vector2(10, 10);

    private List<MarbleDetail> _data = new List<MarbleDetail>();
    private ObjectPool<GameObject> _pool;
    private Dictionary<int, GameObject> _activeCells = new Dictionary<int, GameObject>();

    private int _columns;
    private float _prefabHeight;
    private float _prefabWidth;
    private float _startX;
    private float _contentHeight;
    private float _viewportHeight;

    private void OnEnable()
    {
        EnsurePoolInitialized();
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

    private void EnsurePoolInitialized()
    {
        if (_pool == null)
        {
            _pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(tilePrefab),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => Destroy(obj),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false, // Turned off to prevent hard-crashes during filtering, pool logic handles safety now
                defaultCapacity: 20,
                maxSize: 50
            );
        }
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
        EnsurePoolInitialized();
        ClearActiveCells();

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

        // Handle empty data
        if (_data.Count == 0)
        {
            _contentHeight = _viewportHeight;
            scrollRect.content.sizeDelta = new Vector2(viewportWidth, _contentHeight);
            return;
        }

        _columns = Mathf.Max(1, Mathf.FloorToInt(viewportWidth / _prefabWidth));
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

        EnsurePoolInitialized();

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
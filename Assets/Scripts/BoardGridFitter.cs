using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BoardSlotSpawner : MonoBehaviour
{
    public GameObject slotWrapperPrefab;

    public int rows = 5;
    public int minColumns = 5;
    public float cardRatio = 0.714f; // width / height

    GridLayoutGroup grid;
    RectTransform rect;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        BuildGrid();
    }

    void OnRectTransformDimensionsChange()
    {
        if (rect != null && rect.rect.width > 0)
            BuildGrid();
    }

    void BuildGrid()
    {
        ClearSlots();
        float verticalPadding = grid.padding.top + grid.padding.bottom;

        // enforce equal padding on X using Y padding
        grid.padding.left = grid.padding.top;
        grid.padding.right = grid.padding.bottom;

        float width = rect.rect.width
            - grid.padding.left
            - grid.padding.right;

        float height = rect.rect.height
            - grid.padding.top
            - grid.padding.bottom;

        float cellHeight = (height - grid.spacing.y * (rows - 1)) / rows;
        float cellWidth = cellHeight * cardRatio;

        int columns = Mathf.FloorToInt(
            (width + grid.spacing.x) / (cellWidth + grid.spacing.x)
        );

        columns = Mathf.Max(columns, minColumns);
        if (columns % 2 == 0) columns++; // force odd

        float totalWidth = columns * cellWidth + (columns - 1) * grid.spacing.x;
        if (totalWidth > width)
        {
            cellWidth = (width - (columns - 1) * grid.spacing.x) / columns;
            cellHeight = cellWidth / cardRatio;
        }

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.cellSize = new Vector2(cellWidth, cellHeight);

        int totalSlots = rows * columns;
        for (int i = 0; i < totalSlots; i++)
        {
            Instantiate(slotWrapperPrefab, transform);
        }
    }

    void ClearSlots()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}


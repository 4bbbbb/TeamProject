using UnityEngine;

/// <summary>
/// 1. 맵 데이터 생성
/// 2. 청크 생성
/// 3. 각 청크가 자기 구역 타일 생성
/// 4. Building 월드좌표 -> Tile 배열 좌표
/// </summary>

[ExecuteAlways]
public class MapManager : MonoBehaviour
{
    [Header("<< 맵 사이즈 >>")]
    [SerializeField] private int mapWidth = 128;
    [SerializeField] private int mapHeight = 128;
    [SerializeField] private int chunkSize = 16;

    [Header("<< 맵 프리팹 >>")]
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject waterPrefab;

    private TileData[,] mapData;

    private void Start()
    {
        if (Application.isPlaying)
        {
            GenerateMap();
        }
    }

    #region < ContextMenu >
    [ContextMenu("Generate Map")]
    private void GenerateMap()
    {
        ClearMap();
        GenerateMapData();
        RegisterBuildings();
        GenerateChunks();
    }

    [ContextMenu("Clear Map")]
    private void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }
    #endregion

    #region < Map 생성>
    // 1. 맵 데이터 생성
    private void GenerateMapData()
    {
        mapData = new TileData[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                mapData[x, z] = new TileData
                {
                    x = x,
                    z = z,
                    height = 1,
                    tileType = TileType.Grass,
                    buildable = true,
                    occupied = false

                };
            }
        }
    }

    // 2. 청크 생성 -> 3. 청크가 타일 생성
    private void GenerateChunks()
    {
        int chunkCountX = mapWidth / chunkSize;
        int chunkCountZ = mapHeight / chunkSize;

        for (int ccx = 0; ccx < chunkCountX; ccx++)
        {
            for (int ccz = 0; ccz < chunkCountZ; ccz++)
            {
                GameObject chunkObject = new GameObject($"Chunk_{ccx}_{ccz}");
                chunkObject.transform.parent = transform;

                Chunk chunk = chunkObject.AddComponent<Chunk>();
                chunk.Init(ccx, ccz, chunkSize, mapData, grassPrefab, waterPrefab);
                chunk.GenerateChunk();
            }
        }
    }
    #endregion

    #region < 빌딩 배치 관리>
    // 4. 빌딩의 월드 좌표를 타일 배열 좌표로 변경
    private Vector2Int WorldToTile(Vector3 worldPos)
    {
        int tileX = Mathf.RoundToInt(worldPos.x + mapWidth / 2f);
        int tileZ = Mathf.RoundToInt(worldPos.z + mapHeight / 2f);

        return new Vector2Int(tileX, tileZ);
    }

    private void RegisterBuildings()
    {
        Building[] buildings = FindObjectsByType<Building>(FindObjectsSortMode.None);

        foreach (Building building in buildings)
        {
            Vector2Int origin = WorldToTile(building.transform.position);

            for (int x = 0; x < building.Width; x++)
            {
                for (int z = 0; z < building.Height; z++)
                {
                    int tileX = origin.x + x;
                    int tileZ = origin.y + z;

                    if (IsInMap(tileX, tileZ))
                    {
                        mapData[tileX, tileZ].occupied = true;
                        mapData[tileX, tileZ].buildable = false;
                    }
                }
            }
        }
    }

    // 범위 체크
    private bool IsInMap(int x, int z)
    {
        return x >= 0 && x < mapWidth && z >= 0 && z < mapHeight;
    }
    #endregion
}

using UnityEngine;

/// <summary>
/// 1. 맵 데이터 생성
/// 2. 청크 생성
/// 3. 각 청크가 자기 구역 타일 생성
/// 4. 각 객체의 타일 위치 관리
/// 5. Building 관리
/// 6. Tree 관리
/// 
///  *** 건물과 나무 이동 함수 추가하기
/// </summary>

//bool isPlaced;          // 배치됨
//bool isBuilt;           // 건설 완료됨
//bool isUnderConstruction; // 건설 중
//bool isPreviewing;      // 배치 미리보기 중
//bool canPlace;          // 현재 위치에 배치 가능
//bool isOccupied;        // 해당 타일이 점유됨

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
        RegisterInitialBuildings();
        RegisterInitialTrees();
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

    // 청크가 타일 생성
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

    #region < Tile / Bound >

    // 월드 좌표를 타일 배열 좌표로 변경
    private Vector2Int WorldToTile(Vector3 worldPos)
    {
        int tileX = Mathf.RoundToInt(worldPos.x + mapWidth / 2f);
        int tileZ = Mathf.RoundToInt(worldPos.z + mapHeight / 2f);

        return new Vector2Int(tileX, tileZ);
    }

    // 좌표가 맵 범위 내에 있는지 확인
    private bool IsWithinMapBounds(int x, int z)
    {
        return x >= 0 && x < mapWidth && z >= 0 && z < mapHeight;
    }
    #endregion

    #region < 빌딩 배치 관리 >

    // 게임 시작할 때 이미 맵에 배치되어 있는 건물들을 mapData에 등록
    private void RegisterInitialBuildings()
    {
        Building[] buildings = FindObjectsByType<Building>(FindObjectsSortMode.None);

        foreach (Building building in buildings)
        {
            SetBuildingTilesOccupied(building, true); // 이미 점유중인 타일에는 빌딩을 지을 수 없음
        }
    }

    // 특정 건물이 차지하는 타일들의 점유 상태를 설정하는 함수
    private void SetBuildingTilesOccupied(Building building, bool occupied)
    {
        Vector2Int origin = WorldToTile(building.transform.position);

        for (int x = 0; x < building.Width; x++)
        {
            for (int z = 0; z < building.Height; z++)
            {
                int tileX = origin.x + x;
                int tileZ = origin.y + z;

                if (IsWithinMapBounds(tileX, tileZ))
                {
                    mapData[tileX, tileZ].occupied = occupied;
                    mapData[tileX, tileZ].buildable = !occupied;
                }
            }
        }
    }
    #endregion

    #region < 나무 배치 관리 >

    // 게임 시작할 때 이미 맵에 배치되어 있는 나무들을 mapData에 등록
    private void RegisterInitialTrees()
    {
        Tree[] trees = FindObjectsByType<Tree>(FindObjectsSortMode.None);

        foreach (Tree tree in trees)
        {
            Vector2Int tilePos = WorldToTile(tree.transform.position);

            if (!IsWithinMapBounds(tilePos.x, tilePos.y))
            {
                Debug.LogWarning($"{tree.name}가 범위 밖에 있습니다.");
                continue;
            }

            TileData tile = mapData[tilePos.x, tilePos.y];

            if (tile.occupied)
            {
                Debug.LogWarning("새 위치에 이미 오브젝트가 있습니다.");
                continue;
            }

            tile.occupied = true;
            tile.buildable = false;

            tile.hasTree = true;
            tile.tree = tree;

            tree.SetTilePosition(tilePos);
        }
    }
    #endregion
}

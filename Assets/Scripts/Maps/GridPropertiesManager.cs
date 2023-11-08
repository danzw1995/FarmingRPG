using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGuid))]
public class GridPropertiesManager : SingletonMonoBehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid;

    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
    [SerializeField] private SO_GridProperties[] sO_GridPropertiesArray = null;
    [SerializeField] private SO_CropDetailList sO_CropDetailList = null;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;

    private bool isFirstTimeSceneLoaded = true;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave gameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGuid>().GUID;

        gameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfetSceneLoad;
        EventHandler.AdvanceGameDayEvent += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfetSceneLoad;
        EventHandler.AdvanceGameDayEvent -= AdvanceDay;
    }

    private void Start()
    {
        InitialiseGridProperties();
    }

    private void AfetSceneLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();

        GameObject cropParentGameObject = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform);
        if (cropParentGameObject == null)
        {
            cropParentTransform = null;
        } else
        {
            cropParentTransform = cropParentGameObject.transform;
        }
    }

    private void AdvanceDay(int gameYear, Season season, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        ClearDisplayGridPropertyDetails();

        foreach (SO_GridProperties sO_GridProperties in sO_GridPropertiesArray)
        {
            if (gameObjectSave.sceneData.TryGetValue(sO_GridProperties.sceneName.ToString(), out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDictionary.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string, GridPropertyDetails> item = sceneSave.gridPropertyDetailsDictionary.ElementAt(i);

                        GridPropertyDetails gridPropertyDetails = item.Value;

                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays += 1;
                        }

                        if (gridPropertyDetails.daysSinceWatered > -1)
                        {
                            gridPropertyDetails.daysSinceWatered = -1;
                        }

                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDictionary);
                    }
                }
            }
        }

        DisplayGridPropertyDetails();
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableList.Remove(this);
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableList.Add(this);
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameSaveData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave)) {
            this.gameObjectSave = gameObjectSave;

            ISaveableReStoreScene(SceneManager.GetActiveScene().name);
        }
    }

    public GameObjectSave ISaveableSave()
    {
        ISaveableStoreScene(SceneManager.GetActiveScene().name);

        return gameObjectSave;
    }

    public void ISaveableReStoreScene(string sceneName)
    {
        if (gameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                gridPropertyDetailsDictionary = sceneSave.gridPropertyDetailsDictionary;
            }

            if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded", out bool isFirstLoad))
            {
                isFirstTimeSceneLoaded = isFirstLoad;
            }
            if (isFirstTimeSceneLoaded)
            {
                EventHandler.CallInstantiateCropPrefabsEvent();
            }


            if (gridPropertyDetailsDictionary.Count > 0)
            {
                ClearDisplayGridPropertyDetails();
                DisplayGridPropertyDetails();
            }

            if (isFirstTimeSceneLoaded)
            {
                isFirstTimeSceneLoaded = false;
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        gameObjectSave.sceneData.Remove(sceneName);

        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;

        sceneSave.boolDictionary = new Dictionary<string, bool>();
        sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

        gameObjectSave.sceneData.Add(sceneName, sceneSave);
    }


    private void ClearDisplayDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();

    }

    private void ClearDisplayGridPropertyDetails()
    {
        ClearDisplayDecorations();
        ClearDisplayAllPlantedCrops();
    }

    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] crops = FindObjectsOfType<Crop>();

        foreach (Crop crop in crops)
        {
            Destroy(crop.gameObject);
        }
    }

    private void DisplayGridPropertyDetails()
    {
        foreach (KeyValuePair<string, GridPropertyDetails> item in gridPropertyDetailsDictionary)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;

            DisplayDugGround(gridPropertyDetails);
            DisplayWateredGround(gridPropertyDetails);

            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    // 种植
    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        // 判断种子的itemCode是否存在
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // 获取作物
            CropDetail cropDetail = sO_CropDetailList.GetCropDetail(gridPropertyDetails.seedItemCode);
            if (cropDetail != null)
            {


                GameObject cropPrefab;

                int growthStages = cropDetail.growthDays.Length;

                int currentGrowthStage = 0;

                // 寻找当前作物的生长阶段
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetail.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }
                
                // 获取对应生长阶段的prefab、 sprite

                cropPrefab = cropDetail.growthPrefabs[currentGrowthStage];

                Sprite growthSprite = cropDetail.growthSprites[currentGrowthStage];

                // 转成世界坐标，条件到场景中需要世界坐标
                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

                // 世界坐标，x轴加半个cell的大小，保证居中，y轴不用
                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
        }
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }
    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daysSinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        int gridX = gridPropertyDetails.gridX;
        int gridY = gridPropertyDetails.gridY;
        Tile wateredTile0 = SetWateredTile(gridX, gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), wateredTile0);


        // 判断上下左右四块临近的tile
        Vector2Int[] adjacentGridPositions = new Vector2Int[] {
            new Vector2Int(gridX + 1, gridY),
            new Vector2Int(gridX - 1, gridY),
            new Vector2Int(gridX, gridY + 1),
            new Vector2Int(gridX, gridY - 1)
        };

        for (int i = 0; i < adjacentGridPositions.Length; i++)
        {
            GridPropertyDetails adjacentGridPropertyDetails;
            adjacentGridPropertyDetails = GetGridPropertyDetails(adjacentGridPositions[i].x, adjacentGridPositions[i].y);
            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceWatered > -1)
            {
                Tile wateredTile = SetWateredTile(adjacentGridPositions[i].x, adjacentGridPositions[i].y);
                groundDecoration2.SetTile(new Vector3Int(adjacentGridPositions[i].x, adjacentGridPositions[i].y, 0), wateredTile);
            }
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        int gridX = gridPropertyDetails.gridX;
        int gridY = gridPropertyDetails.gridY;
        Tile dugTile0 = SetDugTile(gridX, gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);


        // 判断上下左右四块临近的tile
        Vector2Int[] adjacentGridPositions = new Vector2Int[] {
            new Vector2Int(gridX + 1, gridY),
            new Vector2Int(gridX - 1, gridY),
            new Vector2Int(gridX, gridY + 1),
            new Vector2Int(gridX, gridY - 1)
        };

        for (int i = 0; i < adjacentGridPositions.Length; i++)
        {
            GridPropertyDetails adjacentGridPropertyDetails;
            adjacentGridPropertyDetails = GetGridPropertyDetails(adjacentGridPositions[i].x, adjacentGridPositions[i].y);
            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daysSinceDug > -1)
            {
                Tile dugTile = SetDugTile(adjacentGridPositions[i].x, adjacentGridPositions[i].y);
                groundDecoration1.SetTile(new Vector3Int(adjacentGridPositions[i].x, adjacentGridPositions[i].y, 0), dugTile);
            }
        }
    }

    private bool IsGridSquareDug(int x, int y)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(x, y);
        if (gridPropertyDetails == null) return false;
        return gridPropertyDetails.daysSinceDug > -1;
    }

    private Tile SetDugTile(int x, int y)
    {
        bool upDug = IsGridSquareDug(x, y + 1);
        bool downDug = IsGridSquareDug(x, y - 1);
        bool rightDug = IsGridSquareDug(x + 1, y);
        bool leftDug = IsGridSquareDug(x - 1, y);

        // 根据上下左右四块tile的状态选择对应的图片
        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        } else if (!upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8];
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }
        return null;
    }

    private bool IsGridSquareWatered(int x, int y)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(x, y);
        if (gridPropertyDetails == null) return false;
        return gridPropertyDetails.daysSinceWatered > -1;
    }


    private Tile SetWateredTile(int x, int y)
    {
        bool upWatered = IsGridSquareWatered(x, y + 1);
        bool downWatered = IsGridSquareWatered(x, y - 1);
        bool rightWatered = IsGridSquareWatered(x + 1, y);
        bool leftWatered = IsGridSquareWatered(x - 1, y);

        // 根据上下左右四块tile的状态选择对应的图片
        if (!upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[0];
        }
        else if (!upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[1];
        }
        else if (!upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[2];
        }
        else if (!upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[3];
        }
        else if (!upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[4];
        }
        else if (upWatered && downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[5];
        }
        else if (upWatered && downWatered && rightWatered && leftWatered)
        {
            return wateredGround[6];
        }
        else if (upWatered && downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[7];
        }
        else if (upWatered && downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[8];
        }
        else if (upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[9];
        }
        else if (upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[10];
        }
        else if (upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[11];
        }
        else if (upWatered && !downWatered && !rightWatered && !leftWatered)
        {
            return wateredGround[12];
        }
        else if (!upWatered && !downWatered && rightWatered && !leftWatered)
        {
            return wateredGround[13];
        }
        else if (!upWatered && !downWatered && rightWatered && leftWatered)
        {
            return wateredGround[14];
        }
        else if (!upWatered && !downWatered && !rightWatered && leftWatered)
        {
            return wateredGround[15];
        }
        return null;
    }
    private void InitialiseGridProperties()
    {
        foreach (SO_GridProperties sO_GridProperties in sO_GridPropertiesArray)
        {
            Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary = new Dictionary<string, GridPropertyDetails>();

            foreach (GridProperty gridProperty in sO_GridProperties.gridProperties)
            {
                GridPropertyDetails gridPropertyDetails;

                gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetailsDictionary);
                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiaggble = true;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItem = true;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = true;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = true;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = true;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, gridPropertyDetailsDictionary);
            }

            SceneSave sceneSave = new SceneSave();
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;
            if (sO_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startSceneName.ToString())
            {
                this.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;
            }

            sceneSave.boolDictionary = new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", true);

            gameObjectSave.sceneData.Remove(sO_GridProperties.sceneName.ToString());

            gameObjectSave.sceneData.Add(sO_GridProperties.sceneName.ToString(), sceneSave);
        }
    }


    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDetailsDictionary);

    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary) {
        string key = "x" + gridX + "y" + gridY;

        if (!gridPropertyDetailsDictionary.TryGetValue(key, out GridPropertyDetails gridPropertyDetails))
        {
            return null;
        }
        return gridPropertyDetails;

    }

    public bool GetGridDimensions(SceneName sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)
    {
        gridDimensions = Vector2Int.zero;
        gridOrigin = Vector2Int.zero;

        foreach(SO_GridProperties sO_GridProperties in sO_GridPropertiesArray)
        {
            if (sO_GridProperties.sceneName == sceneName)
            {
                gridDimensions.x = sO_GridProperties.gridWidth;
                gridDimensions.y = sO_GridProperties.gridHeight;

                gridOrigin.x = sO_GridProperties.originX;
                gridOrigin.y = sO_GridProperties.originY;

                return true;
            }
        }
        return false;
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDetailsDictionary);
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        gridPropertyDetailsDictionary[key] = gridPropertyDetails;
    }

    public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));

        Collider2D[] collider2Ds = Physics2D.OverlapPointAll(worldPosition);

        Crop crop = null;
        for (int i = 0; i < collider2Ds.Length; i ++)
        {
            if (collider2Ds[i] != null)
            {
                crop = collider2Ds[i].gameObject.GetComponentInParent<Crop>();
                if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                {
                    break;
                }
                crop = collider2Ds[i].gameObject.GetComponentInChildren<Crop>();
                if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                {
                    break;
                }
            }
        }
        return crop;
    }

    public CropDetail GetCropDetail(int seedItemCode)
    {
        return sO_CropDetailList.GetCropDetail(seedItemCode);
    }
}

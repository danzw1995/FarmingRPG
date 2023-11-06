using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : SingletonMonoBehaviour<Player>, ISaveable
{
    private AnimationOverrides animationOverrides;

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;

    private WaitForSeconds afterUseLiftToolAnimationPause;
    private WaitForSeconds useLiftToolAnimationPause;

    private WaitForSeconds afterPickAnimationPause;
    private WaitForSeconds pickAnimationPause;

    private bool playerToolUseDisabled = false;

    private  float xInput;
    private float yInput;
    private  bool isWalking;
    private  bool isRunning;
    private  bool isIdle;
    private  bool isCarrying = false;
    private  ToolEffect toolEffect = ToolEffect.none;
    private  bool isUsingToolRight;
    private  bool isUsingToolLeft;
    private  bool isUsingToolUp;
    private  bool isUsingToolDown;
    private  bool isLiftingToolRight;
    private  bool isLiftingToolLeft;
    private  bool isLiftingToolUp;
    private  bool isLiftingToolDown;
    private  bool isPickingRight;
    private  bool isPickingLeft;
    private  bool isPickingUp;
    private  bool isPickingDown;
    private  bool isSwingingToolRight;
    private  bool isSwingingToolLeft;
    private  bool isSwingingToolUp;
    private  bool isSwingingToolDown;

    private Camera mainCamera;

    private GridCursor gridCursor;
    private Cursor cursor;


    new private Rigidbody2D rigidbody2D;

    private Direction playerDirection;

    private float movementSpeed;

    private bool _playerInputIsDisabled = false;

    [Tooltip("Should be populated in the prefab with the equipped item sprite renderer")]
    [SerializeField] private SpriteRenderer equippedItemRenderer = null;

    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;

    private List<CharacterAttribute> characterAttributeCustomisationList;

    public bool playerInputIsDisabled
    {
        get => _playerInputIsDisabled;
        set => _playerInputIsDisabled = value;
    }

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave gameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }



    /*   private static int idleRight;
       private static int idleLeft;
       private static int idleUp;
       private static int idleDown;*/


    protected override void Awake()
    {
        base.Awake();

        mainCamera = Camera.main;


        rigidbody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.none);

        characterAttributeCustomisationList = new List<CharacterAttribute>();

        ISaveableUniqueID = GetComponent<GenerateGuid>().GUID;

        gameObjectSave = new GameObjectSave();

    }

    private void Start()
    {
        gridCursor = GameObject.FindObjectOfType<GridCursor>();
        cursor = GameObject.FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        useLiftToolAnimationPause = new WaitForSeconds(Settings.useLiftToolAnimationPause);
        afterUseLiftToolAnimationPause = new WaitForSeconds(Settings.afterUseLiftToolAnimationPause);

        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);

    }

    private void Update()
    {
        #region  Player Input

        if (!playerInputIsDisabled) {
            ResetAnimationTrigger();

            PlayerMovementInput();

            PlayerWalkInput();
            PlayerClickInput();

            PlayerTestInput();


            EventHandler.CallMoventEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
               toolEffect,
               isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
               isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
               isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
               isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);

        }

        #endregion
    }

    private void FixedUpdate()
    {
        Vector2 move = new Vector2(movementSpeed * xInput * Time.deltaTime, movementSpeed * yInput * Time.deltaTime);

        rigidbody2D.MovePosition(rigidbody2D.position + move);
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += EnablePlayerInput;
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= EnablePlayerInput;
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
    }

    private void ResetAnimationTrigger()
    {
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;

        toolEffect = ToolEffect.none;
    }

    private void PlayerMovementInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (xInput != 0 && yInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput > 0)
        {
            playerDirection = Direction.right;
        } else if (xInput < 0)
        {
            playerDirection = Direction.left;
        } else if (yInput > 0)
        {
            playerDirection = Direction.up; 
        } else if (yInput < 0)
        {
            playerDirection = Direction.down;
        }


        if (xInput == 0 && yInput == 0)
        {
            isIdle = true;
            isWalking = false;
            isRunning = false;
        }

    }

    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        } else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }

    private void ResetMovement()
    {
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.cursorIsEnabled || cursor.cursorIsEnabled)
                {
                    Vector3Int gridCursorPosition = gridCursor.GetGridPositionForCusor();

                    Vector3Int gridPlayerPosition = gridCursor.GetGridPositionForPlayer();

                    ProcessPlayerClickInput(gridCursorPosition, gridPlayerPosition);
                }
            }
        }
    }

    private void ProcessPlayerClickInput(Vector3Int gridCursorPosition, Vector3Int gridPlayerPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(gridCursorPosition, gridPlayerPosition);

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridCursorPosition.x, gridCursorPosition.y);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }
                    break;
                case ItemType.Commodity:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                case ItemType.Watering_tool:
                case ItemType.Hoeing_tool:
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputHoeingTool(gridPropertyDetails, itemDetails, playerDirection);
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int gridCursorPosition, Vector3Int gridPlayerPosition)
    {
        if (gridCursorPosition.x >  gridPlayerPosition.x)
        {
            return Vector3Int.right;
        } else if (gridCursorPosition.x < gridPlayerPosition.x)
        {
            return Vector3Int.left;
        } else if (gridCursorPosition.y > gridPlayerPosition.y)
        {
            return Vector3Int.up;
        } else
        {
            return Vector3Int.down;
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (
            cursorPosition.x > playerPosition.x
            && 
            cursorPosition.y > playerPosition.y - cursor.itemUseRadius / 2f 
            && 
            cursorPosition.y  < playerPosition.y + cursor.itemUseRadius / 2f
            )
        {
            return Vector3Int.right;
        }
        else if (
            cursorPosition.x < playerPosition.x
            &&
            cursorPosition.y > playerPosition.y - cursor.itemUseRadius / 2f
            &&
            cursorPosition.y < playerPosition.y + cursor.itemUseRadius / 2f
            )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputHoeingTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.cursorPositionIsValid)
                {
                    HoeGroundCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Watering_tool:
                if (gridCursor.cursorPositionIsValid)
                {
                    WaterGroundCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Reaping_tool:
                if (cursor.cursorIsEnabled)
                {
                    playerDirection = GetPlayerClickDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
            case ItemType.Chopping_tool:
                if (gridCursor.cursorPositionIsValid)
                {
                    ChopInPlayerDirectionAtCursor(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Breaking_tool:
                if (gridCursor.cursorPositionIsValid)
                {
                    BreakingInPlayerDirectionAtCursor(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            case ItemType.Collecting_tool:
                if (gridCursor.cursorPositionIsValid)
                {
                    playerDirection = GetPlayerClickDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    CollectInPlayerDirectionAtCursor(gridPropertyDetails, itemDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private void BreakingInPlayerDirectionAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectPickaxe);

        StartCoroutine(BreakingInPlayerDirectionAtCursorRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator BreakingInPlayerDirectionAtCursorRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVariantType = PartVariantType.pickaxe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
        yield return useToolAnimationPause;


        yield return afterUseToolAnimationPause;


        playerInputIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void ChopInPlayerDirectionAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectAxe);

        StartCoroutine(ChopInPlayerDirectionAtCursorRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator ChopInPlayerDirectionAtCursorRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVariantType = PartVariantType.axe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
        yield return useToolAnimationPause;


        yield return afterUseToolAnimationPause;


        playerInputIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void CollectInPlayerDirectionAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectBasket);

        StartCoroutine(CollectInPlayerDirectionAtCursorRoutine(gridPropertyDetails, itemDetails, playerDirection));
    }

    private IEnumerator CollectInPlayerDirectionAtCursorRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;


        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
        yield return pickAnimationPause;


        yield return afterPickAnimationPause;


        playerInputIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void ProcessCropWithEquippedItemInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {

        switch (equippedItemDetails.itemType)
        {
            case ItemType.Chopping_tool:
            case ItemType.Breaking_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                } else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                } else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                } else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }

                break;
            case ItemType.Collecting_tool:

                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            default:
                break;
        }

     

        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);
        if (crop != null)
        {

            switch (equippedItemDetails.itemType)
            {
                case ItemType.Chopping_tool:
                case ItemType.Breaking_tool:
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolLeft, isUsingToolRight, isUsingToolUp, isUsingToolDown);
                    break;
                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingLeft, isPickingRight, isPickingUp, isPickingDown);
                    break;
                default:
                    break;

            }

        }
   
    }


    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {

        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        playerInputIsDisabled = false;
        playerToolUseDisabled = false;

    }

    private void UseToolInPlayerDirection(ItemDetails equipmentItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch(equipmentItemDetails.itemType)
            {
                case ItemType.Reaping_tool:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    } else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    } else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    } else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }

                    Vector3 playerCenterPosition = GetPlayerCentrePosition();

                    Vector2 point = new Vector2(playerCenterPosition.x + (playerDirection.x * equipmentItemDetails.itemUseRadius / 2f), playerCenterPosition.y + (playerDirection.y * equipmentItemDetails.itemUseRadius / 2f));

                    Vector2 size = new Vector2(equipmentItemDetails.itemUseRadius, equipmentItemDetails.itemUseRadius);

                    Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0f);

                    int repableItemCount = 0;

                    for (int i = 0; i < itemArray.Length; i ++)
                    {
                        if (itemArray[i] != null && InventoryManager.Instance.GetItemDetails(itemArray[i].itemCode).itemType == ItemType.Reapable_scenary)
                        {
                            Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x, itemArray[i].transform.position.y + Settings.gridCellSize / 2f, itemArray[i].transform.position.z);

                            EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);

                            AudioManager.Instance.PlaySound(SoundName.effectScythe);


                            Destroy(itemArray[i].gameObject);
                            repableItemCount++;
                            if (repableItemCount >= Settings.maxTargetComponentsToDestroyPerReapSwing)
                            {
                                break;
                            }
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }

    private void WaterGroundCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectWateringCan);
        StartCoroutine(WaterGroundCursorRoutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator WaterGroundCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;

        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return useLiftToolAnimationPause;
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);


        yield return afterUseLiftToolAnimationPause;

        playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    private void HoeGroundCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.effectHoe);
        StartCoroutine(HoeGroundCursorRoutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator HoeGroundCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        playerInputIsDisabled = true;
        playerToolUseDisabled = true;

        toolCharacterAttribute.partVariantType = PartVariantType.hoe;

        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);


        if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        } else if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        } else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        } else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;
        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        playerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.cursorPositionIsValid && gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        } else if (itemDetails.canBeDropped && gridCursor.cursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridPropertiesManager.Instance.GetCropDetail(itemDetails.itemCode) != null) {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            EventHandler.CallRemoveSelectedItemFormInventoryEvent();

            AudioManager.Instance.PlaySound(SoundName.effectPlantingSound);
        }

    
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.cursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();

        ResetMovement();
        EventHandler.CallMoventEvent(xInput, yInput, isWalking, isRunning, isIdle, isCarrying,
        toolEffect,
        isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
        isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
        isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
        isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
         false, false, false, false);
    }

    public void EnablePlayerInput()
    {
        playerInputIsDisabled = false;
    }

    public void DisablePlayerInput()
    {
        playerInputIsDisabled = true;
    }

    public Vector3 GetPlayerViewPortPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemRenderer.sprite = itemDetails.itemSprite;
            equippedItemRenderer.color = new Color(1f, 1f, 1f, 1f);

            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);

            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
            isCarrying = true;
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemRenderer.sprite = null;
        equippedItemRenderer.color = new Color(1f, 1f, 1f, 0f);

        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();

        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        isCarrying = false;
    }
    /*
     测试用
     */
    public void PlayerTestInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(), transform.position);
        }
    }


    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        gameObjectSave.sceneData.Remove(Settings.persistentScene);

        SceneSave sceneSave = new SceneSave();

        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serilizable>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        Vector3Serilizable vector3Serilizable = new Vector3Serilizable(transform.position.x, transform.position.y, transform.position.z);

        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serilizable);

        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        gameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);

        return gameObjectSave;
        
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameSaveData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null && sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serilizable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                if (sceneSave.stringDictionary != null)
                {
                    if(sceneSave.stringDictionary.TryGetValue("currentScene", out string sceneName))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(sceneName, transform.position);
                    }

                    if (sceneSave.stringDictionary.TryGetValue("playerDirection", out string playerDirectionString))
                    {
                        bool playerDirectionFound = Enum.TryParse<Direction>(playerDirectionString, out Direction direction);

                        if (playerDirectionFound)
                        {
                            playerDirection = direction;

                            SetPlayerDirection(playerDirection);
                        }
                    }
                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName) { }

    public void ISaveableReStoreScene(string sceneName) { }


    public void SetPlayerDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.up:
                EventHandler.CallMoventEvent(0f, 0f, false, false, false, false,
                  ToolEffect.none,
                  false, false, false, false,
                  false, false, false, false,
                  false, false, false, false,
                  false, false, false, false,
                  false, false, true, false);
                break;
            case Direction.down:
                EventHandler.CallMoventEvent(0f, 0f, false, false, false, false,
                 ToolEffect.none,
                 false, false, false, false,
                 false, false, false, false,
                 false, false, false, false,
                 false, false, false, false,
                 false, false, false, true);
                break;
            case Direction.left:
                EventHandler.CallMoventEvent(0f, 0f, false, false, false, false,
              ToolEffect.none,
              false, false, false, false,
              false, false, false, false,
              false, false, false, false,
              false, false, false, false,
              false, true, false, false);
                break;
            case Direction.right:
                EventHandler.CallMoventEvent(0f, 0f, false, false, false, false,
              ToolEffect.none,
              false, false, false, false,
              false, false, false, false,
              false, false, false, false,
              false, false, false, false,
              true, false, false, false);
                break;
            default:
                break;
        }
    }
}

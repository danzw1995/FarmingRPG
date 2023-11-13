using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorSwap
{
    public Color fromColor;
    public Color toColor;

    public ColorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}

public class ApplyCharatcterCustomisation : MonoBehaviour
{
    [Header("Base Textures")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    [SerializeField] private Texture2D hatsBaseTexture = null;
    [SerializeField] private Texture2D adornmentBaseTexture = null;
    private Texture2D farmerBaseTexture = null;


    [Header("Output Texture To Be Used For Animation")]

    [SerializeField] private Texture2D farmerBaseCustomised = null;
    [SerializeField] private Texture2D hairCustomised = null;
    [SerializeField] private Texture2D hatsCustomised = null;
    private Texture2D farmerBaseShirtUpdated;
    private Texture2D farmerBaseAdornmentUpdated;
    private Texture2D selectedShirts;
    private Texture2D selectedAdornment;

    [Header("Select Shirt Style")]
    [Range(0, 1)]
    [SerializeField] private int inputShirtStyleNo = 0;

    [Header("Select Hair Style")]
    [Range(0, 2)]
    [SerializeField] private int inputHairStyleNo = 0;

    [Header("Select Hat Style")]
    [Range(0, 1)]
    [SerializeField] private int inputHatStyleNo = 0;

    [Header("Select Adornment Style")]
    [Range(0, 2)]
    [SerializeField] private int inputAdornmentStyleNo = 0;

    [Header("Select Skin Type")]
    [Range(0, 3)]
    [SerializeField] private int inputSkinTypeNo = 0;

    [Header("Select Sex: 0-Male, 1-Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    [SerializeField] private Color inputTrouserColor = Color.blue;

    [SerializeField] private Color inputHairColor = Color.black;

    private Facing[,] bodyFacingArray;
    private Vector2Int[,] bodyShirtOffsetArray;
    private Vector2Int[,] bodyAdornmentOffsetArray;


    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;

    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStylesInSpriteWidth = 16;

    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    private int hairStylesInSpriteWidth = 8;

    private int hatTextureWidth = 20;
    private int hatTextureHeight = 80;
    private int hatStylesInSpriteWidth = 12;

    private int adornmentTextureWidth = 16;
    private int adornmentTextureHeight = 32;
    private int adornmentStylesInSpriteWidth = 8;
    private int adornmentSpriteWidth = 16;
    private int adornmentSpriteHeight = 16;

    private List<ColorSwap> colorSwapList;

    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);


    private Color32 skinTargetColor1 = new Color32(145, 117, 90, 255);
    private Color32 skinTargetColor2 = new Color32(204, 155, 108, 255);
    private Color32 skinTargetColor3 = new Color32(207, 166, 128, 255);
    private Color32 skinTargetColor4 = new Color32(238, 195, 154, 255);


    private void Awake()
    {
        colorSwapList = new List<ColorSwap>();

        ProcessCustomisation();
    }

    // 自定义人物
    private void ProcessCustomisation() {

        // 处理性别
        ProcessGender();

        // 处理衣服
        ProcessShirt();

        // 处理手臂
        ProcessArms();

        // 处理裤子
        ProcessTrousers();

        // 处理头发
        ProcessHair();

        // 处理肤色
        ProcessSkin();

        // 处理帽子
        ProcessHat();

        // 处理装饰
        ProcessAdornment();

        // 合并更改
        MergeCustomisations();
    }

    private void ProcessGender() {
        if (inputSex == 0)
        {
            farmerBaseTexture = maleFarmerBaseTexture;
        } else if (inputSex == 1)
        {
            farmerBaseTexture = femaleFarmerBaseTexture;
        }

        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        farmerBaseCustomised.SetPixels(farmerBasePixels);
        farmerBaseCustomised.Apply();

    }

    private void ProcessShirt() {
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        PopulateBodyFacingArray();

        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        PopulateBodyShirtOffsetArray();

        AddShirtToTexture(inputShirtStyleNo);

        ApplyShirtTextureToBase();

    }

    private void PopulateBodyFacingArray()
    {
        for (int row = 0; row < 10; row++)
        {

            for (int column = 0; column < bodyColumns; column++)
            { 
                // 这些位置设为none;
                bodyFacingArray[column, row] = Facing.none;
             }
        }


        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;


        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;


        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;




    }

    private void PopulateBodyShirtOffsetArray()
    {
        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < bodyColumns; column++)
            {
                // 这些位置设为none;
                bodyShirtOffsetArray[column, row] = new Vector2Int(99, 99);

            }
        }

        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);


        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }


    private void AddShirtToTexture(int shirtStyleNo)
    {
        selectedShirts = new Texture2D(shirtTextureWidth, shirtTextureHeight);

        selectedShirts.filterMode = FilterMode.Point;

        int y = (shirtStyleNo / shirtStylesInSpriteWidth) * shirtTextureHeight;
        int x = (shirtStyleNo % shirtStylesInSpriteWidth) * shirtTextureWidth;

        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        selectedShirts.SetPixels(shirtPixels);
        selectedShirts.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        farmerBaseShirtUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseShirtUpdated.filterMode = FilterMode.Point;

        SetTextureToParent(farmerBaseShirtUpdated);

        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

        frontShirtPixels = selectedShirts.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirts.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirts.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        for (int column = 0; column < bodyColumns; column++)
        {
            for (int row = 0; row < bodyRows; row++)
            {
                int pixelColumn = column * farmerSpriteWidth;
                int pixelRow = row * farmerSpriteHeight;

                if (bodyShirtOffsetArray[column, row] != null)
                {
                    if (bodyShirtOffsetArray[column, row].x == 99 && bodyShirtOffsetArray[column, row].y == 99)
                    {
                        continue;
                    }
                    pixelColumn += bodyShirtOffsetArray[column, row].x;
                    pixelRow += bodyShirtOffsetArray[column, row].y;

                    switch (bodyFacingArray[column, row])
                    {
                        case Facing.front:
                            farmerBaseShirtUpdated.SetPixels(pixelColumn, pixelRow, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                            break;
                        case Facing.back:
                            farmerBaseShirtUpdated.SetPixels(pixelColumn, pixelRow, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                            break;
                        case Facing.right:
                            farmerBaseShirtUpdated.SetPixels(pixelColumn, pixelRow, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                            break;
                        case Facing.none:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        farmerBaseShirtUpdated.Apply();
    }

    private void SetTextureToParent(Texture2D texture2D)
    {
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }

        texture2D.SetPixels(fill);
    }
    private void ProcessArms() {

        Color[] farmerPixelsToReColor = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);

        PopulateArmColorSwapList();

        ChangePixelColors(farmerPixelsToReColor, colorSwapList);

        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToReColor);

        farmerBaseCustomised.Apply();
    }

    private void PopulateArmColorSwapList()
    {
        colorSwapList.Clear();

        colorSwapList.Add(new ColorSwap(armTargetColor1, selectedShirts.GetPixel(0, 7)));
        colorSwapList.Add(new ColorSwap(armTargetColor2, selectedShirts.GetPixel(0, 6)));
        colorSwapList.Add(new ColorSwap(armTargetColor3, selectedShirts.GetPixel(0, 5)));
    }

    private void ChangePixelColors(Color[] baseArray, List<ColorSwap> colorSwap)
    {
        if (colorSwap.Count > 0)
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                for (int j = 0; j < colorSwap.Count; j ++)
                {
                    if (isSameColor(baseArray[i], colorSwap[j].fromColor))
                    {
                        baseArray[i] = colorSwap[j].toColor;
                    }
                }
            }
        }
     
    }

    private bool isSameColor(Color color1, Color color2)
    {
        if (color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a) { 
            return true;
    }    else
        {
            return false;

        }
    }

    private void ProcessTrousers()
    {
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);

        TintPixelColors(farmerTrouserPixels, inputTrouserColor);



        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);

        farmerBaseCustomised.Apply();
    }

    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        for (int i = 0; i < basePixelArray.Length; i ++)
        {
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }

    private void ProcessHair()
    {
        AddHairToTexture(inputHairStyleNo);

        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();

        TintPixelColors(farmerSelectedHairPixels, inputHairColor);

        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();
    }

    private void AddHairToTexture(int hairStyleNo)
    {
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth;

        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);
        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }

    private void ProcessSkin()
    {
        Color[] farmerPixelsToReColor = farmerBaseCustomised.GetPixels(0, 0, 288, farmerBaseTexture.height);

        PopulateSkinColorSwapList(inputSkinTypeNo);

        ChangePixelColors(farmerPixelsToReColor, colorSwapList);

        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToReColor);

        farmerBaseCustomised.Apply();
        
    }

    private void PopulateSkinColorSwapList(int skinTypeNo)
    {
        colorSwapList.Clear();

        switch (skinTypeNo)
        {
       
            case 1:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(187, 157, 128, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(231, 187, 144, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(221, 186, 154, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(213, 189, 167, 255)));
                break;
            case 2:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(105, 69, 2, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(128, 87, 12, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(145, 103, 26, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(161, 114, 25, 255)));
                break;
            case 3:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, new Color32(151, 132, 0, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, new Color32(187, 166, 15, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, new Color32(209, 188, 39, 255)));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, new Color32(211, 199, 112, 255)));
                break;
            case 0:
            default:
                colorSwapList.Add(new ColorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new ColorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new ColorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new ColorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }

    private void ProcessHat()
    {
        AddHatToTexture(inputHatStyleNo);
    }

    private void AddHatToTexture(int hatStyleNo)
    {
        int y = (hatStyleNo / hatStylesInSpriteWidth) * hatTextureHeight;
        int x = (hatStyleNo % hatStylesInSpriteWidth) * hatTextureWidth;

        Color[] hatPixels = hatsBaseTexture.GetPixels(x, y, hatTextureWidth, hatTextureHeight);

        hatsCustomised.SetPixels(hatPixels);
        hatsCustomised.Apply();
    }

    private void ProcessAdornment()
    {
        bodyAdornmentOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        PopulateBodyAdornmentOffsetArray();

        AddAdornmentToTexture(inputAdornmentStyleNo);

        farmerBaseAdornmentUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);

        farmerBaseAdornmentUpdated.filterMode = FilterMode.Point;

        SetTextureToParent(farmerBaseAdornmentUpdated);

        ApplyAddornmentTextureToBase();
    }

    private void PopulateBodyAdornmentOffsetArray ()
    {
        for (int row = 0; row < 10; row++)
        {
            for (int column = 0; column < bodyColumns; column++)
            {
                bodyAdornmentOffsetArray[column, row] = new Vector2Int(99, 99);
            }
        }

        bodyAdornmentOffsetArray[0, 10] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[1, 10] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[2, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentOffsetArray[3, 10] = new Vector2Int(0, 2 + 16);
        bodyAdornmentOffsetArray[4, 10] = new Vector2Int(0, 1 + 16);
        bodyAdornmentOffsetArray[5, 10] = new Vector2Int(0, 0 + 16);

        bodyAdornmentOffsetArray[0, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentOffsetArray[1, 11] = new Vector2Int(0, 2 + 16);
        bodyAdornmentOffsetArray[2, 11] = new Vector2Int(0, 1 + 16);
        bodyAdornmentOffsetArray[3, 11] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[4, 11] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[5, 11] = new Vector2Int(99, 99);

        bodyAdornmentOffsetArray[0, 12] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[1, 12] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[2, 12] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[3, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[4, 12] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[5, 12] = new Vector2Int(0, -1 + 16);

        bodyAdornmentOffsetArray[0, 13] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[1, 13] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[2, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentOffsetArray[3, 13] = new Vector2Int(1, -1 + 16);
        bodyAdornmentOffsetArray[4, 13] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[5, 13] = new Vector2Int(99, 99);

        bodyAdornmentOffsetArray[0, 14] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[1, 14] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[2, 14] = new Vector2Int(0, -3 + 16);
        bodyAdornmentOffsetArray[3, 14] = new Vector2Int(0, -5 + 16);
        bodyAdornmentOffsetArray[4, 14] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[5, 14] = new Vector2Int(0, 1 + 16);

        bodyAdornmentOffsetArray[0, 15] = new Vector2Int(0, -2 + 16);
        bodyAdornmentOffsetArray[1, 15] = new Vector2Int(0, -5 + 16);
        bodyAdornmentOffsetArray[2, 15] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[3, 15] = new Vector2Int(0, 2 + 16);
        bodyAdornmentOffsetArray[4, 15] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[5, 15] = new Vector2Int(99, 99);

        bodyAdornmentOffsetArray[0, 16] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[1, 16] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[2, 16] = new Vector2Int(0, -3 + 16);
        bodyAdornmentOffsetArray[3, 16] = new Vector2Int(0, -2 + 16);
        bodyAdornmentOffsetArray[4, 16] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[5, 16] = new Vector2Int(0, 0 + 16);

        bodyAdornmentOffsetArray[0, 17] = new Vector2Int(0, -3 + 16);
        bodyAdornmentOffsetArray[1, 17] = new Vector2Int(0, -2 + 16);
        bodyAdornmentOffsetArray[2, 17] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[3, 17] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[4, 17] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[5, 17] = new Vector2Int(99, 99);


        bodyAdornmentOffsetArray[0, 18] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[1, 18] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[2, 18] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[3, 18] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[4, 18] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[5, 18] = new Vector2Int(0, -1 + 16);

        bodyAdornmentOffsetArray[0, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[1, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[2, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[3, 19] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[4, 19] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[5, 19] = new Vector2Int(0, -1 + 16);


        bodyAdornmentOffsetArray[0, 20] = new Vector2Int(0, 0 + 16);
        bodyAdornmentOffsetArray[1, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[2, 20] = new Vector2Int(0, -1 + 16);
        bodyAdornmentOffsetArray[3, 20] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[4, 20] = new Vector2Int(99, 99);
        bodyAdornmentOffsetArray[5, 20] = new Vector2Int(99, 99);
    }

    private void AddAdornmentToTexture(int adornmentStyleNo)
    {
        selectedAdornment = new Texture2D(adornmentTextureWidth, adornmentTextureHeight);

        selectedAdornment.filterMode = FilterMode.Point;

        int y = (adornmentStyleNo / adornmentStylesInSpriteWidth) * adornmentTextureHeight;
        int x = (adornmentStyleNo % adornmentStylesInSpriteWidth) * adornmentTextureWidth;

        Color[] adornmentPixels = adornmentBaseTexture.GetPixels(x, y, adornmentTextureWidth, adornmentTextureHeight);

        selectedAdornment.SetPixels(adornmentPixels);

        selectedAdornment.Apply();
    }

    private void ApplyAddornmentTextureToBase()
    {
        Color[] frontAdornmentPixels;
        Color[] rightAdornmentPixels;

        frontAdornmentPixels = selectedAdornment.GetPixels(0, adornmentSpriteHeight * 1, adornmentSpriteWidth, adornmentSpriteHeight);
        rightAdornmentPixels = selectedAdornment.GetPixels(0, adornmentSpriteHeight * 0, adornmentSpriteWidth, adornmentSpriteHeight);

        for (int x = 0; x < bodyColumns; x ++)
        {
            for (int y = 0; y < bodyRows; y ++)
            {
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;
                if (bodyAdornmentOffsetArray[x, y] != null)
                {
                    pixelX += bodyAdornmentOffsetArray[x, y].x;
                    pixelY += bodyAdornmentOffsetArray[x, y].y;
                }

                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;
                    case Facing.front:
                        farmerBaseAdornmentUpdated.SetPixels(pixelX, pixelY, adornmentSpriteWidth, adornmentSpriteHeight, frontAdornmentPixels);
                        break;
                    case Facing.right:
                        farmerBaseAdornmentUpdated.SetPixels(pixelX, pixelY, adornmentSpriteWidth, adornmentSpriteHeight, rightAdornmentPixels);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void MergeCustomisations() {
        Color[] farmerShirtPixels = farmerBaseShirtUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288, 0, 96, farmerBaseTexture.height);

        Color[] farmerAdornmentPixels = farmerBaseAdornmentUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        Color[] farmerBodyPixels = farmerBaseCustomised.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);

        MergeColorArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColorArray(farmerBodyPixels, farmerShirtPixels);
        MergeColorArray(farmerBodyPixels, farmerAdornmentPixels);

        farmerBaseCustomised.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);

        farmerBaseCustomised.Apply();

    }

    private void MergeColorArray(Color[] baseArray, Color[] mergeArray)
    {
        for (int i = 0; i < baseArray.Length; i ++)
        {
            if (mergeArray[i].a > 0)
            {
                if (mergeArray[i].a >= 1)
                {
                    baseArray[i] = mergeArray[i];
                } else
                {
                    float alpha = mergeArray[i].a;

                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;

                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }

}

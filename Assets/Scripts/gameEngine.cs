using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEngine : MonoBehaviour
{
    public GameObject gameCube;
    public int len;
    public Material blank;
    public Material farmField;
    public Material pollinators;
    public Material prairie;
    public Material riparianNoTree;
    public Material riparianTree;
    public Material upland;
    public Material river;
    public Material road;
    public Material selector;
    public Material backgroundPlaneMaterial;

    public Camera projectiveCamera;
    public Camera orthographicCamera;

    public GameObject backgroundPlane;
    public GameObject yeildPlane;
    public GameObject biodiversityPlane;
    public GameObject biodiversityBlurPlane;

    public Texture2D yeildTextureInput;
    public Texture2D pollinatorTextureInput;
    public Texture2D prairieTextureInput;
    public Texture2D riparianNoTreeTextureInput;
    public Texture2D riparianTextureInput;
    public Texture2D uplandTreeTextureInput;
    public Texture2D waterTextureInput;
    public Texture2D heightTextureInput;
    public Texture2D backgroundPlaneTexture;

    // computeshader texture variables
    private Texture2D biodiversityTextureResult;
    private Texture2D fertilizerTextureResult;
    private Texture2D biodiversityTextureResultBoth;
    private RenderTexture biodiversityRendTextureResult;
    private RenderTexture fertilizerRendTextureResult;
    private Texture2D biodiversityTextureBlurResult;
    private RenderTexture biodiversityRendTextureBlurResult;
    private Texture2D yeildTextureResult;
    private RenderTexture yeildRendTextureResult;
    public Texture2D minMapBoxTexture;
    public Texture2D minMapBoxTextureFinal; //also used in gui with camera position
    private RenderTexture minMapBoxRendTexture;

    // computeshader buffers
    private ComputeBuffer cubeThereBuffer;
    private ComputeBuffer cubeBuffer;


    // computeshader initialization variables
    public ComputeShader computeShader;
    private int CSMain;

    private int fertilizerFlow = 0;

    // score variables
    private int select;
    private int season;
    private float yeildScore1;
    private float yeildScore2;
    private float yeildScore3;
    private float biodiversityScore1;
    private float biodiversityScore2;
    private float biodiversityScore3;
    private float fertilizerScore2;
    private float fertilizerScore3;

    // GUi box variables
    public Texture panBoxTexture;
    Rect panBoxRect;

    Rect minMapBoxRect;

    private int cursorType;
    private int cameraType;
    private int cursorSelectDeselect;
    private float initMx;
    private float initMy;
    private Vector3 initRayPoint;
    private float finalMx;
    private float finalMy;
    private int buffercount;

    private int posi;
    private int posi_;
    private int posj;
    private int posj_;

    private string spreadString = "2";
    private string baseBiodiversityString = "0.6";
    private string biodiversityFlowString = "0.6";
    private string s1YeildString = "0.5";
    private string s2YeildString = "0.49";
    private string s3YeildString = "0.47";
    private string s2BiodiversityString = "2";
    private string s3BiodiversityString = "3";

    private int spread;
    private float baseBiodiversity;
    private float biodiversityFlow;
    private float s1Yeild;
    private float s2Yeild;
    private float s3Yeild;
    private float s2Biodiversity;
    private float s3Biodiversity;

    // color management
    Color[] color;
    float[] colorAvg;

    private GameObject[] cubeInstList;
    private int[] cubeThere;

    // Start is called before the first frame update
    void Start()
    {
        //gameCube.GetComponent<Renderer>().material = selector;
        select = 1;
        season = 1;

        panBoxRect = new Rect(0, 0, 0, 0);
        minMapBoxRect = new Rect(Screen.width-160, Screen.height - 160, 150, 150);
        cursorType = 0;
        cameraType = 0;
        cursorSelectDeselect = 1;

        buffercount = 0;

        baseBiodiversity = float.Parse(baseBiodiversityString);
        biodiversityFlow = float.Parse(biodiversityFlowString);
        s1Yeild = float.Parse(s1YeildString);
        s2Yeild = float.Parse(s2YeildString);
        s3Yeild = float.Parse(s3YeildString);
        s2Biodiversity = float.Parse(s2BiodiversityString);
        s3Biodiversity = float.Parse(s3BiodiversityString);
        //
        //
        //
        // plane stuff
        //
        //
        //

        // backgorund plane color and transparency
        backgroundPlane.GetComponent<Renderer>().material = backgroundPlaneMaterial;
        backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // backgorund position, scale, rotation
        backgroundPlane.transform.position = new Vector3(len / 2 - 0.5f, -0f, len / 2 - 0.5f);
        backgroundPlane.transform.localScale = new Vector3(len / 10 + 0.8f, 1, -(len / 10 + 0.8f));
        backgroundPlane.transform.Rotate(0f, 90f, 0f, Space.Self);

        //backgroundPlaneTexture = backgroundPlane.GetComponent<Renderer>().material.mainTexture;
        backgroundPlane.GetComponent<Renderer>().material.mainTexture = backgroundPlaneTexture;

        // yeild plane stuff
        yeildPlane.GetComponent<Renderer>().material.mainTexture = yeildTextureInput;

        // biodiversity plane stuff
        biodiversityPlane.GetComponent<Renderer>().material.mainTexture = biodiversityTextureResult;

        biodiversityTextureResultBoth = new Texture2D(len, len);



        //
        //
        //
        // Averaging material colors to manually set in compute shader
        //
        //
        //
        RenderTexture yeildAvg = new RenderTexture(1, 1, 0);
        Graphics.Blit(farmField.mainTexture, yeildAvg);
        Debug.Log("farmField color: " + GetRTPixels(yeildAvg).GetPixels()[0]);

        Graphics.Blit(pollinators.mainTexture, yeildAvg);
        Debug.Log("pollinators color: " + GetRTPixels(yeildAvg).GetPixels()[0]);

        Graphics.Blit(prairie.mainTexture, yeildAvg);
        Debug.Log("prairie color: " + GetRTPixels(yeildAvg).GetPixels()[0]);

        Graphics.Blit(riparianNoTree.mainTexture, yeildAvg);
        Debug.Log("riparianNoTree color: " + GetRTPixels(yeildAvg).GetPixels()[0]);

        Graphics.Blit(riparianTree.mainTexture, yeildAvg);
        Debug.Log("riparianTree color: " + GetRTPixels(yeildAvg).GetPixels()[0]);

        Graphics.Blit(upland.mainTexture, yeildAvg);
        Debug.Log("upland color: " + GetRTPixels(yeildAvg).GetPixels()[0]);



        //
        //
        //
        //
        // reading texture values
        //
        //
        //
        //


        RenderTexture rendTextureScaled = new RenderTexture(len, len, 0);
        colorAvg = new float[len * len * 13];

        // yeild texture
        Graphics.Blit(yeildTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 1 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }


        // pollinator texture
        Graphics.Blit(pollinatorTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 2 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }

        // prairie texture
        Graphics.Blit(prairieTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 3 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }


        // riparian no tree texture
        Graphics.Blit(riparianNoTreeTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 4 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }

        // riparian texture
        Graphics.Blit(riparianTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 5 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }

        // upland tree texture
        Graphics.Blit(uplandTreeTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 6 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }

        // water basin texture
        Graphics.Blit(waterTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 7 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }

        // height texture
        Graphics.Blit(heightTextureInput, rendTextureScaled);
        color = GetRTPixels(rendTextureScaled).GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            colorAvg[len * len * 8 + i] = color[i].r / 3.0f + color[i].g / 3.0f + color[i].b / 3.0f;
        }
        

        // waterbasin dX and dY. Empty fertilizer value and Empty biodiversity value
        for (int i = 1; i < len - 1; i++)
        {
            for (int j = 1; j < len - 1; j++)
            {
                colorAvg[len * len * 9 + i * len + j] =
                    colorAvg[len * len * 7 + (i - 1) * len + (j - 1)] * -03f + colorAvg[len * len * 7 + (i - 1) * len + (j + 0)] * -00f + colorAvg[len * len * 7 + (i - 1) * len + (j + 1)] * +03f +
                    colorAvg[len * len * 7 + (i + 0) * len + (j - 1)] * -10f + colorAvg[len * len * 7 + (i + 0) * len + (j + 0)] * -00f + colorAvg[len * len * 7 + (i + 0) * len + (j + 1)] * +10f +
                    colorAvg[len * len * 7 + (i + 1) * len + (j - 1)] * -03f + colorAvg[len * len * 7 + (i + 1) * len + (j + 0)] * -00f + colorAvg[len * len * 7 + (i + 1) * len + (j + 1)] * +03f;

                colorAvg[len * len * 10 + i * len + j] =
                    colorAvg[len * len * 7 + (i - 1) * len + (j - 1)] * -03f + colorAvg[len * len * 7 + (i - 1) * len + (j + 0)] * -10f + colorAvg[len * len * 7 + (i - 1) * len + (j + 1)] * -03f +
                    colorAvg[len * len * 7 + (i + 0) * len + (j - 1)] * +00f + colorAvg[len * len * 7 + (i + 0) * len + (j + 0)] * -00f + colorAvg[len * len * 7 + (i + 0) * len + (j + 1)] * +00f +
                    colorAvg[len * len * 7 + (i + 1) * len + (j - 1)] * +03f + colorAvg[len * len * 7 + (i + 1) * len + (j + 0)] * +10f + colorAvg[len * len * 7 + (i + 1) * len + (j + 1)] * +03f;

                colorAvg[len * len * 11 + i * len + j] = 0.0f;
                colorAvg[len * len * 12 + i * len + j] = 0.0f;
            }
        }
        //
        //
        //
        //
        // creating gridmap of cube instances
        //
        //
        //
        //
        cubeInstList = new GameObject[len * len];
        cubeThere = new int[len * len];
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                cubeInstList[i * len + j] = Instantiate(gameCube, new Vector3(i * 1.0f, 0, j * 1.0f), Quaternion.identity);
                cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(1);
                cubeThere[i * len + j] = 1;
                cubeInstList[i * len + j].transform.localScale = new Vector3(0.9f, colorAvg[len * len * 8 + i * len + j] * 3 +0.1f, 0.9f);

                //change material color based on height
                cubeInstList[i * len + j].GetComponent<MeshRenderer>().material.color = ColorFromHeight(colorAvg[len * len * 8 + i * len + j]);
            }
        }
        

        //
        //
        //
        //
        //
        //_____________adding roads
        //
        //
        //
        //
        //

        // single roads

        int numOfSinlgeRoads = 6;

        // vertical single roads

        for  (int i = 0; i < len; i = i + Mathf.RoundToInt(len / numOfSinlgeRoads))
        {
            for (int j = 0; j < len; j++)
            {
                cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                cubeThere[i * len + j] = 8; // change tile type
            }
        }

        // horizontal single roads

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j = j + Mathf.RoundToInt(len / numOfSinlgeRoads))
            {
                cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                cubeThere[i * len + j] = 8; // change tile type
            }
        }

        // double roads

        int widthOfDoubleRoads = 2;

        // vertical border roads -> horizontal
        for (int i = 0; i < len; i = i + len/2 - widthOfDoubleRoads/2)
        {
            for (int j = 0; j < len; j++)
            {
                for (int k = 0; k < widthOfDoubleRoads; k++)
                {
                    cubeInstList[(i + k) * len + j].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                    cubeThere[(i + k) * len + j] = 8; // change tile type
                }
            }
        }

        // left horizontal half double roads
        for (int i = 0; i < len / 2; i++)
        {
            for (int j = Mathf.FloorToInt(len / 6); j < len; j = j + len / 2 - widthOfDoubleRoads)
            {
                for (int k = 0; k < widthOfDoubleRoads; k++)
                {
                    cubeInstList[i * len + j+k].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                    cubeThere[i * len + j+k] = 8; // change tile type
                }
            }
        }

        //right horizontal half double roads
        for (int i = len / 2; i < len; i++)
        {
            for (int j = Mathf.FloorToInt(len / 4.1f); j < len * 0.7f; j = j + len / 3)
            {
                for (int k = 0; k < widthOfDoubleRoads; k++)
                {
                    cubeInstList[i * len + j+k].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                    cubeThere[i * len + j+k] = 8; // change tile type
                }
            }
        }

        // horizontal border roads
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j = j + len - widthOfDoubleRoads)
            {
                for (int k = 0; k < widthOfDoubleRoads; k++)
                {
                    cubeInstList[i * len + (j + k)].GetComponent<Renderer>().material = IterToMaterial(8); // change material
                    cubeThere[i * len + (j + k)] = 8; // change tile type
                }
            }
        }

        //updating localscale value of roads
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                if (cubeThere[i * len + j] == 8)
                {
                    Vector3 s = cubeInstList[i * len + j].transform.localScale;

                    s.x = 1f;
                    s.y += 0.1f;
                    s.z = 1f;

                    cubeInstList[i * len + j].transform.localScale = s;
                }
            }
        }

        //
        //
        //
        //
        //
        //_____________adding river
        //
        //
        //
        //
        //

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                if (colorAvg[i * len + j + len * len * 7] * 100f < 10 && cubeThere[i * len + j] != 8) // river should be a minimum threashold. threashold value is based of water level
                {
                    cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(7); // change material
                    cubeThere[i * len + j] = 7; // change tile type
                    cubeInstList[i * len + j].transform.localScale = new Vector3(0.9f, 0.1f, 0.9f);
                }
            }
        }

        //
        //
        //
        //
        //
        //_____________seeding
        //
        //
        //
        //
        //

        float randFactor = 0.40f;
        float thresholdFactor = 0.9f;

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                /*
                if (k == 0) return blank;
                if (k == 1) return farmField;
                if (k == 2) return pollinators;
                if (k == 3) return prairie;
                if (k == 4) return riparianNoTree;
                if (k == 5) return riparianTree;
                if (k == 6) return upland;
                if (k == 7) return river;
                if (k == 8) return road;
                if (k == 9) return selector;
                 */
                //for (int k = 2; k < 7; k++)
                for (int k = 4; k < 6; k++)
                {
                    if (colorAvg[i * len + j + len * len * k] > thresholdFactor && Random.Range(0.0f, 1.0f) > randFactor && cubeThere[i * len + j] != 7 && cubeThere[i * len + j] != 8) // pollinator to upland seeding
                    {
                        //increment buffer count
                        if(cubeThere[i * len + j] == 1) buffercount++;

                        cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(k); // change material
                        cubeThere[i * len + j] = k; // change tile type

                        //change material color based on height
                        cubeInstList[i * len + j].GetComponent<MeshRenderer>().material.color = ColorFromHeight(colorAvg[len * len * 8 + i * len + j]);

                    }
                }
            }
        }


        //
        //
        //
        //
        //
        //_____________________________ compute shader initialization ________________________________
        //
        //
        //
        //
        //
        CSMain = computeShader.FindKernel("CSMain");
        computeShader.SetInt("len", len);
        computeShader.SetInt("fertilizerFlow", fertilizerFlow);
        computeShader.SetInt("season", season);

        // yeild texture initialization
        yeildTextureResult = new Texture2D(len, len);
        yeildRendTextureResult = new RenderTexture(len, len, 0);
        yeildRendTextureResult.enableRandomWrite = true;
        yeildRendTextureResult.Create();
        Graphics.Blit(yeildTextureResult, yeildRendTextureResult);
        computeShader.SetTexture(CSMain, "yeildTextureResult", yeildRendTextureResult);

        // minimap texture initialization
        minMapBoxTexture = new Texture2D(len, len);
        minMapBoxRendTexture = new RenderTexture(len, len, 0);
        minMapBoxRendTexture.enableRandomWrite = true;
        minMapBoxRendTexture.Create();
        Graphics.Blit(yeildTextureResult, minMapBoxRendTexture);
        computeShader.SetTexture(CSMain, "minMapTexture", minMapBoxRendTexture);

        // biodiversity texture initialization
        biodiversityTextureResult = new Texture2D(len, len);
        biodiversityRendTextureResult = new RenderTexture(len, len, 0);
        biodiversityRendTextureResult.enableRandomWrite = true;
        biodiversityRendTextureResult.Create();
        Graphics.Blit(biodiversityTextureResult, biodiversityRendTextureResult);
        computeShader.SetTexture(CSMain, "biodiversityTextureResult", biodiversityRendTextureResult);

        // biodiversity blur texture initialization
        biodiversityRendTextureBlurResult = new RenderTexture(len, len, 0);
        biodiversityRendTextureBlurResult.enableRandomWrite = true;
        biodiversityRendTextureBlurResult.Create();
        Graphics.Blit(biodiversityTextureResult, biodiversityRendTextureBlurResult);
        computeShader.SetTexture(CSMain, "biodiversityTextureBlurResult", biodiversityRendTextureBlurResult);

        // fertilizer texture initialization
        fertilizerTextureResult = new Texture2D(len, len);
        fertilizerRendTextureResult = new RenderTexture(len, len, 0);
        fertilizerRendTextureResult.enableRandomWrite = true;
        fertilizerRendTextureResult.Create();
        Graphics.Blit(fertilizerTextureResult, fertilizerRendTextureResult);
        computeShader.SetTexture(CSMain, "fertilizerTextureResult", fertilizerRendTextureResult);



        //
        //
        // creating compute buffers
        //
        //

        // tile compute buffer
        cubeThereBuffer = new ComputeBuffer(cubeThere.Length, sizeof(int));
        cubeThereBuffer.SetData(cubeThere);
        computeShader.SetBuffer(CSMain, "cubeThere", cubeThereBuffer);

        cubeBuffer = new ComputeBuffer(colorAvg.Length, sizeof(float));
        cubeBuffer.SetData(colorAvg);
        computeShader.SetBuffer(CSMain, "cubeBuffer", cubeBuffer);

        // run shader once
        RunShaderAndUpdateScores();

        //
        //
        //Setting default camera
        //
        //
        //

        projectiveCamera.tag = "Untagged";
        projectiveCamera.enabled = false;

        orthographicCamera.transform.position = projectiveCamera.transform.position;
        orthographicCamera.tag = "MainCamera";
        orthographicCamera.enabled = true;
        orthographicCamera.GetComponent<orthographicCameraControls>().ResetOrthographicSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (select < 0)
        {
            gameCube.transform.position = new Vector3(0, -10, 0);
        }
        if (select >= 0)
        {
            //change selector material
            //selector.mainTexture = IterToMaterial(select).mainTexture;
            selector = IterToMaterial(select);
            selector.color = new Color(0.9f, 0.9f, 1f, 1f);
            gameCube.GetComponent<Renderer>().material = selector;

            { //selector
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
                {
                    GameObject gameObject = hit.collider.gameObject;
                    if (gameObject.tag == "gameCube")
                    {
                        posi = Mathf.RoundToInt(gameObject.transform.position.x);
                        posj = Mathf.RoundToInt(gameObject.transform.position.z);

                        gameCube.transform.position = cubeInstList[posi * len + posj].transform.position;
                        gameCube.transform.localScale = cubeInstList[posi * len + posj].transform.localScale + new Vector3(0.01f,0.01f,0.01f);

                        if (cubeThere[posi * len + posj] != 8 && cubeThere[posi * len + posj] != 7)
                        {
                            gameCube.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.9f, 1f, 1f);
                        }
                        else
                        {
                            gameCube.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f, 1f);
                        }
                    }
                }
            }

            if (Input.GetMouseButton(1) && cursorType == 0)// point selector
            {
                if (cubeThere[posi * len + posj] !=8 && cubeThere[posi * len + posj] != 7 && select != -1)
                {
                    //increment buffer count
                    if (cubeThere[posi * len + posj] == 1 && select>1&& select < 7) buffercount++;

                    cubeInstList[posi * len + posj].GetComponent<Renderer>().material = IterToMaterial(select);
                    cubeThere[posi * len + posj] = select;
                    cubeThereBuffer.SetData(cubeThere);

                    //change material color based on height
                    cubeInstList[posi * len + posj].GetComponent<MeshRenderer>().material.color = ColorFromHeight(colorAvg[len * len * 8 + posi * len + posj]);

                    RunShaderAndUpdateScores();
                }
            }

            //
            //
            //
            // pan selector
            //
            //
            //
            if (Input.GetMouseButtonDown(1) && cursorType == 1)
            {
                Debug.Log("right click pressed down");
                posi_ = posi;
                posj_ = posj;
            }
            if (Input.GetMouseButton(1) && cursorType == 1)// point selector
            {
                int mini = posi_ <= posi ? posi_ : posi;
                int maxi = posi_ <= posi ? posi : posi_;
                int minj = posj_ <= posj ? posj_ : posj;
                int maxj = posj_ <= posj ? posj : posj_;


                for (int i = mini; i <= maxi; i++)
                {
                    for (int j = minj; j <= maxj; j++)
                    {
                        GameObject gameCubeinst = Instantiate(gameCube, new Vector3(i * 1.0f, 0, j * 1.0f), Quaternion.identity);
                        gameCubeinst.transform.localScale = cubeInstList[i * len + j].transform.localScale + new Vector3(0.01f, 0.01f, 0.01f);

                        if (cubeThere[i * len + j] != 8 && cubeThere[i * len + j] != 7)
                        {
                            gameCubeinst.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.9f, 1f, 1f);
                        }
                        else
                        {
                            gameCubeinst.GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f, 1f);
                        }
                        Destroy(gameCubeinst, Time.deltaTime * 4.0f + 0.01f);
                    }
                }
            }
            if (Input.GetMouseButtonUp(1) && cursorType == 1)
            {
                int mini = posi_ <= posi ? posi_ : posi;
                int maxi = posi_ <= posi ? posi : posi_;
                int minj = posj_ <= posj ? posj_ : posj;
                int maxj = posj_ <= posj ? posj : posj_;

                Debug.Log("right click pressed up");
                for (int i = mini; i <= maxi; i++)
                {
                    for (int j = minj; j <= maxj; j++)
                    {
                        if (cubeThere[i * len + j] != 8 && cubeThere[i * len + j] != 7 && select != -1)
                        {
                            //increment buffer count
                            if (cubeThere[i * len + j] == 1 && select > 1 && select < 7) buffercount++;

                            cubeInstList[i * len + j].GetComponent<Renderer>().material = IterToMaterial(select);
                            cubeThere[i * len + j] = select;
                            cubeThereBuffer.SetData(cubeThere);

                            //change material color based on height
                            cubeInstList[i * len + j].GetComponent<MeshRenderer>().material.color = ColorFromHeight(colorAvg[len * len * 8 + i * len + j]);
                        }
                    }
                }
                RunShaderAndUpdateScores();
                posi_ = posi;
                posj_ = posj;
            }

        }
        //
        //
        //Camera position on minimap
        //
        //
        //
        minMapBoxTextureFinal = new Texture2D(len,len);
        int _i = Mathf.RoundToInt(Camera.main.transform.position.x);
        int _j = Mathf.RoundToInt(Camera.main.transform.position.z);
        Color[] _pixels = minMapBoxTexture.GetPixels();
        
        for (int i = _i-2; (i < _i + 2) && (i>=0) && (i< minMapBoxTexture.width); i++)
        {
            for (int j = _j-2; (j < _j + 2) && (j >= 0) && (j < minMapBoxTexture.width); j++)
            {
                //minMapBoxTextureFinal.SetPixel(i, j, new Color(1f, 1f, 1f, 1f));
                _pixels[j * minMapBoxTexture.width + i] = new Color(1f, 1f, 1f, 1f);
            }
        }
        minMapBoxTextureFinal.SetPixels(_pixels);
        minMapBoxTextureFinal.Apply();
    }

    ////
    ////
    ////
    ////
    ////Compute Shader stuff
    ////
    ////
    ////
    ////

    void ComputerFertilizer()
    {
        fertilizerFlow = 1;
        computeShader.SetInt("fertilizerFlow", fertilizerFlow);

        for (int i = 0; i < 15; i++)
        {
            // run shader
            computeShader.Dispatch(CSMain, len, len, 1);
        }

        //read fertilizer texture
        fertilizerTextureResult = GetRTPixels(fertilizerRendTextureResult);
        fertilizerTextureResult.Apply();

        //read fertilizer red value and avg it
        Color[] color;
        float pixelAvg = 0f;
        float avgAccuracy = len / 8f;

        RenderTexture yeildAvg = new RenderTexture(Mathf.RoundToInt(avgAccuracy), Mathf.RoundToInt(avgAccuracy), 0);
        Graphics.Blit(fertilizerRendTextureResult, yeildAvg);

        color = GetRTPixels(yeildAvg).GetPixels();
        pixelAvg = 0f;
        for (int i = 0; i < color.Length; i++)
        {
            pixelAvg += (color[i].r - color[i].g) / avgAccuracy;
        }

        if (season == 2) fertilizerScore2 = pixelAvg * 100f;
        if (season == 3) fertilizerScore3 = pixelAvg * 100f;

        //read biodiversity texture
        biodiversityTextureResult = GetRTPixels(biodiversityRendTextureResult);
        biodiversityTextureResult.Apply();

        //read biodiversity value and avg it

        yeildAvg = new RenderTexture(Mathf.RoundToInt(avgAccuracy), Mathf.RoundToInt(avgAccuracy), 0);
        Graphics.Blit(biodiversityRendTextureResult, yeildAvg);

        color = GetRTPixels(yeildAvg).GetPixels();
        pixelAvg = 0f;
        for (int i = 0; i < color.Length; i++)
        {
            pixelAvg += color[i].r / avgAccuracy;
        }

        if (season == 2) biodiversityScore2 = pixelAvg * 100f;
        if (season == 3) biodiversityScore3 = pixelAvg * 100f;

        fertilizerFlow = 0;
        computeShader.SetInt("fertilizerFlow", fertilizerFlow);

    }


    void RunShaderAndUpdateScores()
    {
        // updating yeild multipliers
        computeShader.SetFloat("baseBiodiversity", baseBiodiversity);
        computeShader.SetFloat("biodiversityFlow", biodiversityFlow);
        computeShader.SetFloat("s1Yeild", s1Yeild);
        computeShader.SetFloat("s2Yeild", s2Yeild);
        computeShader.SetFloat("s3Yeild", s3Yeild);
        computeShader.SetFloat("s2Biodiversity", s2Biodiversity);
        computeShader.SetFloat("s3Biodiversity", s3Biodiversity);


        // run shader
        computeShader.Dispatch(CSMain, len, len, 1);
        
        //minimap stuff
        minMapBoxTexture = GetRTPixels(minMapBoxRendTexture);

        //read fertilizer texture
        fertilizerTextureResult = GetRTPixels(fertilizerRendTextureResult);


        //create biodiversity blur and set it
        spread = Mathf.Clamp(spread, 0, 7);
        RenderTexture biodiversityBlurAvg = new RenderTexture(len / (int)Mathf.Pow(2, spread ), len / (int)Mathf.Pow(2, spread ), 0);
        Graphics.Blit(biodiversityTextureResult, biodiversityBlurAvg);
        Graphics.Blit(biodiversityBlurAvg, biodiversityRendTextureBlurResult);

        // run shader again to read biodiversity blur texture
        computeShader.Dispatch(CSMain, len, len, 1);

        //read yeild value and avg it
        Color[] color;
        float pixelAvg = 0f;
        float avgAccuracy = len / 8f;
        yeildTextureResult = GetRTPixels(yeildRendTextureResult);
        yeildPlane.GetComponent<Renderer>().material.mainTexture = yeildTextureResult;

        RenderTexture yeildAvg = new RenderTexture(Mathf.RoundToInt(avgAccuracy), Mathf.RoundToInt(avgAccuracy), 0);
        Graphics.Blit(yeildTextureResult, yeildAvg);

        color = GetRTPixels(yeildAvg).GetPixels();
        pixelAvg = 0f;
        for (int i = 0; i < color.Length; i++)
        {
            pixelAvg += color[i].r / avgAccuracy;
        }

        if (season == 1) yeildScore1 = pixelAvg * 100f;
        if (season == 2) yeildScore2 = pixelAvg * 100f;
        if (season == 3) yeildScore3 = pixelAvg * 100f;


        //read biodiversity value and avg it
        biodiversityTextureResult = GetRTPixels(biodiversityRendTextureResult);
        biodiversityPlane.GetComponent<Renderer>().material.mainTexture = biodiversityTextureResult;

        RenderTexture biodiversityAvg = new RenderTexture(Mathf.RoundToInt(avgAccuracy), Mathf.RoundToInt(avgAccuracy), 0);
        Graphics.Blit(biodiversityTextureResult, biodiversityAvg);

        color = GetRTPixels(biodiversityAvg).GetPixels();
        pixelAvg = 0f;
        for (int i = 0; i < color.Length; i++)
        {
            pixelAvg += color[i].r / avgAccuracy;
        }

        if (season == 1) biodiversityScore1 = pixelAvg * 100f;
        if (season == 2) biodiversityScore2 = pixelAvg * 100f;
        if (season == 3) biodiversityScore3 = pixelAvg * 100f;


        biodiversityTextureBlurResult = GetRTPixels(biodiversityRendTextureBlurResult);
        biodiversityBlurPlane.GetComponent<Renderer>().material.mainTexture = biodiversityTextureBlurResult;

        //create a visual sum for both biodiversity and biodiversity sum
        color = biodiversityTextureResultBoth.GetPixels();
        Color[] color1 = biodiversityTextureResult.GetPixels();
        Color[] color2 = biodiversityTextureBlurResult.GetPixels();


        for (int i = 0; i < color1.Length; i++)
        {
            color[i] = color1[i] + color2[i];
        }
        biodiversityTextureResultBoth.SetPixels(color);
        biodiversityTextureResultBoth.Apply();

    }

    // guistuff
    void OnGUI()
    {
        // make the buffor selector box
        GUI.Box(new Rect(10, 10, 140, 270), "plant menue");
        // make the buffers
        if (GUI.Button(new Rect(20, 40, 120, 20), "crop"))
        {
            select = 1;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = yeildTextureInput;
        }
        if (GUI.Button(new Rect(20, 70, 120, 20), "pollinator"))
        {
            select = 2;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = pollinatorTextureInput;
        }
        if (GUI.Button(new Rect(20, 100, 120, 20), "prairie"))
        {
            select = 3;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = prairieTextureInput;
        }
        if (GUI.Button(new Rect(20, 130, 120, 20), "riparian (no tree)"))
        {
            select = 4;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = riparianNoTreeTextureInput;
        }
        if (GUI.Button(new Rect(20, 160, 120, 20), "riparian (tree)"))
        {
            select = 5;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = riparianTextureInput;
        }
        if (GUI.Button(new Rect(20, 190, 120, 20), "upland tree"))
        {
            select = 6;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = uplandTreeTextureInput;
        }
        if (GUI.Button(new Rect(20, 220, 120, 20), "remove"))
        {
            select = 0;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = backgroundPlaneTexture;
        }
        if (GUI.Button(new Rect(20, 250, 120, 20), "deselect"))
        {
            select = -1;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = backgroundPlaneTexture;
        }
        


        // Make a score box
        GUI.Box(new Rect(160, 10, 190, 160), "season 1 yeild: " + yeildScore1 + "\n season 1 biodiversity: " + biodiversityScore1 + 
            " \n\n season 2 yeild: " + yeildScore2 + "\n season 2 biodiversity: " + biodiversityScore2 + "\n season 2 fertilizer: " + fertilizerScore2 +
            " \n\n season 3 yeild: " + yeildScore3 + "\n season 3 biodiversity: " + biodiversityScore3 + "\n season 3 fertilizer: " + fertilizerScore3);

        // Make a season update box
        GUI.Box(new Rect(160, 180, 180, 180), "Season: " + season);

        // button to update box
        if (GUI.Button(new Rect(180, 210, 140, 20), "Update season"))
        {
            season = season % 3;
            season++;
            computeShader.SetInt("season", season);
            
            //updaiting float values
            spread = int.Parse(spreadString);
            baseBiodiversity = float.Parse(baseBiodiversityString);
            biodiversityFlow = float.Parse(biodiversityFlowString);
            s1Yeild = float.Parse(s1YeildString);
            s2Yeild = float.Parse(s2YeildString);
            s3Yeild = float.Parse(s3YeildString);
            s2Biodiversity = float.Parse(s2BiodiversityString);
            s3Biodiversity = float.Parse(s3BiodiversityString);


            RunShaderAndUpdateScores();
            ComputerFertilizer();

            if (select == -2)
            {
                backgroundPlane.GetComponent<Renderer>().material.mainTexture = yeildTextureResult;
            }
            if (select == -3)
            {
                backgroundPlane.GetComponent<Renderer>().material.mainTexture = biodiversityTextureResultBoth;
            }
            if (select == -4)
            {
                backgroundPlane.GetComponent<Renderer>().material.mainTexture = fertilizerTextureResult;
            }
        }
        // gui input for multiplication value
        baseBiodiversityString = GUI.TextField(new Rect(180, 240, 40, 20), baseBiodiversityString, 5);
        spreadString = GUI.TextField(new Rect(230, 240, 40, 20), spreadString, 5);
        biodiversityFlowString = GUI.TextField(new Rect(280, 240, 40, 20), biodiversityFlowString, 5);
        s1YeildString = GUI.TextField(new Rect(180, 270, 60, 20), s1YeildString, 5);
        s2YeildString = GUI.TextField(new Rect(180, 300, 60, 20), s2YeildString, 5);
        s2BiodiversityString = GUI.TextField(new Rect(260, 300, 60, 20), s2BiodiversityString, 5);
        s3YeildString = GUI.TextField(new Rect(180, 330, 60, 20), s3YeildString, 5);
        s3BiodiversityString = GUI.TextField(new Rect(260, 330, 60, 20), s3BiodiversityString, 5);


        // Make a cursor box
        GUI.Box(new Rect(360, 10, 180, 100), "Cursor");

        if (GUI.Button(new Rect(380, 40, 140, 20), "point"))
        {
            cursorType = 0;
        }

        if (GUI.Button(new Rect(380, 70, 140, 20), "box"))
        {
            cursorType = 1;
        }

        // Make a camera box
        GUI.Box(new Rect(360, 120, 180, 40), "");

        if (GUI.Button(new Rect(380, 130, 140, 20), "change camera"))
        {
            cameraType++;
            cameraType = cameraType % 2;

            if(cameraType == 0)
            {
                projectiveCamera.tag = "Untagged";
                projectiveCamera.enabled = false;

                orthographicCamera.transform.position = projectiveCamera.transform.position;
                orthographicCamera.tag = "MainCamera";
                orthographicCamera.enabled = true;
                orthographicCamera.GetComponent<orthographicCameraControls>().ResetOrthographicSize();
            }
            if (cameraType == 1)
            {
                orthographicCamera.tag = "Untagged";
                orthographicCamera.enabled = false;

                projectiveCamera.transform.position = orthographicCamera.transform.position;
                projectiveCamera.tag = "MainCamera";
                projectiveCamera.enabled = true;
                projectiveCamera.GetComponent<cameraControls>().ResetRotAndHeight();
            }
        }
        

        // Make a percentage box
        GUI.Box(new Rect(560, 10, 140, 40), "Buffer Percentage:\n" + ((float)buffercount / (len * len)));

        // Make a effect maps box
        GUI.Box(new Rect(560, 60, 140, 120), "chose effects");
        if (GUI.Button(new Rect(570, 90, 120, 20), "yeild"))
        {
            select = -2;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = yeildTextureResult;
        }
        if (GUI.Button(new Rect(570, 120, 120, 20), "biodiversity"))
        {
            select = -3;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = biodiversityTextureResultBoth;
        }
        if (GUI.Button(new Rect(570, 150, 120, 20), "fertilizer"))
        {
            select = -4;
            backgroundPlane.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
            backgroundPlane.GetComponent<Renderer>().material.mainTexture = fertilizerTextureResult;
        }


        //selector box
        //GUI.DrawTexture(panBoxRect, panBoxTexture);
        //minimap box
        GUI.DrawTexture(minMapBoxRect, minMapBoxTextureFinal);

        

        //exiting game
        if (GUI.Button(new Rect(Screen.width - 100, 10, 90, 30), "Quit game"))
        {
            Application.Quit();
        }
    }

    Color ColorFromHeight(float colorAvg)
    {
        //change material color based on height
        float c = colorAvg * 0.5f + 0.5f;
        return new Color(c, c, c, 1f);
    }
    Material IterToMaterial(int k)
    {
        
        if (k == 0) return blank;
        if (k == 1) return farmField;
        if (k == 2) return pollinators;
        if (k == 3) return prairie;
        if (k == 4) return riparianNoTree;
        if (k == 5) return riparianTree;
        if (k == 6) return upland;
        if (k == 7) return river;
        if (k == 8) return road;
        if (k == 9) return selector;
        return blank;
    }

    static public Texture2D GetRTPixels(RenderTexture rt)
    {
        // Remember currently active render texture
        RenderTexture currentActiveRT = RenderTexture.active;

        // Set the supplied RenderTexture as the active one
        RenderTexture.active = rt;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(rt.width, rt.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();

        // Restorie previously active render texture
        RenderTexture.active = currentActiveRT;
        return tex;
    }

}

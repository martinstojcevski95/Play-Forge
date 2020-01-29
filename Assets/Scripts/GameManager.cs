﻿

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera TopViewCamera;

    public List<SinglePlayer> allPlayers = new List<SinglePlayer>();

    public static GameManager Instance;
    public PathCreator pathCreator;
    public GameObject PlayerPrefab, DefensePrefab;

    Vector3 Camera3DView, CameraDefaultView;
    Quaternion Camera3DRotation, CameraDefaultRotation;
    Transform newPlayerPosition;
    Transform newDefensePlayerPosition;

    public Image LoadinPanel;

    public List<string> FormationsName = new List<string>();

    public List<string> PlaysNames = new List<string>();
    public Dropdown PlayNames;
    public Dropdown FormationNames;
    public float LoadingSpeed;
    public int SelectedPlayerID;

    public Color PlayerSelectedColorHolder;
    public int PlayersCount;
    public string CurrentLinkedName;
    //  public LinkedFormationsAndPlays linkedNames;
    // string formationameHolder;
    public GameObject ArrowPref, BlockPref;
    //public List<int> LinkedPlayerPointerID = new List<int>();
    // int howMuchPointersShouldBeSpawned;
    // public string PointerType;
    public AllFormations allFormations;
    public int formationCounter;
    public int playsCounter;
    public int SelectedFormation;
    public Dropdown FORMATIONS, PLAYS;
    // string FORMATIONNAMEHOLDER, PLAYNAMEHOLDER;
    public bool CanDrawIfSelected;


    public Camera ScreenShootCamera;
    bool takeScreenShotOnNextFrame;

    int selectedFormationIDForPreview, selectedPlayIDForPreview;


    [SerializeField]
    InputField placeHolder;

    [SerializeField]
    PlaysNameHolder playsNamesHolder;

    void Awake()
    {


        LoadingSpeed = 1 / 1.8f;
        Instance = this;
        formationCounter = allFormations.AllFormmations.Count;
        //   playsNamesHolder.PlaysNamesHolder.Add("Default");
        // string js = JsonUtility.ToJson(playsNamesHolder);
        // File.WriteAllText(Application.dataPath + "/PlaysNamesHolder.json", js);
    }



    // Use this for initialization
    void Start()
    {
        LoadFullData();

        CameraDefaultView = Camera.main.transform.position;
        CameraDefaultRotation = Camera.main.transform.rotation;

        newPlayerPosition = GameObject.FindGameObjectWithTag("newplayerpos").transform;
        newDefensePlayerPosition = GameObject.FindGameObjectWithTag("newdefenseplayerpos").transform;
        Camera3DView = GameObject.FindGameObjectWithTag("3DView").transform.position;
        Camera3DRotation = GameObject.FindGameObjectWithTag("3DView").transform.rotation;
        var cam = FindObjectOfType<Camera>();
        if (cam != null)
        {
            if (cam.tag == "ScreenShootCam")
            {
                ScreenShootCamera = cam;
            }
        }

        PLAYS.ClearOptions();
        PLAYS.AddOptions(new List<string> { "Choose Play" });
        FORMATIONS.ClearOptions();
        FORMATIONS.AddOptions(new List<string> { "Choose formation" });


        GetAllDataAtStart();

    }



    //void OnPostRender()
    //{
    //    if (takeScreenShotOnNextFrame)
    //    {
    //        takeScreenShotOnNextFrame = false;
    //        RenderTexture renderTexture = ScreenShootCamera.targetTexture;

    //        Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);

    //        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
    //        renderResult.ReadPixels(rect, 0, 0);
    //        byte[] byteArray = renderResult.EncodeToPNG();
    //        if (Application.isEditor)
    //        {
    //            File.WriteAllBytes(Application.dataPath + "/" + screenshotNameHolder + ".png", byteArray);// for editor test
    //        }
    //        else
    //        {
    //            File.WriteAllBytes(Application.dataPath + "/Resources" + "/colson.png", byteArray); // when releasing a build

    //        }


    //        Debug.Log("screenshot has been taken!");
    //        RenderTexture.ReleaseTemporary(renderTexture);
    //        ScreenShootCamera.targetTexture = null;

    //    }
    //}


    //loading this way
    //byte[] test = File.ReadAllBytes(Application.dataPath + "/test.png");
    //temp = new Texture2D(500, 500);
    //temp.LoadImage(test);
    //        WTF.texture = temp;



    //void TakeScreenShot(int height, int width)
    //{

    //    ScreenShootCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
    //    takeScreenShotOnNextFrame = true;
    //}

    /// <summary>
    /// Using this. Loading the full data
    /// </summary>
    public void LoadFullData()
    {
        FormationsName.Clear();
        StartCoroutine(LateLoadEverything());

    }


    /// <summary>
    /// Using this. Saving the formation
    /// </summary>
    /// <param name="formationname"></param>
    public void SavingFormation(InputField formationname)
    {
        var formation = new Formation();
        formation.FormationName = formationname.text;

        formation.FormationID = allFormations.AllFormmations.Count;
        allFormations.AllFormmations.Add(formation);
        //     var players = FindObjectsOfType<SinglePlayer>();

        //   DrawingMode(); // call this on create new formation

        //    StartMovingThePlayers(true);
    }

    public void SaveDefaultPlayOnFormationReset()
    {
        // UIManager.Instance.SaveFormationPopUp.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        SavingPlay(placeHolder);
    }

    public void StartNewPlayWithOnlyDrawing()
    {
        UIManager.Instance.NewFormatonAndPlay.DOAnchorPos(new Vector2(0, 1000), 0.5f);
        DrawingMode();
    }


    /// <summary>
    /// Using this. Saving the play
    /// </summary>
    /// <param name="playname"></param>
    void SavingPlay(InputField playname)
    {

        var play = new Play();
        formationCounter = allFormations.AllFormmations.Count;
        play.PlayName = playname.text;
        play.PlayID = allFormations.AllFormmations[formationCounter - 1].FormationID;

        var players = FindObjectsOfType<SinglePlayer>();
        playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;

        if (allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation[playsCounter - 1].PlayName == playname.text)
        {
            UIManager.Instance.SelectTextType("Play with this name already exists, please choose different name", "warning", 2f);

            Debug.Log("play like this already exists");
        }
        else
        {
            //adding this play because there isn't a play with this name
            allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(play);
        }
        if (players != null)
        {
            foreach (var item in players)
            {

                item.Populate(formationCounter - 1, playsCounter - 1);

            }
        }
        playsNamesHolder.PlaysNamesHolder.Add(play.PlayName);
        formationCounter += 1;
        StartCoroutine(LateSaveEverything());

        //if (!playsNamesHolder.PlaysNamesHolder.Contains(playname.text))
        //{

        //    if (playname.text == "")
        //    {
        //        var play = new Play();
        //        formationCounter = allFormations.AllFormmations.Count;
        //        play.PlayName = allFormations.AllFormmations[formationCounter - 1].FormationName + " Formation";
        //        play.PlayID = allFormations.AllFormmations[formationCounter - 1].FormationID;
        //        allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(play);
        //        var players = FindObjectsOfType<SinglePlayer>();
        //        playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;
        //        if (players != null)
        //        {
        //            foreach (var item in players)
        //            {

        //                item.Populate(formationCounter - 1, playsCounter - 1);

        //            }
        //        }
        //        playsNamesHolder.PlaysNamesHolder.Add(play.PlayName);
        //        formationCounter += 1;
        //        StartCoroutine(LateSaveEverything());
        //    }
        //    else
        //    {
        //        var play = new Play();
        //        formationCounter = allFormations.AllFormmations.Count;
        //        play.PlayName = playname.text;
        //        play.PlayID = allFormations.AllFormmations[formationCounter - 1].FormationID;
        //        allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(play);
        //        var players = FindObjectsOfType<SinglePlayer>();
        //        playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;
        //        if (players != null)
        //        {
        //            foreach (var item in players)
        //            {

        //                item.Populate(formationCounter - 1, playsCounter - 1);

        //            }
        //        }
        //        playsNamesHolder.PlaysNamesHolder.Add(play.PlayName);
        //        formationCounter += 1;
        //        StartCoroutine(LateSaveEverything());
        //    }
        //}
        //else
        //{
        //    UIManager.Instance.SelectTextType("Play with this name already exists, please choose different name", "warning", 2f);
        //}





        //var play = new Play();
        //play.PlayName = playname.text;
        //formationCounter = allFormations.AllFormmations.Count;
        //play.PlayID = allFormations.AllFormmations[formationCounter - 1].FormationID;
        //allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Add(play);
        //var players = FindObjectsOfType<SinglePlayer>();
        //playsCounter = allFormations.AllFormmations[formationCounter - 1].LinkedPlaysWithFormation.Count;
        //if (players != null)
        //{
        //    foreach (var item in players)
        //    {

        //        item.Populate(formationCounter - 1, playsCounter - 1);

        //    }
        //}

        //formationCounter += 1;
        //StartCoroutine(LateSaveEverything());


    }


    void ResetPlayersForNewDrawAfterPlayIsSaved()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].playerStats.Points.Clear();
            allPlayers[i].playerStats.PointerRotation = new Quaternion(0, 0, 0, 0);
            allPlayers[i].GetComponent<LineRenderer>().positionCount = 0;
        }
        pathCreator.GetComponent<LineRenderer>().positionCount = 0;
        pathCreator.PointerHolder.SetActive(false);
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                Destroy(item.transform.parent.gameObject);
            }
        }
    }

    public void RecenterCamerView()
    {
        Camera.main.transform.position = CameraDefaultView;
        Camera.main.transform.rotation = CameraDefaultRotation;
    }

    public void GoBackToDefaultViewAfterSavingFormationAndPlay()
    {
        CameraMovement.Instance.isPanning = true;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].GetComponent<LineRenderer>().positionCount = 0;
            allPlayers[i].playerNav.enabled = false;
            allPlayers[i].pathMover.enabled = false;
            allPlayers[i].playerStats.Points.Clear();
            allPlayers[i].transform.position = allPlayers[i].playerStats.StartingPlayerLocalPosition;
            allPlayers[i].playerStats.CanMove = false;

        }

        pathCreator.GetComponent<LineRenderer>().positionCount = 0;
        pathCreator.enabled = false;
        pathCreator.PointerHolder.SetActive(false);
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                Destroy(item.transform.parent.gameObject);
            }
        }
        Camera.main.transform.position = CameraDefaultView;
        Camera.main.transform.rotation = CameraDefaultRotation;
        UIManager.Instance.View3D.interactable = false;
        UIManager.Instance.CloseAllPopUps();
    }


    /// <summary>
    /// Using this. Populating UI data for the formations and plays from the JSON 
    /// </summary>
    void PopulateDropdownDataForFormationsAndPlays()
    {
        //var info = new DirectoryInfo(Application.dataPath);
        //var fileinfo = info.GetFiles("*.png");

        //foreach (var item in fileinfo)
        //{
        //    string[] x = item.Name.Split('.');

        //    foreach (var s in x)
        //    {
        //        if (s != "png")
        //        {
        //            Debug.Log(s);
        //            ScreenShotsNames.Add(s);
        //        }
        //    }
        //}

        for (int i = 0; i < allFormations.AllFormmations.Count; i++)
        {

            FormationsName.Add(allFormations.AllFormmations[i].FormationName);
        }
        FORMATIONS.ClearOptions();
        FORMATIONS.AddOptions(FormationsName);



    }


    public void FormationPreviewForPlays(Toggle selectedItem)
    {

        foreach (var item in allFormations.AllFormmations)
        {
            if (item.FormationName.Contains(selectedItem.GetComponentInChildren<Text>().text))
            {
                selectedFormationIDForPreview = item.FormationID;
            }

        }
    }



    //byte[] test = File.ReadAllBytes(Application.dataPath + "/test.png");
    //temp = new Texture2D(500, 500);
    //temp.LoadImage(test);
    //        WTF.texture = temp;

    /// <summary>
    /// Using this. Switching the field of the view 2D/3D
    /// </summary>
    /// <param name="t"></param>
    public void SwitchCameraView(bool t)
    {
        if (t)
        {

            Camera.main.transform.position = Camera3DView;
            Camera.main.transform.rotation = Camera3DRotation;
            foreach (var item in allPlayers)
            {
                item.playerNav.enabled = true;
                item.pathMover.enabled = true;
                item.pathMover.SetPoints(item.playerStats.Points);
                //item.renderer.SetPosition(0, new Vector3(-100, -100, 0));
            }


        }
        else
        {

            Camera.main.transform.position = CameraDefaultView;
            Camera.main.transform.rotation = CameraDefaultRotation;
            foreach (var item in allPlayers)
            {
                item.playerNav.enabled = false;
                item.pathMover.enabled = false;
                item.transform.position = item.playerStats.PlayerLocalPosition;

            }
        }


    }




    /// <summary>
    /// Using this. Loading formation data
    /// </summary>
    /// <param name="formation"></param>
    public void OnSelectedFormation(Dropdown formation)
    {
        int menuIndex = formation.GetComponent<Dropdown>().value;
        var formationname = formation.options[menuIndex].text;


        foreach (var item in allFormations.AllFormmations)
        {
            if (item.FormationName == formationname)
            {

                Debug.Log(item.FormationID);
                SelectedFormation = item.FormationID;
                var playsCount = allFormations.AllFormmations[item.FormationID].LinkedPlaysWithFormation.Count;
                PlaysNames.Clear();
                for (int i = 0; i < playsCount; i++)
                {
                    PlaysNames.Add(allFormations.AllFormmations[item.FormationID].LinkedPlaysWithFormation[i].PlayName);

                }

            }

        }


        PLAYS.ClearOptions();
        PLAYS.AddOptions(new List<string> { "Choose Play" });
        PLAYS.AddOptions(PlaysNames);
        UIManager.Instance.LoadFormation(false);
    }

    void Update()
    {
        //if (inPreview)
        //{
        //    for (int i = 0; i < allPlayers.Count; i++)
        //    {
        //        allPlayers[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].Points;
        //        allPlayers[i].LoadPlayData();
        //    }
        //    Debug.Log("dataa");
        //    inPreview = false;
        //}

    }

    public void PlayPreview(Toggle item)
    {

        //var i = item.GetComponentInChildren<Text>().text;
        //var plays = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation;
        //foreach (var ss in plays)
        //{
        //    if (ss.PlayName.Contains(i))
        //    {
        //        Debug.Log(ss.PlayName);
        //        selectedPlayIDForPreview = ss.PlayID;
        //    }
        //}

        //inPreview = true;
    }

    public void GetPlayIDValue(Dropdown play)
    {

        //selectedPlayIDForPreview = play.GetComponent<Dropdown>().value;
        //Debug.Log(allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].PlayName);
        ////   Debug.Log(allFormations.AllFormmations[selectedFormationIDForPreview].LinkedPlaysWithFormation[selectedPlayIDForPreview].PlayName);
        //int playersCountForPlay = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays.Count;
        ////Debug.Log(" Selecting play on  hover " + allFormations.AllFormmations[selectedFormationIDForPreview].LinkedPlaysWithFormation[selectedPlayIDForPreview].PlayName);

        //for (int i = 0; i < allPlayers.Count; i++)
        //{
        //    allPlayers[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].Points;
        //    allPlayers[i].LoadPlayData();
        //}
        //var players = FindObjectsOfType<SinglePlayer>();
        //if (players != null)
        //{
        //    for (int i = 0; i < playersCountForPlay; i++)
        //    {
        //        if (allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].PlayerID == players[i].playerStats.PlayerID)
        //        {
        //            players[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].Points;
        //            players[i].playerStats.PlayerPointerType = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].PlayerPointerType;
        //            players[i].playerStats.PointerPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].PointerPosition;
        //            players[i].playerStats.PointerRotation = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].PointerRotation;
        //            players[i].playerStats.PlayerLocalPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[selectedPlayIDForPreview].LinkedPlayersWithPlays[i].PlayerLocalPosition;
        //            players[i].LoadPlayData();
        //            players[i].LoadFormationData();

        //        }
        //    }
        //}
    }


    public void OnSelectedPlayPreview(Dropdown play)
    {
        int menuIndex = play.GetComponent<Dropdown>().value - 1;
        int playersCountForPlay = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays.Count;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerID == allPlayers[i].playerStats.PlayerID)
            {

                allPlayers[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].Points;
                allPlayers[i].playerStats.PlayerPointerType = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerPointerType;
                allPlayers[i].playerStats.PointerPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerPosition;
                allPlayers[i].playerStats.PointerRotation = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerRotation;
                allPlayers[i].playerStats.PlayerLocalPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerLocalPosition;
                allPlayers[i].LoadPlayData();
                allPlayers[i].LoadFormationData();

            }
        }
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                DestroyImmediate(item.gameObject);
            }
        }

    }


    public void DeleteLineForSelectedPlayer()
    {



        allPlayers[SelectedPlayerID].GetComponent<LineRenderer>().positionCount = 0;
        allPlayers[SelectedPlayerID].GetComponent<LineRenderer>().positionCount = 0;
        allPlayers[SelectedPlayerID].playerStats.Points = null;
        pathCreator.lineRenderer.positionCount = 0;
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                if (item.transform.parent != null)
                {
                    DestroyImmediate(item.transform.parent.gameObject);
                }
                else
                    DestroyImmediate(item.gameObject);
            }
        }

    }


    /// <summary>
    /// Using this. Loading play data
    /// </summary>
    /// <param name="play"></param>
    public void OnSelectedPlay(Dropdown play)
    {
        int menuIndex = play.GetComponent<Dropdown>().value - 1; // because there is choose plays value at index 0;
        var playname = play.options[menuIndex].text;
        Debug.Log("menu index " + menuIndex);
        int playersCountForPlay = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays.Count;
        var pointers = FindObjectsOfType<Pointer>();
        if (pointers != null)
        {
            foreach (var item in pointers)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        var players = FindObjectsOfType<SinglePlayer>();
        if (players != null)
        {
            for (int i = 0; i < playersCountForPlay; i++)
            {
                if (allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerID == players[i].playerStats.PlayerID)
                {

                    players[i].playerStats.Points = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].Points;
                    players[i].playerStats.PlayerPointerType = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerPointerType;
                    players[i].playerStats.PointerPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerPosition;
                    players[i].playerStats.PointerRotation = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PointerRotation;
                    players[i].playerStats.PlayerLocalPosition = allFormations.AllFormmations[SelectedFormation].LinkedPlaysWithFormation[menuIndex].LinkedPlayersWithPlays[i].PlayerLocalPosition;
                    players[i].LoadPlayData();
                    players[i].LoadFormationData();

                }
            }
        }

    }

    /// <summary>
    /// Using this. Enable drawing for each player after a formation is saved
    /// </summary>
    public void DrawingMode()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].playerStats.CanMove = true;
            allPlayers[i].playerNav.enabled = false;
            allPlayers[i].pathMover.enabled = false;
            allPlayers[i].GetComponentInChildren<MeshRenderer>().enabled = false;

        }
        UIManager.Instance.View3D.interactable = true;
        pathCreator.enabled = true;

    }


    /// <summary>
    /// Using this. Save formation and play data after a play is saved  
    /// </summary>
    /// <returns></returns>
    IEnumerator LateSaveEverything()
    {
        yield return new WaitForSeconds(.5f);
        string jsonData = JsonUtility.ToJson(allFormations);
        File.WriteAllText(Application.dataPath + "/Formations-Plays.json", jsonData);
        string JsonData = JsonUtility.ToJson(playsNamesHolder);
        File.WriteAllText(Application.dataPath + "/PlaysNamesHolder.json", JsonData);
        yield return new WaitForSeconds(1f);
        ResetPlayersForNewDrawAfterPlayIsSaved();
        StartCoroutine(LateLoadEverything());
        // GoBackToDefaultViewAfterSavingFormationAndPlay();


        //  PopulateDropdownDataForFormationsAndPlays();
    }

    IEnumerator LateLoadEverything()
    {
        yield return new WaitForSeconds(.2f);
        string path = Application.dataPath + "/Formations-Plays.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            allFormations = JsonUtility.FromJson<AllFormations>(json);

            PopulateDropdownDataForFormationsAndPlays();
        }


        string playsHolderPath = Application.dataPath + "/PlaysNamesHolder.json";
        if (File.Exists(playsHolderPath))
        {
            string json = File.ReadAllText(playsHolderPath);
            playsNamesHolder = JsonUtility.FromJson<PlaysNameHolder>(json);
        }
    }


    public void SaveFormationDataAndDrawForPlay(InputField savedFormationName)
    {

        SavingFormation(savedFormationName);
    }

    public void SavePlayAndFormationAfterDrawing(InputField savedPlayName)
    {

        SavingPlay(savedPlayName);
    }



    //IEnumerator GetPlaysData()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    string path = Application.dataPath + "/PlayNames.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);

    //        plays = JsonUtility.FromJson<Plays>(json);
    //        PlayNames.AddOptions(plays.PlaylISTNames);
    //    }

    //}

    //IEnumerator GetFormationsData()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    string path = Application.dataPath + "/FormationNames.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        formations = JsonUtility.FromJson<Formations>(json);
    //        FormationNames.AddOptions(formations.FormationListNames);

    //    }

    //}




    //public void GetDataForSeletedFormation(Dropdown FormationName)
    //{


    //    if (FormationNames.value == 0)
    //        UIManager.Instance.SelectTextType("Please  select  formation  from  the  dropdown", "warning", 5f);
    //    else
    //        UIManager.Instance.SelectTextType("", "none", 0f);


    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        int menuIndex = FormationNames.GetComponent<Dropdown>().value;
    //        var formationname = FormationNames.options[menuIndex].text;
    //        string output = formationname.Substring(formationname.IndexOf(',') + 1);
    //        allPlayers[i].GetFormation(output);
    //        formationameHolder = output;
    //    }
    //    StartMovingThePlayers(true);
    //    CurrentLinkedName = formationameHolder;
    //    StartCoroutine((CheckForLinkedPlays(formationameHolder)));
    //}

    //public void GetDataForSeletedPlay(Dropdown PlayName)
    //{


    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        int menuIndex = PlayNames.GetComponent<Dropdown>().value;
    //        var playname = PlayName.options[menuIndex].text;
    //        string output = playname.Substring(playname.IndexOf(',') + 1);

    //        allPlayers[i].GetPlay(output);
    //    }
    //    SpawnPointersAfterPlayIsLoaded();
    //}

    //void SavePointerData()
    //{
    //    var pointers = FindObjectsOfType<Pointer>();
    //    foreach (var item in pointers)
    //    {
    //        item.SavePointerPositionAndRotation();
    //    }

    //    // reload the formation after saving a play

    //    StartCoroutine(ReloadFormationAfterSavingAPlay());
    //}

    //IEnumerator ReloadFormationAfterSavingAPlay()
    //{
    //    yield return new WaitForSeconds(2f);
    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        allPlayers[i].singlePlayer.points.Clear();
    //        allPlayers[i].singlePlayer.PointerPositionOnly = new Vector3(0, 0, 0);
    //        allPlayers[i].GetFormation(CurrentLinkedName);
    //    }
    //    var pointers = FindObjectsOfType<Pointer>();
    //    foreach (var item in pointers)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    StartMovingThePlayers(true);
    //    pathCreator.enabled = false;
    //    CurrentLinkedName = formationameHolder;
    //    //  StartCoroutine((CheckForLinkedPlays(formationameHolder)));
    //}
    //IEnumerator SearchForDrawingAfterLoadingPlay()
    //{

    //    var pointers = FindObjectsOfType<Pointer>();
    //    foreach (var item in pointers)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    yield return new WaitForSeconds(.2f);
    //    LinkedPlayerPointerID.Clear();
    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        if (allPlayers[i].singlePlayer.points.Count >= 1)
    //        {

    //            LinkedPlayerPointerID.Add(allPlayers[i].singlePlayer.PlayerID);
    //        }
    //    }
    //    howMuchPointersShouldBeSpawned = LinkedPlayerPointerID.Count - 1;
    //    while (howMuchPointersShouldBeSpawned >= 2)
    //    {
    //        // if (PointerType == "arrow")
    //        // {
    //        var newArrow = Instantiate(ArrowPref, new Vector3(0, 0, 0), Quaternion.identity);
    //        // newArrow.SetActive(false);
    //        newArrow.AddComponent<Pointer>();
    //        howMuchPointersShouldBeSpawned--;
    //        // }
    //        //else if (PointerType == "block")
    //        //{
    //        //    var newBlock = Instantiate(BlockPref, new Vector3(0, 0, 0), Quaternion.identity);
    //        //    newBlock.SetActive(true);
    //        //    newBlock.AddComponent<Pointer>();
    //        //    howMuchPointersShouldBeSpawned--;
    //        //}

    //    }
    //    WaitForTheSpawnedPointers();
    //    //    StartCoroutine(WaitForTheSpawnedPointers());
    //}

    //void WaitForTheSpawnedPointers()
    //{
    //    var pointers = FindObjectsOfType<Pointer>();

    //    for (int i = 0; i < pointers.Length; i++)
    //    {
    //        pointers[i].SelectedPlayerPointerID = LinkedPlayerPointerID[i];
    //        pointers[i].LoadPointerPositionAndRotation();
    //        Debug.Log(pointers[i].name);
    //    }
    //}

    //IEnumerator WaitForTheSpawnedPointers()
    //{

    //    yield return new WaitForSeconds(.2f);
    //    var pointers = FindObjectsOfType<Pointer>();

    // //   yield return new WaitForSeconds(.4f);
    //    for (int i = 0; i < pointers.Length; i++)
    //    {
    //        pointers[i].SelectedPlayerPointerID = LinkedPlayerPointerID[i];
    //     //   yield return new WaitForSeconds(.2f);
    //        pointers[i].LoadPointerPositionAndRotation();
    //    }
    //}

    //public void SpawnPointersAfterPlayIsLoaded()
    //{
    //    StartCoroutine(SearchForDrawingAfterLoadingPlay());
    //}





    //void SavePlayListNames(string playName)
    //{
    //    plays.PlaylISTNames.Add(playName);
    //    string jsonData = JsonUtility.ToJson(plays);
    //    File.WriteAllText(Application.dataPath + "/PlayNames.json", jsonData);
    //    PlayNames.ClearOptions();
    //    StartCoroutine(GetPlaysData());
    //}



    //void SaveFormationListNames(string formationName)
    //{
    //    formations.FormationListNames.Add(formationName);
    //    string jsonData = JsonUtility.ToJson(formations);
    //    File.WriteAllText(Application.dataPath + "/FormationNames.json", jsonData);
    //    FormationNames.ClearOptions();
    //    StartCoroutine(GetFormationsData());
    //}





    //savving linked data for formation + play
    //public void SaveLinkedData()
    //{
    //    linkedNames.LinkedNames.Add(CurrentLinkedName);
    //    string jsonData = JsonUtility.ToJson(linkedNames);
    //    File.WriteAllText(Application.dataPath + "/LinkedNames.json", jsonData);
    //    string formationOnly = CurrentLinkedName.Remove(CurrentLinkedName.IndexOf('-'));
    //    CurrentLinkedName = "";
    //    CurrentLinkedName = formationOnly;
    //    Debug.Log("formation name only " + formationOnly);
    //    StartCoroutine(GetLinkedData());

    //}

    //// coroutine for getting linked data from json file
    //IEnumerator GetLinkedData()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    string path = Application.dataPath + "/LinkedNames.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        linkedNames = JsonUtility.FromJson<LinkedFormationsAndPlays>(json);

    //    }

    //}


    //// coroutine for checking linked plays and formation after a formation is choosen from the dropdown
    //IEnumerator CheckForLinkedPlays(string formationName)
    //{
    //    PlayNames.ClearOptions();
    //    plays.PlaylISTNames.Clear();
    //    foreach (var item in linkedNames.LinkedNames)
    //    {

    //        string formationOnly = item.Remove(item.IndexOf('-'));
    //        if (item.Remove(item.IndexOf('-')) == formationName)
    //        {
    //            string output = item.Substring(item.IndexOf('-') + 1);

    //            plays.PlaylISTNames.Add(output);
    //            Debug.Log(output);
    //            yield return new WaitForSeconds(.2f);

    //        }
    //    }
    //    var pointers = FindObjectsOfType<Pointer>();
    //    foreach (var item in pointers)
    //    {
    //        Destroy(item.gameObject);
    //    }
    //    PlayNames.AddOptions(plays.PlaylISTNames);
    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        allPlayers[i].singlePlayer.PointerPositionOnly = new Vector3(0, 0, 0);
    //        allPlayers[i].singlePlayer.points.Clear();
    //    }
    //}



    // saving formation data
    //public void SaveFormation(InputField formationName)
    //{
    //    if (formationName.text != "")
    //    {
    //        CurrentLinkedName = formationName.text;

    //        for (int i = 0; i < allPlayers.Count; i++)
    //        {
    //            allPlayers[i].FormationSave(formationName.text);
    //        }
    //        if (formations.FormationListNames.Contains(formationName.text))
    //        {
    //            formations.FormationListNames.Remove(formationName.text);
    //            Debug.Log("adding formation with same name ");

    //        }
    //        SaveFormationListNames(formationName.text);
    //        StartCoroutine(LoadFormationDataAfterSaving(formationName.text));
    //    }
    //    else
    //        UIManager.Instance.SelectTextType("Please   give  the   formation  name", "warning", 5f);

    //}

    //coroutine for loading formation data after saving  and going into play drawing
    //IEnumerator LoadFormationDataAfterSaving(string formationName)
    //{
    //    CameraMovement.Instance.DisablePanning();
    //    yield return new WaitForSeconds(0.3f);
    //    for (int i = 0; i < allPlayers.Count; i++)
    //    {
    //        allPlayers[i].GetFormation(formationName);
    //        formationameHolder = formationName;
    //    }
    //    StartMovingThePlayers(true);
    //    CurrentLinkedName = formationameHolder;
    //    StartCoroutine((CheckForLinkedPlays(formationameHolder)));
    //}




    /// <summary>
    /// Using this. Gtting and setting the players into list and the path creator
    /// </summary>
    void GetAllDataAtStart()
    {

        var foundPlayer = FindObjectsOfType<SinglePlayer>();
        for (int i = 0; i < foundPlayer.Length; i++)
        {
            foundPlayer[i].playerStats.PlayerID = i;
            foundPlayer[i].playerNav.enabled = false;
            foundPlayer[i].pathMover.enabled = false;
            foundPlayer[i].GetComponentInChildren<MeshRenderer>().enabled = false;
            // Debug.Log("addign the found players");
            allPlayers.Add(foundPlayer[i]);

        }

        var pathcreator = FindObjectOfType<PathCreator>();
        if (pathcreator != null)
        {
            pathCreator = pathcreator;
            //     pathCreator.gameObject.SetActive(false);
            pathCreator.enabled = false;

        }

        PlayersCount = allPlayers.Count;
        LoadinPanel.gameObject.SetActive(false);
    }



    //adding new player 
    public void RenewTheAllPlayerListOnNewPlayerAdded(string playerType)
    {
        StartCoroutine(AddNewPlayer(playerType));
    }


    // coroutine for adding a new player into the game
    IEnumerator AddNewPlayer(string playerType)
    {
        //  pathCreator.gameObject.SetActive(true);
        //    pathCreator.enabled = true;
        //  DrawingMode();

        int playersCount = allPlayers.Count;
        yield return new WaitForSeconds(.3f);
        Debug.Log("there are " + playersCount + " players in the field");
        if (playerType == "P")
        {
            GameObject newPlayer = Instantiate(PlayerPrefab, new Vector3(newPlayerPosition.transform.position.x, newPlayerPosition.transform.position.y, newPlayerPosition.transform.position.z), Quaternion.identity);
            newPlayer.GetComponent<SinglePlayer>().singlePlayer.PlayerType = "P";
            StartCoroutine(PopulateDataForNewPlayer(newPlayer, playersCount));
        }
        else if (playerType == "D")
        {
            GameObject newPlayer = Instantiate(DefensePrefab, new Vector3(newDefensePlayerPosition.transform.position.x, newDefensePlayerPosition.transform.position.y, newDefensePlayerPosition.transform.position.z), Quaternion.identity);
            newPlayer.GetComponent<SinglePlayer>().singlePlayer.PlayerType = "D";
            var rotation = transform.rotation.eulerAngles;
            rotation.y = -180f;
            //  rotation.x = 0f;
            newPlayer.transform.rotation = Quaternion.Euler(rotation);
            StartCoroutine(PopulateDataForNewPlayer(newPlayer, playersCount));
        }
    }


    //coroutine for populating data for each new spawned player
    IEnumerator PopulateDataForNewPlayer(GameObject newPlayer, int playersCount)
    {
        yield return new WaitForSeconds(.3f);
        var p = newPlayer.GetComponent<SinglePlayer>();
        p.playerStats.PlayerID = playersCount;
        //  p.playerStats.CanMove = true;
        // p.singlePlayer.PlayerName = p.singlePlayer.PlayerType + p.singlePlayer.PlayerID;
        //  p.gameObject.name = p.singlePlayer.PlayerType + "ID" + p.singlePlayer.PlayerID;
        // p.GetComponentInChildren<Canvas>().GetComponentInChildren<Text>().text = p.singlePlayer.PlayerType + p.singlePlayer.PlayerID;
        allPlayers.Add(p);
        //   pathCreator.gameObject.SetActive(false);
        //pathCreator.enabled = false;
        yield return new WaitForSeconds(.3f);
        PlayersCount = allPlayers.Count;
    }





    [Serializable]
    public class Formation
    {
        public string FormationName;
        public int FormationID;
        public string FormationTag;
        public List<Play> LinkedPlaysWithFormation = new List<Play>();


    }
    [Serializable]
    public class Play
    {
        public string PlayName;
        public int PlayID;
        public string PlayTag;
        public List<Player> LinkedPlayersWithPlays = new List<Player>();


    }
    [Serializable]
    public class Player
    {
        public string PlayerName;
        public int PlayerID;
        public string PlayerPointerType;
        public Vector3 StartingPlayerLocalPosition;
        public Vector3 PlayerLocalPosition;
        public Vector3 PointerPosition;
        public Quaternion PointerRotation;
        public List<Vector3> Points = new List<Vector3>();
    }

    [Serializable]
    public class AllFormations
    {
        public List<Formation> AllFormmations;
    }

    [Serializable]
    public class PlaysNameHolder
    {

        public List<string> PlaysNamesHolder = new List<string>();
    }
}

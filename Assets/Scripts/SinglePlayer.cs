﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.UI;

public class SinglePlayer : MonoBehaviour
{



    int playerID;
    [HideInInspector]
    public NavMeshAgent playerNav;
    [HideInInspector]
    public PathMover pathMover;
    public Color SelectedPlayer;
    [HideInInspector]
    public Text PlayerName;
    public bool canMove;
    bool isSelected;
    //drag and drop
    private Vector3 mOffset;
    private float mZCoord;


    PlayerType playerType;
    GameObject Middle;
    bool isMiddleMoving;
    public Player singlePlayer;
    public Vector3 PlayerLocalPosition;
    // Use this for initialization
    public LineRenderer renderer;
    public Material drawingLineMaterial;
    Vector3[] positionArray = new[] { new Vector3(0f, 0f, 0f) };
    public PathCreator creator;
    public GameObject Pointer;
    string PlayName;
    public PlayerStats playerStats;
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };


    void Awake()
    {
        playerNav = GetComponent<NavMeshAgent>();
        pathMover = GetComponent<PathMover>();
        var pathcreator = (PathCreator)FindObjectOfType(typeof(PathCreator));
        if (pathcreator != null)
        {
            creator = pathcreator;
        }
    }
    void Start()
    {

        renderer = GetComponent<LineRenderer>();
        renderer.startWidth = 0.15f;
        renderer.endWidth = 0.15f;
        renderer.material = drawingLineMaterial;
        renderer.enabled = false;
        playerStats.PlayerLocalPosition = transform.position;

        playerType = PlayerType.player;
        Middle = GameObject.FindGameObjectWithTag("MiddlePlayer");
        canMove = false;
        isMiddleMoving = false;

        playerStats.StartingPlayerLocalPosition = transform.position;


    }



    /// <summary>
    /// Using this. Populate the data for each player 
    /// </summary>
    /// <param name="formationCountNumber"></param>
    /// <param name="playsCounter"></param>
    public void Populate(int formationCountNumber, int playsCounter)
    {
        if (playerStats.Points != null)
        {
            var play = new GameManager.Player();
            play.PlayerName = playerStats.PlayerName;
            play.PlayerID = playerStats.PlayerID;
            play.PlayerPointerType = playerStats.PlayerPointerType;
            play.Points = playerStats.Points;
            play.PointerPosition = playerStats.PointerPosition;
            play.PointerRotation = playerStats.PointerRotation;
            play.PlayerLocalPosition = playerStats.PlayerLocalPosition;// transform.position;
            Debug.Log("punam data za formacija  i play" + GameManager.Instance.allFormations.AllFormmations[formationCountNumber].LinkedPlaysWithFormation[playsCounter].PlayName);
            GameManager.Instance.allFormations.AllFormmations[formationCountNumber].LinkedPlaysWithFormation[playsCounter].LinkedPlayersWithPlays.Add(play);
        }

    }

    /// <summary>
    ///Using this. Load the formation data for  each player
    /// </summary>
    public void LoadFormationData()
    {

        transform.position = playerStats.PlayerLocalPosition;
    }

    /// <summary>
    /// Using this. Load the data for each player
    /// </summary>
    public void LoadPlayData()
    {
        if (playerStats.Points != null)
        {
            //Debug.Log("WTF");
            OnNewPathCreated(playerStats.Points);
            renderer.enabled = true;
            renderer.positionCount = playerStats.Points.Count;
            renderer.SetPositions(playerStats.Points.ToArray());
            // transform.position = playerStats.PlayerLocalPosition;
            if (playerStats.PlayerPointerType == "arrow")
            {

                // instantiate pointer and populate data
                var newArrow = Instantiate(GameManager.Instance.ArrowPref, playerStats.PointerPosition, playerStats.PointerRotation);
                newArrow.AddComponent<Pointer>();
                newArrow.GetComponent<Pointer>().PointerLocalPosition = playerStats.PointerPosition;
                newArrow.GetComponent<Pointer>().PointerLocalRotation = playerStats.PointerRotation;
            }
            else if (playerStats.PlayerPointerType == "block")
            {
                var newBlock = Instantiate(GameManager.Instance.BlockPref, playerStats.PointerPosition, playerStats.PointerRotation);
                newBlock.AddComponent<Pointer>();
                newBlock.GetComponent<Pointer>().PointerLocalPosition = playerStats.PointerPosition;
                newBlock.GetComponent<Pointer>().PointerLocalRotation = playerStats.PointerRotation;
            }


        }
    }
    //save it as position
    //search by name
    //public void GetPlay(string playName)
    //{
    //    StartCoroutine(WaitForThePlayersToBeFound(playName));
    //    StartCoroutine(GetSetPlayPositions(playName));
    //}

    //using this
    //IEnumerator GetSetPlayPositions(string playName)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    yield return new WaitForSeconds(.1f);
    //    if (ES2.Exists("LinePoint" + singlePlayer.PlayerID + " playname " + playName))
    //    {
    //        singlePlayer.points = ES2.LoadList<Vector3>("LinePoint" + singlePlayer.PlayerID + " playname " + playName);
    //        renderer.positionCount = singlePlayer.points.Count;
    //        renderer.SetPositions(singlePlayer.points.ToArray());
    //    }
    //    GameManager.Instance.LoadinPanel.gameObject.SetActive(false);
    //}

    //IEnumerator GetAndSetPlayPosition(string playname)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    yield return new WaitForSeconds(.3f);
    //    if (ES2.Exists("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playname))
    //    {
    //        singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playname);


    //        yield return new WaitForSeconds(.4f);
    //        renderer.positionCount = 2;
    //        if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //        {

    //            if (singlePlayer.playerPosition != Vector3.zero)
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //            }
    //            else
    //            {
    //                renderer.SetPosition(0, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));

    //            }
    //        }

    //        GameManager.Instance.LoadinPanel.gameObject.SetActive(false);
    //    }

    //}



    //USING THIS
    //public void GetFormation(string formationName)
    //{
    //    RemoveAllDrawingsWhenLoadingNew();
    //    renderer.enabled = true;
    //    //  RemoveAllDrawingsWhenLoadingNew();
    //    renderer.positionCount = 2;

    //    singlePlayer.playerPosition = ES2.Load<Vector3>("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName);
    //    if (singlePlayer.playerPosition != Vector3.zero)
    //        transform.position = singlePlayer.playerPosition;
    //    else
    //    {
    //        transform.position = PlayerLocalPosition;
    //    }
    //    Debug.Log(transform.position);
    //    // StartCoroutine(GetFormationAfterFromDB(formationName));
    //}

    //IEnumerator GetFormationAfterFromDB(string formationname)
    //{
    //    renderer.enabled = true;
    //    RemoveAllDrawingsWhenLoadingNew();
    //    renderer.positionCount = 2;
    //    yield return new WaitForSeconds(0.3f);
    //    singlePlayer.playerPosition = ES2.Load<Vector3>("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationname);
    //    if (singlePlayer.playerPosition != Vector3.zero)
    //        transform.position = singlePlayer.playerPosition;
    //    else
    //    {
    //        transform.position = PlayerLocalPosition;
    //    }
    //    Debug.Log(transform.position);
    //}

    //void RemoveAllDrawingsWhenLoadingNew()
    //{
    //    renderer.enabled = true;
    //    creator.PointerHolder.SetActive(false);
    //    var allDrawings = FindObjectsOfType<LineRenderer>();
    //    foreach (var item in allDrawings)
    //    {
    //        item.positionCount = 0;
    //    }
    //    var allPointers = FindObjectsOfType<Pointer>();
    //    if (allPointers != null)
    //    {
    //        foreach (var item in allPointers)
    //        {
    //            if (item != null)
    //            {
    //                // Destroy(item.transform.parent.gameObject);

    //            }
    //        }
    //    }


    //}




    //IEnumerator WaitForThePlayersToBeFound(string playName)
    //{
    //    renderer.enabled = true;
    //    yield return new WaitForSeconds(.3f);

    //    yield return new WaitForSeconds(.1f);

    //    if (ES2.Exists("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName))
    //    {
    //        Debug.Log("the play exists getting data...");

    //        singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName);
    //        Vector3 pointerHolder = singlePlayer.PointerPositionOnly;

    //        yield return new WaitForSeconds(.3f);
    //        if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //        {

    //            if (singlePlayer.playerPosition != Vector3.zero)// if formation is selected
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //                renderer.SetPosition(1, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //                CheckTheFirstPositionAfterDrawing();
    //            }
    //            else   // if there is drawing on default player positions
    //            {
    //                renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //                renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //                CheckTheFirstPositionAfterDrawing();
    //            }
    //            Debug.Log("data for play " + playName + " is loaded");
    //        }
    //        else
    //        {
    //            renderer.positionCount = 0;
    //        }
    //    }



    //    //if (singlePlayer.playerPosition != Vector3.zero)// if formation is selected
    //    //{
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(singlePlayer.playerPosition.x, singlePlayer.playerPosition.y, singlePlayer.playerPosition.z));
    //    //    //CheckTheFirstPositionAfterDrawing();
    //    //}
    //    //else   // if there is drawing on default player positions
    //    //{
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    //    CheckTheFirstPositionAfterDrawing();
    //    //}


    //    //if (ES2.Exists("PlayPID" + singlePlayer.PlayerID + " playname " + playName))
    //    //{
    //    //    UIManager.Instance.SelectTextType("none", "warning", 0f);
    //    //    PlayerLocalPosition = ES2.Load<Vector3>("PlayPLP" + singlePlayer.PlayerID + " playname " + playName);
    //    //    // singlePlayer.playerPosition = ES2.Load<Vector3>("playerPositionID" + singlePlayer.PlayerID + " tacticname " + tacticname);
    //    //    singlePlayer.PointerPositionOnly = ES2.Load<Vector3>("PlayPID" + singlePlayer.PlayerID + " playname " + playName);
    //    //    transform.position = singlePlayer.PointerPositionOnly;
    //    //    renderer.SetPosition(0, new Vector3(singlePlayer.PointerPositionOnly.x, singlePlayer.PointerPositionOnly.y, singlePlayer.PointerPositionOnly.z));
    //    //    renderer.SetPosition(1, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    // renderer.SetPosition(0, new Vector3(PlayerLocalPosition.x, PlayerLocalPosition.y, PlayerLocalPosition.z));
    //    //    //  renderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z));
    //    //    transform.position = PlayerLocalPosition; // removes the positioning on the players at the end of the line
    //    //    CheckTheFirstPositionAfterDrawing();
    //    //    //var newArrow = Instantiate(Pointer, singlePlayer.PointerPositionOnly, Quaternion.identity);
    //    //    //newArrow.SetActive(true);
    //    //    //newArrow.transform.position = singlePlayer.PointerPositionOnly;

    //    //}
    //    ////else
    //    ////{
    //    ////    //PlayerLocalPosition 
    //    ////    singlePlayer.playerPosition  = ES2.Load<Vector3>("FormationPLP" + singlePlayer.PlayerID + " formationname " + playName);
    //    ////    transform.position = singlePlayer.playerPosition;
    //    ////}

    //    GameManager.Instance.LoadinPanel.gameObject.SetActive(false);

    //}


    //void CheckTheFirstPositionAfterDrawing()
    //{
    //    var first = renderer.GetPosition(0);
    //    Debug.Log(first.magnitude);
    //    if (first.magnitude.Equals(0))
    //    {
    //        renderer.positionCount = 0;
    //    }
    //}

    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {

            if (Input.GetMouseButtonDown(0))
            {
                var foundPlayer = hitInfo.transform.GetComponent<SinglePlayer>();
                if (foundPlayer != null)
                {
                    Debug.Log("clicked on player");
                    CameraMovement.Instance.isPanning = false;
                    GameManager.Instance.CanDrawIfSelected = true;

                }
                else
                {
                    GameManager.Instance.CanDrawIfSelected = false;
                    CameraMovement.Instance.isPanning = true;

                }

            }

        }

        // remove the drawed line and make the player unselected so it won't be saved
        if (Input.GetButtonUp("Fire3"))
        {
            if (Physics.Raycast(ray, out hitInfo))
            {
                var selectedplayer = hitInfo.transform.gameObject.GetComponent<SinglePlayer>();
                if (selectedplayer != null)
                {
                    Debug.Log("selected player " + selectedplayer.singlePlayer.PlayerID);
                    selectedplayer.singlePlayer.isSelected = false;
                    selectedplayer.GetComponent<LineRenderer>().positionCount = 0;
                    selectedplayer.singlePlayer.PointerPositionOnly = new Vector3(0, 0, 0);
                    var pointers = FindObjectsOfType<Pointer>();
                    if (pointers != null)
                    {
                        foreach (var item in pointers)
                        {
                            Destroy(item.gameObject);
                        }
                    }
                }

            }

        }

        if (transform.position.x > 23)
            transform.position = new Vector3(23, transform.position.y, transform.position.z);
        else if (transform.position.x < -23)
            transform.position = new Vector3(-23, transform.position.y, transform.position.z);
    }

    public void ActivatelayerPath()
    {
        pathMover = GetComponent<PathMover>();
        if (pathMover != null)
            pathMover.enabled = false;
    }




    void OnMouseOver()
    {
        foreach (var item in GameManager.Instance.allPlayers)
        {
            item.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
        transform.GetComponentInChildren<MeshRenderer>().enabled = true;


        if (playerStats.CanMove)
        {

            //  pathMover.SetPoints(playerStats.Points);
            GameManager.Instance.SelectedPlayerID = playerStats.PlayerID;
        }




        //if (canMove)
        //{
        //    GameManager.Instance.pathCreator.gameObject.SetActive(true);
        //    GameManager.Instance.pathCreator.enabled = true;
        //    playerNav.speed = 12;
        //    playerNav.angularSpeed = 30;


        //    for (int i = 0; i < GameManager.Instance.allPlayers.Count; i++)
        //    {
        //      //  GameManager.Instance.allPlayers[i].GetComponent<MeshRenderer>().material.color = Color.white;
        //        GameManager.Instance.allPlayers[i].playerNav.enabled = false;
        //        GameManager.Instance.allPlayers[i].pathMover.enabled = false;
        //        GameManager.Instance.allPlayers[i].GetComponentInChildren<MeshRenderer>().enabled = false;
        //    }
        //    //gameObject.GetComponent<MeshRenderer>().material.color = SelectedPlayer;
        //    transform.GetComponentInChildren<MeshRenderer>().enabled = true;
        //    GameManager.Instance.SelectedPlayerID = singlePlayer.PlayerID;

        //    playerNav.enabled = true;
        //    pathMover.enabled = false; // false if we don't want the players to follow the line

        //    //var player = gameObject.GetComponent<SinglePlayer>();
        //    //if (GameManager.Instance.ListOfPlayersForSaving.Contains(player))
        //    //    Debug.Log("player  " + player.playerID + "is already added to the list for saving");
        //    //else
        //    //    GameManager.Instance.ListOfPlayersForSaving.Add(player);
        //}

    }

    //public void PlaySave(string playname)
    //{
    //    StartCoroutine(SavingPlayPosition(playname));
    //}

    //IEnumerator SavingPlayPosition(string playName)
    //{
    //    PlayName = playName;
    //    UIManager.Instance.SelectTextType("Play has been saved!", "success", 2f);
    //    //  PlayerName.text = "Saved!";
    //    yield return new WaitForSeconds(.2f);
    //    SaveAllPoints(playName);
    //    if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //    {
    //        //   ES2.Save(singlePlayer.PointerPositionOnly, "PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName);
    //    }

    //    if (singlePlayer.PointerPositionOnly != Vector3.zero)
    //    {
    //        //if (ES2.Exists("PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName))
    //        //{


    //        //    Debug.Log("play with this name already exists, do you want to override it ?");
    //        //}
    //        //else
    //        //{
    //        //    ES2.Save(singlePlayer.PointerPositionOnly, "PlayrPointerPosition" + singlePlayer.PlayerID + " playname " + playName);
    //        //}


    //    }
    //    //singlePlayer.playerPosition = singlePlayer.PointerPositionOnly;

    //    //ES2.Save(singlePlayer.playerPosition, "PlayPID" + singlePlayer.PlayerID + " playname " + playName);
    //    //ES2.Save(PlayerLocalPosition, "PlayPLP" + singlePlayer.PlayerID + " playname " + playName);
    //    //GetComponent<MeshRenderer>().material.color = Color.white;
    //}



    //public void FormationSave(string formationname)
    //{
    //    StartCoroutine(SavingFormation(formationname));

    //}



    //IEnumerator SavingFormation(string formationName)
    //{
    //    UIManager.Instance.SelectTextType("Formation has been saved!", "success", 2f);
    //    yield return new WaitForSeconds(.3f);

    //    ES2.Save(singlePlayer.playerPosition, "FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName);

    //    Debug.Log("saving position " + singlePlayer.playerPosition);
    //    if (ES2.Exists("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName))
    //    {
    //        //deleteing the old one and creating new under the same name
    //        ES2.Delete("FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName);
    //        ES2.Save(singlePlayer.playerPosition, "FormationPlayerLocalPosition" + singlePlayer.PlayerID + " formationname " + formationName);




    //    }
    //    GetComponent<MeshRenderer>().material.color = Color.white;
    //}

    //void SaveAllPoints(string playName)
    //{
    //    //for (int i = 0; i < singlePlayer.points.Count; i++)
    //    //{
    //    //    ES2.Save(singlePlayer.points[i], "LinePoint" + singlePlayer.PlayerID + " playname " + playName);
    //    //}
    //    ES2.Save(singlePlayer.points, "LinePoint" + singlePlayer.PlayerID + " playname " + playName);

    //}
    ////IEnumerator SavingPosition()
    ////{
    ////    PlayerName.text = "Saved!";
    ////    yield return new WaitForSeconds(1f);
    ////    singlePlayer.playerPosition = transform.position;
    ////    ES2.Save(singlePlayer.playerPosition, "playerPosition" + singlePlayer.PlayerID);
    ////    PlayerName.text = singlePlayer.PlayerName;
    ////    GetComponent<MeshRenderer>().material.color = Color.white;
    ////}

    void OnMouseDown()
    {

        mZCoord = Camera.main.WorldToScreenPoint(
        gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()

    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition;



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }
    void OnMouseDrag()
    {

        if (!playerStats.CanMove)
        {

            transform.position = GetMouseAsWorldPoint() + mOffset;
            playerStats.PlayerLocalPosition = transform.position;
        }

    }







    [Serializable]
    public class Player
    {
        public int PlayerID;
        public string PlayerName;
        public string PlayerType;
        public Vector3 playerPosition;
        public string TacticName;
        public Vector3 PointerPositionOnly;
        public bool isSelected;
        public List<Vector3> points = new List<Vector3>();
    }

    [Serializable]
    public class PlayerStats
    {
        public string PlayerName;
        public int PlayerID;
        public bool CanMove;
        public bool HasDrawnedLine;
        public string PlayerPointerType;
        public Vector3 StartingPlayerLocalPosition;
        public Vector3 PlayerLocalPosition;
        public Vector3 PointerPosition;
        public Quaternion PointerRotation;
        public List<Vector3> Points;
    }

}


enum PlayerType
{
    player,
    middlePlayer
}

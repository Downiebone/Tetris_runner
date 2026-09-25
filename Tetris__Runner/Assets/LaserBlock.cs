using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlock : Block_To_Inherit
{
    //0 = up,  1 = right,  2 = down,  3 = left
    private int myRotation = 0;
    [SerializeField] private Sprite[] RotationSprites;
    private Vector2Int myGridPosition;
    private Vector2Int laserEjectionDirection;

    private GridEditor gridScript;

    private List<Vector2Int> currentActiveLasersPosition = new List<Vector2Int>();
    [SerializeField] private GameObject[] LaserPieces;
    [SerializeField] private GameObject LaserSparcle;

    public override void PlaceThisBlock(Vector2Int gridPos, int spawnRotation)
    {
        Debug.Log("PlaceThisLaser " + spawnRotation.ToString()); 

        myRotation = spawnRotation;
        myGridPosition = gridPos;

        switch (spawnRotation)
        {
            case 0:
                laserEjectionDirection = new Vector2Int(0, 1); //up
                LaserSparcle.transform.Rotate(0, 0, 180, Space.World);
                break;
            case 1:
                laserEjectionDirection = new Vector2Int(1, 0); //right
                for (int i = 0; i < LaserPieces.Length; i++)
                {
                    LaserPieces[i].transform.Rotate(0, 0, 90, Space.World);
                }
                LaserSparcle.transform.Rotate(0, 0, 90, Space.World);
                break;
            case 2:
                laserEjectionDirection = new Vector2Int(0, -1); //down
                for (int i = 0; i < LaserPieces.Length; i++)
                {
                    LaserPieces[i].transform.Rotate(0, 0, 180, Space.World);
                }
                break;
            case 3:
                laserEjectionDirection = new Vector2Int(-1, 0); //left
                for (int i = 0; i < LaserPieces.Length; i++)
                {
                    LaserPieces[i].transform.Rotate(0, 0, 270, Space.World);
                }
                LaserSparcle.transform.Rotate(0, 0, 270, Space.World);
                break;
            default:
                laserEjectionDirection = new Vector2Int(0, 1); //up
                break;
        }

        GetComponent<SpriteRenderer>().sprite = RotationSprites[myRotation];
    }

    public override void DestroyThisBlock()
    {
        Debug.Log("Destroy this laser #########################################");

        gridScript.GridUpdatedEvent.RemoveListener(OnPlaceEventTriggered);

        Destroy(this.gameObject);
    }

    [SerializeField] private float DistanceToCountAsOffScreen = 5;

    private Transform playerTrans;

    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        gridScript = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridEditor>();

        gridScript.GridUpdatedEvent.AddListener(OnPlaceEventTriggered);

        //recalculate once on place
        OnPlaceEventTriggered();
    }

    void OnPlaceEventTriggered()
    {
        Debug.Log("Callback executed");

        for (int i = 0; i < LaserPieces.Length; i++)
        {
            LaserPieces[i].SetActive(false);
        }

        for (int i = 0; i < currentActiveLasersPosition.Count; i++)
        {
            //remove the invisible danger from last laser positions
            if (gridScript.getCellAtPoint(currentActiveLasersPosition[i]).type == Cell.Cell_type.InvisibleDanger)
            {
                gridScript.deleteTile_exeptFloor(currentActiveLasersPosition[i]);
            }
        }

        currentActiveLasersPosition.Clear();

        Vector2Int currentExpandingPos = myGridPosition;
        //recalculate laser
        for (int i = 0; i < LaserPieces.Length; i++)
        {
            currentExpandingPos += laserEjectionDirection;
            Cell expandingLaser_TestPos_Cell = gridScript.getCellAtPoint(currentExpandingPos);
            //only expand laser if its a space within grid and inactive
            if (currentExpandingPos.y < gridScript.gridHeight && currentExpandingPos.y > 0 && !gridScript.Cell_is_Active_and_blocking_building(currentExpandingPos))
            {
                currentActiveLasersPosition.Add(currentExpandingPos);

                //danger blocks are invisible
                gridScript.generateTile_NoEvent(currentExpandingPos, Color.white, Cell.Cell_type.InvisibleDanger, 0, 0);
                LaserPieces[i].SetActive(true);
                LaserPieces[i].transform.position = (Vector2)currentExpandingPos;
            }
            else
            {
                //failed (we reached a end)
                //put the sparkle on the last block, because right now we are inside a wall or something
                LaserSparcle.transform.position = (Vector2)(currentExpandingPos - laserEjectionDirection);

                return;
            }
        }

        //if we never found an end. Just put the sparkle at the end of the laser
        LaserSparcle.transform.position = (Vector2)(currentExpandingPos);

    }

    private void FixedUpdate()
    {
        if (transform.position.x < playerTrans.position.x - DistanceToCountAsOffScreen)
        {
            DestroyThisBlock();
        }
    }
}

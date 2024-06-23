using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditor_Camera : MonoBehaviour
{
    [SerializeField] private float camSpeed;
    [SerializeField] private float camSpeed_multiplier;

    [SerializeField] private GameObject cell_highlight;

    [SerializeField] private GridEditor Grid_script;

    public GridSaver Save_Script;

    // Update is called once per frame
    void Update()
    {
        float cam_multiplier = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            cam_multiplier = camSpeed_multiplier;
        }

        transform.Translate(Vector2.up * Input.GetAxis("Vertical") * camSpeed * cam_multiplier * Time.deltaTime);
        transform.Translate(Vector2.right * Input.GetAxis("Horizontal") * camSpeed * cam_multiplier * Time.deltaTime);
    }

    private void LateUpdate()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int currentGridMousePos = new Vector2Int(Grid_script.getClosestGridPos(mouseWorldPos).x, Grid_script.getClosestGridPos(mouseWorldPos).y);
        cell_highlight.transform.position = Grid_script.getClosestGridPos(mouseWorldPos);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Grid_script.placeTile(currentGridMousePos, Color.white);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Grid_script.deleteTile(currentGridMousePos);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Save_Script.Setup_AreaSize(Grid_script, Grid_script.gridHeight, Grid_script.gridLength);
            Save_Script.SaveCellsToJson("test1");
        }

    }
}

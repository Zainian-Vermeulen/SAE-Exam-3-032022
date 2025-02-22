﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Respresents a game level with a playground of cellCount_X * cellCount_Y elements.
/// </summary>
/// 




public class Level
{

    [SerializeField] private ElementGrid elementGrid;
    /// <summary>
    /// Possible States of the currentLevel. The GameManager decides when to end the level (Win/Loose) upon the current state of the level.
    /// </summary>
    public enum LevelState
    {
        /// <summary>
        /// The Level is "empty". All <see cref="Element"/>s of the ElementGrid were removed by the player through Click-Events.
        /// </summary>
        NoElementsLeft,
        /// <summary>
        /// There are still <see cref="Element"/>s in the level but the player can't perform moves. Means that there are no <see cref="Element"/>s with Neighbours of the same type (<see cref="Element.ElementType"/>) left.
        /// </summary>
        NoMoreMovesPossible,
        /// <summary>
        /// This is the DEFAULT state. This State remains as long as the player has the possibility to remove further <see cref="Element"/>s from the level.
        /// </summary>
        FurtherMovesPossible
    };
    
    
    
    //---Member---
    /// <summary>
    /// Returns the Playerscore (points the player has made).
    /// </summary>
    int points;
    /// <summary>
    /// Returns or set the Playerscore (points the player has made).
    /// </summary>
    public int Points
    {
        get { return points; }
        set { points = value; }
    }

    /// <summary>
    /// The playground.
    /// </summary>
    ElementGrid grid;

    //------STUDENTS IMPLEMENT FUNCTIONALITY BELOW---------------------------------------------------------------------
    //---Constructor---
    /// <summary>
    /// Creates a new Level of the SameGame. Initializes the ElementGrid, fills the ElementgGrid with new random Elements (determined by the prefabs and the randomGeneratorSeed) and creates instances of the element visuals of the prefabs in the scene.
    /// </summary>
    /// <param name="origin">The Origin of the level grid.</param>
    /// <param name="cellSize">Size of each cell of the level grid. </param>
    /// <param name="cellCount_X">Number of cells in x-direction.</param>
    /// <param name="cellCount_Y">Number of cells in y-direction.</param>
    /// <param name="seedForrandomNumberGenerator">Seed for the random number generator to reproduce random levels.</param>
    /// <param name="prefabs">The Prefabs for the Element visuals in the Unity scene. The number of different element types corresponds to the number of prefabs.</param>
    /// <param name="parent">The Parent for all Element visuals in the scene.</param>
    public Level(Vector3 origin, Vector2 cellSize, int cellCount_X, int cellCount_Y, int seedForRandomNumberGenerator, GameObject[] prefabs, Transform parent)
    {
        // Don't delete the following lines
        grid = new ElementGrid(origin, cellSize, cellCount_X, cellCount_Y);
        points = 0;

        // ***** Students Start here ******

      

        for (int x = 0; x < cellCount_X; x++)
        {
            for (int y = 0; y < cellCount_Y; y++)
            {

                var gameObject = new Element(_ElementType, seedForRandomNumberGenerator);
                
                //var GO = GameObject.Instantiate(gameObject.ElementType, origin, Quaternion.identity);
                elementGrid.SetElement(y, gameObject);
                // grid.CellCount[i, j] = grid.

                //currentLevel = new Level(transform.position, cellSize, numberOfCellsPerRow, numberOfCellsPerColumn, randomNumberSeed, prefabs, transform);

            }
        }
        //var element = ElementType;

    }

    private GameObject _elementType;
    public GameObject _ElementType
    {
        get { return _elementType; }
        set { _elementType = value; }
    }


    //---Methods---
    /// <summary>
    /// Returns the indices of adjacent cells with the same type (the type of the Element of the cell at cellIndex) as an array of integer indices.
    /// </summary>
    /// <returns>An array of integer indices of adjacent cells with the same type (including the selected cell) (<see cref="Element.ElementType"/>) or null (if the selected cell is empty).</returns>
    /// <param name="cellIndex">Index of the original cell.</param>
    public int[] GetAdjacentCellsOfSameType(int cellIndex)
    {

        // comment the out the following line
        //return null;        
            var gridElement = elementGrid.GetNeighbours(cellIndex);

            if (gridElement.Length > 0)
            {
                return gridElement;
            }
            else
                return null;
    }
    /// <summary>
    /// This Function implements the funcionality for MouseHover events. It is called each frame if no MouseClick is detected.
    /// <BR><B>Attention:</B> The given worldPosition is not necessarily inside of the grid bounds.
    /// </summary>
    /// <returns> The Number of the hovered cells. </returns>
    /// <param name="worldPosition">Point in worldcoordinates (this position is not necessarily inside of the grid bounds).</param>
    public int HoverCells(Vector3 worldPosition)
    {
        worldPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(worldPosition);
        RaycastHit hit;

        
        List<GameObject> objectsToSpin = new List<GameObject>();

        if (Physics.Raycast(ray, out hit) && (!Input.GetKeyDown(KeyCode.Mouse0)))
        {
            GameObject obj = hit.transform.GetComponent<GameObject>();
            var objIndex = elementGrid.PointToIndex(worldPosition);
            //elements next too
            // element 
       
            var objNeighbours = GetAdjacentCellsOfSameType(objIndex);
            if (objNeighbours.Length > 0)
            {
                objectsToSpin.Add(obj);
            }    

            foreach (GameObject item in objectsToSpin)
            {
                item.transform.Rotate(0,45,0);
            }
            Debug.Log("Object hit: " + hit.collider.name);
        }

        return objectsToSpin.Count;
        // comment the out the following line
        //return -99;
    }



    /// <summary>
    /// This Function implements the funcionality for MouseClick events. It is called when a mouseclick was detected to make a move (remove adjacent Elements if there are more then two adjacent elements of the same type beginning at the worldPosition).
    /// <BR><B>Attention:</B> The given worldPosition is not necessarily inside of the grid bounds.
    /// </summary>
    /// <returns> The Number of the selected/removed cells. </returns>
    /// <param name="worldPosition">Point in worldcoordinates (this position is not necessarily inside of the grid bounds).</param>
    public int SelectCells(Vector3 worldPosition)
    {
        // comment the out the following line
        //return -99;

        worldPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(worldPosition);
        RaycastHit hit;


        List<GameObject> objectsToSelect = new List<GameObject>();
            var point = elementGrid.PointToIndex(worldPosition);
            var adjacentCells = GetAdjacentCellsOfSameType(point);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.transform.GetComponent<GameObject>();
            if (adjacentCells.Length > 2)
            {
                elementGrid.RemoveElements(adjacentCells);
                objectsToSelect.Add(obj);
                CalculatePoints(adjacentCells.Length);
            }

        }

        return adjacentCells.Length;

    }
    /// <summary>
    /// Checks the state of the current Level. 
    /// <returns><c>NoElementsLeft</c>: if there are no more (not empty) Elements left; <BR><c>NoMoreMovesPossible</c>: if there are no more moves possible (not empty Elements left, but only single Elements without neighbours of the same type); <BR><c>FurtherMovesPossible</c>: otherwise (see <see cref="Level.LevelState"/>).</returns>
    /// </summary>
    public LevelState CheckLevelState()
    {

        // comment the out the following line
        //return LevelState.NoElementsLeft;
        for (int i = 0; i < grid.CellCount; i++)
        {
            if (!elementGrid.IsIndexValid(i))
            {
               return LevelState.NoElementsLeft;
            }
            else if (GetAdjacentCellsOfSameType(i).Length <= 1)
            {
                return LevelState.NoMoreMovesPossible; ;
            }
        }

        return LevelState.FurtherMovesPossible;
    }

    /// <summary>
    /// Calculates the points for each move (by the number of involved Elements of the same type) or if the player looses (by the number of the remaining Elements in the level).
    /// </summary>
    /// <returns>The points to add or substract to the players score.</returns>
    /// <param name="numElements">Number of elements.</param>
    public int CalculatePoints(int numElements)
    {
        //Points = (Amount of Stones -2)² (a)
        var points = (numElements - 2) ^ 2;
        return points;
        // comment the out the following line
        //return -99
    }

    /// <summary>
    /// Destroys the level. Cleanup the Grid.
    /// This Function is calle by the GameManager.
    /// </summary>
    public void DestroyLevel()
    {
        // Don't delete the following lines!
        if (grid != null)
        {
            grid.DestroyGrid();
            grid = null;
        }
    }


    public void ChangeCursor()
    {
        Vector3 mousePoint = Input.mousePosition;
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            GUI.skin.settings.cursorColor = Color.cyan;
        }
        else if (HoverCells(mousePoint) <= 0)
        {
            GUI.skin.settings.cursorColor = Color.blue;
        }
        else if (EventSystem.current.IsPointerOverGameObject() && SelectCells(Input.mousePosition) < 1 )
        {
            GUI.skin.settings.cursorColor = Color.red;
        }
        else if (SelectCells(Input.mousePosition) > 1)
        {
            GUI.skin.settings.cursorColor = Color.white;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GUI.skin.settings.cursorColor = Color.black;

        }


        //if (!Physics.Raycast(ray, out currentIsPointerOverGameObject()))
        //{ 
        //}
        /*
         * Cursor outside the grid
        ● Cursor is in grid, but currently not hovering an element
●                   Cursor is in grid and hovering over an element, but the element has no other
            adjacent elements of the same type (no possible move)
●                   Cursor is on valid (according to rules clickable) element (normal cursor)
●                   Cursor is being clicked
         */
       
    }
}
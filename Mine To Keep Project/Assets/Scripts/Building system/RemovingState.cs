using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    int ID;
    ObjectsDatabaseSO database;
    Grid grid;
    PreviewSystem previewSystem;
    GridData structureData;
    ObjectPlacer objectPlacer;

    public RemovingState(Grid grid, PreviewSystem previewSystem, ObjectsDatabaseSO database, GridData structureData,ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.structureData = structureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
        
    }

    public int GetID()
    {
        return ID;
    }

    public void EndState()
    {
        previewSystem.StopShowingPlacementPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if(structureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = structureData;
        }

        if(selectedData == null)
        {
            //sound
            return;
        }
        else
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if(gameObjectIndex == -1)
            {
                return;
            }
            for (int i = 0; i < database.GetObjectByID(selectedData.GetObjectIDAt(gridPosition)).resourceCost.Length; i++)
            {
                GameResource resource = new GameResource(database.GetObjectByID(selectedData.GetObjectIDAt(gridPosition)).resourceCost[i],
                                                        database.GetObjectByID(selectedData.GetObjectIDAt(gridPosition)).resourceCostAmount[i]);
                ResourceManager.instance.AddResource(resource);
            }

            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return !(structureData.CanPlaceObjectAt(gridPosition, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = CheckIfSelectionIsValid(gridPosition);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), isValid);
    }
}
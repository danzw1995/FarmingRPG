using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & TileMaps References")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenalties = true;

    [Range(0, 20)]
    [SerializeField] private int pathMovementPenalty = 0;
    [Range(0, 20)]
    [SerializeField] private int defaultMovementPenalty = 0;

    private GridNodes gridNodes;
    private Node startNode;
    private Node endNode;

    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathfound = false;

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStack)
    {
        pathfound = false;
        if (PopulateGridNodesFormGridPropertiesDictionary(sceneName, startGridPosition, endGridPosition))
        {
            if (FindShortestPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStack);
                return true;
            }
        }

        return false;
    }

    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStack)
    {
        Node nextNode = endNode;

        while (nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    private bool FindShortestPath ()
    {
        openNodeList.Add(startNode);

        while(openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            closedNodeList.Add(currentNode);

            if (currentNode == endNode)
            {
                pathfound = true;
                break;
            }

            EvaluateCurrentNodeNeighbours(currentNode);
        }

        return pathfound;
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int curerntNodeGridPositoin = currentNode.gridPosition;
        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                validNeighbourNode = GetValidNodeNeighBour(curerntNodeGridPositoin.x + i, curerntNodeGridPositoin.y + j);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    if (observeMovementPenalties)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, endNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }


    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        } else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    private Node GetValidNodeNeighBour(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }

        if (x > gridWidth || y > gridHeight)
        {
            return null;
        }

        Node neighBourNode = gridNodes.GetGridNode(x, y);
        if (neighBourNode == null)
        {
            return null;
        }

        if (neighBourNode.isObstacle || closedNodeList.Contains(neighBourNode))
        {
            return null;
        }
        return neighBourNode;
    }

    private bool PopulateGridNodesFormGridPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        SceneSave sceneSave;

        if (GridPropertiesManager.Instance.gameObjectSave.sceneData.TryGetValue(sceneName.ToString(), out sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDictionary != null)
            {
                if (GridPropertiesManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                {
                    gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                    gridWidth = gridDimensions.x;
                    gridHeight = gridDimensions.y;

                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    openNodeList = new List<Node>();
                    closedNodeList = new HashSet<Node>();
                } else
                {
                    return false;
                }

                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);
                endNode = gridNodes.GetGridNode(endGridPosition.x - gridOrigin.x, endGridPosition.y - gridOrigin.y);

                for (int x = 0; x < gridDimensions.x; x ++)
                {
                    for (int y = 0; y < gridDimensions.y; y ++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);
                        if (gridPropertyDetails != null)
                        {
                            Node node = gridNodes.GetGridNode(x, y);

                            if (gridPropertyDetails.isNPCObstacle == true)
                            {
                                node.isObstacle = true;
                            } else if (gridPropertyDetails.isPath)
                            {
                                node.movementPenalty = pathMovementPenalty;
                            } else
                            {
                                node.movementPenalty = defaultMovementPenalty;
                            }
                        } 
                    }
                }

            } else
            {
                return false;
            }
        } else
        {
            return false;
        }

        return true;
    }

}

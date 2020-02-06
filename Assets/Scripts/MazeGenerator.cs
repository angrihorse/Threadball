using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeGenerator : MonoBehaviour
{
    public Vector2 mazeSize;
    public Cell[,] cells;

    public Transform floorPrefab;
    public Transform destinationCellPrefab;
    public Transform wallPrefab;
    public Transform player;

    public float cellSize;
    public float cellHeight;
    public float wallThickness;
    public float wallHeight;

    public bool instantiatePlayer;

    // Start is called before the first frame update
    void Start() {
        InitializeCells();
        GenerateMazeWithRecursiveBacktracker();
        if (instantiatePlayer) {
            PutPlayerOnStartingPosition();
        }
        SetDestination();
        ScalePrefabs();
        InstantiateMaze();
    }

    void InitializeCells() {
        cells = new Cell[(int)mazeSize.x, (int)mazeSize.y];
        for (int i = 0; i < mazeSize.x; i++) {
            for (int j = 0; j < mazeSize.y; j++) {
                cells[i, j] = new Cell(i, j);
            }
        }
    }

    void GenerateMazeWithRecursiveBacktracker() {
        Cell current = cells[0, 0];
        current.visited = true;
        Stack stack = new Stack();
        void RecursiveBacktracker() {
            if (HasUnvisitedNeighbors(current, out bool[] unvisitedNeighbors)) {
                Cell next = GetRandomNeighbor(current, unvisitedNeighbors);
                next.visited = true;
                stack.Push(current);
                RemoveWallBetween(current, next);
                current = next;
                RecursiveBacktracker();
            } else if (stack.Count > 0) {
                  current = stack.Pop() as Cell;
                  RecursiveBacktracker();
            }
        }

        RecursiveBacktracker();

        bool HasUnvisitedNeighbors(Cell cell, out bool[] neighbors) {
            neighbors = UnvisitedNeighbors(cell);
            foreach (bool val in neighbors) {
                if (val) {
                    return true;
                }
            }
            return false;
        }

        bool[] UnvisitedNeighbors(Cell cell) {
            bool[] withinMazeSize = new bool[4];
            withinMazeSize[0] = cell.neighborsPositions[0].x >= 0;
            withinMazeSize[1] = cell.neighborsPositions[1].y < mazeSize.y;
            withinMazeSize[2] = cell.neighborsPositions[2].x < mazeSize.x;
            withinMazeSize[3] = cell.neighborsPositions[3].y >= 0;

            bool[] check = new bool[4];
            for (int i = 0; i < 4; i++) {
                check[i] = withinMazeSize[i] && !cells[(int)cell.neighborsPositions[i].x, (int)cell.neighborsPositions[i].y].visited;
            }

            return check;
        }

        Cell GetRandomNeighbor(Cell cell, bool[] check) {
            var potentialNextCells = new List<Cell>{};
            for (int i = 0; i < 4; i++) {
                if (check[i]) {
                    potentialNextCells.Add(cells[(int)cell.neighborsPositions[i].x, (int)cell.neighborsPositions[i].y]);
                }
            }
            return potentialNextCells[UnityEngine.Random.Range(0, potentialNextCells.Count)];
        }

        void RemoveWallBetween(Cell cell1, Cell cell2) {
            Vector2 displacement = cell2.position - cell1.position;

            // Top.
            if (displacement == new Vector2(-1, 0)) {
                cell1.walls[0] = false;
                cell2.walls[2] = false;
            }

            // Right.
            if (displacement == new Vector2(0, 1)) {
                cell1.walls[1] = false;
                cell2.walls[3] = false;
            }

            // Bottom.
            if (displacement == new Vector2(1, 0)) {
                cell1.walls[2] = false;
                cell2.walls[0] = false;
            }

            // Left.
            if (displacement == new Vector2(0, -1)) {
                cell1.walls[3] = false;
                cell2.walls[1] = false;
            }
        }
    }

    public void PutPlayerOnStartingPosition() {
        List<Vector2> potentialStartingPoints = new List<Vector2>{};
        for (int x = 0; x < mazeSize.x; x++) {
            for (int y = 0; y < mazeSize.y; y++) {
                bool[] walls = cells[x, y].walls;
                int wallCount = 0;
                foreach (bool hasWall in walls) {
                    if (hasWall) {
                        wallCount++;
                    }
                }
                if (wallCount == 1) {
                    potentialStartingPoints.Add(new Vector2(x, y));
                }
            }
        }
        if (potentialStartingPoints.Count == 0) {
            potentialStartingPoints.Add(Vector2.zero);
        }

        List<float> distancesToPotentialStartingPoints = new List<float>{};
        foreach(Vector2 pos in potentialStartingPoints) {
            distancesToPotentialStartingPoints.Add(Vector2.Distance(pos, Vector2.zero));
        }

        float minDistance = Mathf.Min(distancesToPotentialStartingPoints.ToArray());
        int minDistanceIndex = distancesToPotentialStartingPoints.IndexOf(minDistance);
        Vector2 startingPointPosition = potentialStartingPoints[minDistanceIndex];

        Transform newPlayer = Instantiate(player, new Vector3(startingPointPosition.x * (int)floorPrefab.localScale.x, player.localScale.y/2, startingPointPosition.y * (int)floorPrefab.localScale.z), Quaternion.identity);
    }

    void SetDestination() {
        List<Vector2> potentialDestinations = new List<Vector2>{};
        for (int x = 0; x < mazeSize.x; x++) {
            for (int y = 0; y < mazeSize.y; y++) {
                bool[] walls = cells[x, y].walls;
                int wallCount = 0;
                foreach (bool hasWall in walls) {
                    if (hasWall) {
                        wallCount++;
                    }
                }
                if (wallCount == 3) {
                    potentialDestinations.Add(new Vector2(x, y));
                }
            }
        }

        List<float> distancesToPotentialDestinations = new List<float>{};
        foreach(Vector2 pos in potentialDestinations) {
            distancesToPotentialDestinations.Add(Vector2.Distance(pos, Vector2.zero));
        }

        float maxDistance = Mathf.Max(distancesToPotentialDestinations.ToArray());
        int maxDistanceIndex = distancesToPotentialDestinations.IndexOf(maxDistance);
        Vector2 destinationPosition = potentialDestinations[maxDistanceIndex];

        Transform destination = Instantiate(destinationCellPrefab, new Vector3(destinationPosition.x * (int)floorPrefab.localScale.x, destinationCellPrefab.localScale.y/2 + 0.001f, destinationPosition.y * (int)floorPrefab.localScale.z), Quaternion.identity);
        destination.parent = transform;
    }

    public void ScalePrefabs() {
        floorPrefab.localScale = new Vector3(cellSize, cellHeight, cellSize);
        destinationCellPrefab.localScale = new Vector3(cellSize, cellHeight, cellSize);
        wallPrefab.localScale = new Vector3(cellSize, wallHeight, wallThickness);
    }

    void InstantiateMaze() {
        for (int x = 0; x < mazeSize.x; x++) {
            for (int y = 0; y < mazeSize.y; y++) {
                float xScaled = x * (int)floorPrefab.localScale.x;
                float yScaled = y * (int)floorPrefab.localScale.z;

                // Instantiate the Floor Prefab for each cell.
                Transform newCell = Instantiate(floorPrefab, new Vector3(xScaled, floorPrefab.localScale.y/2, yScaled), Quaternion.identity);
                newCell.parent = transform;

                // Instantiate walls in the order [top, right, bottom, left].
                bool[] walls = cells[x, y].walls;
                if (walls[0]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xScaled - floorPrefab.localScale.x/2, wallPrefab.localScale.y/2, yScaled), Quaternion.Euler(0, -90, 0));
                    newWall.parent = transform;
                }
                if (walls[1]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xScaled, wallPrefab.localScale.y/2, yScaled + floorPrefab.localScale.z/2), Quaternion.identity);
                    newWall.parent = transform;
                }
                if (walls[2]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xScaled + floorPrefab.localScale.x/2 , wallPrefab.localScale.y/2, yScaled), Quaternion.Euler(0, -90, 0));
                    newWall.parent = transform;
                }
                if (walls[3]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xScaled, wallPrefab.localScale.y/2, yScaled - floorPrefab.localScale.z/2), Quaternion.identity);
                    newWall.parent = transform;
                }
            }
        }
    }
}

public class Cell {
    public bool[] walls = new bool[] {true, true, true, true};
    public bool visited;
    public Vector2 position;
    public Vector2[] neighborsPositions;

    public Cell(int i, int j) {
        position = new Vector2(i, j);
        neighborsPositions = new Vector2[] {new Vector2(i - 1, j), new Vector2(i, j + 1), new Vector2(i + 1, j), new Vector2(i, j - 1)};
    }
}

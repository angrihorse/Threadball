using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public Vector2 mazeSize;

    public Transform floorPrefab;
    public Transform destinationCellPrefab;
    public Transform wallPrefab;
    public Transform helperPrefab;

    public Stack stack = new Stack();

    public void GenerateMaze() {
        // Initialize the 2D array of cells.
        Cell[,] cells = new Cell[(int)mazeSize.x, (int)mazeSize.y];
        for (int i = 0; i < mazeSize.x; i++) {
            for (int j = 0; j < mazeSize.y; j++) {
                cells[i, j] = new Cell(new Vector2(i, j));
            }
        }

        Cell current = cells[0, 0];
        current.visited = true;
        void RecursiveBacktracker() {
            if (HasUnvisitedNeighbors(current)) {
                Cell next = GetRandomNeighbor(current, UnvisitedNeighbors(current));
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
            return potentialNextCells[Random.Range(0, potentialNextCells.Count)];
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

        bool HasUnvisitedNeighbors(Cell cell) {
            bool[] neighbors = UnvisitedNeighbors(cell);
            foreach (bool val in neighbors) {
                if (val) {
                    return true;
                }
            }
            return false;
        }

        // Instantiate the labyrinth.
        for (int x = 0; x < mazeSize.x; x++) {
            for (int y = 0; y < mazeSize.y; y++) {
                float xx = x * (int)floorPrefab.localScale.x;
                float yy = y * (int)floorPrefab.localScale.z;

                // Instantiate the Floor Prefab for each cell.
                Transform newCell = Instantiate(floorPrefab, new Vector3(xx, floorPrefab.localScale.y/2, yy), Quaternion.identity);
                newCell.parent = transform;

                // Instantiate walls in the order [top, right, bottom, left].
                bool[] walls = cells[x, y].walls;
                if (walls[0]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xx - wallPrefab.localScale.x/2, wallPrefab.localScale.y/2, yy - floorPrefab.localScale.z/2 + wallPrefab.localScale.x/2), Quaternion.Euler(0, -90, 0));
                    newWall.parent = transform;
                }
                if (walls[1]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xx, wallPrefab.localScale.y/2, yy + floorPrefab.localScale.z/2), Quaternion.identity);
                    newWall.parent = transform;
                }
                if (walls[2]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xx + wallPrefab.localScale.x/2, wallPrefab.localScale.y/2, yy - floorPrefab.localScale.z/2 + wallPrefab.localScale.x/2), Quaternion.Euler(0, -90, 0));
                    newWall.parent = transform;
                }
                if (walls[3]) {
                    Transform newWall = Instantiate(wallPrefab, new Vector3(xx, wallPrefab.localScale.y/2, yy - floorPrefab.localScale.z/2), Quaternion.identity);
                    newWall.parent = transform;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }
}


class Cell {
    public bool[] walls = new bool[] {true, true, true, true};
    public bool visited;
    public Vector2 position;
    public Vector2[] neighborsPositions;

    public Cell(Vector2 _position) {
        position = _position;
        neighborsPositions = new Vector2[] {new Vector2(position.x - 1, position.y),
                                            new Vector2(position.x, position.y + 1),
                                            new Vector2(position.x + 1, position.y),
                                            new Vector2(position.x, position.y - 1)};
    }


}

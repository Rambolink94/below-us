using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BelowUs
{
    public class MeshGenerator
    {
        int squareSize;
        int wallHeight;

        MeshData meshData;

        public MeshGenerator(int squareSize, int wallHeight)
        {
            this.squareSize = squareSize;
            this.wallHeight = wallHeight;
        }

        public MeshGenerator(MeshData meshData)
        {
            this.meshData = meshData;
        }

        public MeshData GenerateMapMeshes(int[,] floorMap)
        {
            SquareGrid squareGrid = new SquareGrid(floorMap, squareSize, wallHeight);
            meshData = new MeshData(squareGrid);

            int vertexIndex = 0;
            for (int x = 0; x < meshData.squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < meshData.squareGrid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(meshData.squareGrid.squares[x, y], meshData);

                    vertexIndex++;
                }
            }

            //CalculateUVs(meshData);
            return meshData;
        }

        public MeshData GenerateMapMeshes(MeshData meshData)
        {
            int vertexIndex = 0;
            for (int x = 0; x < meshData.squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < meshData.squareGrid.squares.GetLength(1); y++)
                {
                    TriangulateSquare(meshData.squareGrid.squares[x, y], meshData);

                    vertexIndex++;
                }
            }

            return meshData;
        }

        public MeshData UpdateMeshData(Vector3 hitPos, float radius, bool calledFromNeighbour)
        {
            bool placeBlock;
            if (radius == -1)
            {
                radius = 0.5f;
                placeBlock = true;
            }
            else
            {
                placeBlock = false;
            }

            // Loop through original square array and gather effected squares
            for (int x = 0; x < meshData.squareGrid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < meshData.squareGrid.squares.GetLength(1); y++)
                {
                    Square currentSquare = meshData.squareGrid.squares[x, y];

                    currentSquare.ResetVertexIndexes();

                    if (IsSquareWithinRange(currentSquare.squareCenter, hitPos, radius))
                    {
                        // Modify mesh data for later use
                        meshData.squareGrid.squares[x, y] = RecalculateConfiguration(currentSquare, hitPos, radius, calledFromNeighbour, placeBlock);
                    }
                }
            }

            //PrintGridInfo(meshData.squareGrid.squares);

            return GenerateMapMeshes(meshData);
        }

        public static void PrintGridInfo(Square[,] squares)
        {
            for (int x = 0; x < squares.GetLength(0); x++)
            {
                for (int y = 0; y < squares.GetLength(1); y++)
                {
                    Square currentSquare = squares[x, y];

                    Debug.Log("TopLeft: " + currentSquare.topLeft.active + " TopRight: " + currentSquare.topRight.active + " BottomLeft: " + currentSquare.bottomLeft.active + " BottomRight: " + currentSquare.bottomRight.active + " Config: " + currentSquare.configuration + " Pos: " + currentSquare.squareCenter);
                }
            }
        }

        bool IsSquareWithinRange(Vector3 squarePos, Vector3 rangeCenter, float radius)
        {
            float circleDistanceX = Mathf.Abs(rangeCenter.x - squarePos.x);
            float circleDistanceZ = Mathf.Abs(rangeCenter.z - squarePos.z);

            if (circleDistanceX > (squareSize / 2 + radius)) return false;
            if (circleDistanceZ > (squareSize / 2 + radius)) return false;

            if (circleDistanceX <= (squareSize / 2)) return true;
            if (circleDistanceZ <= (squareSize / 2)) return true;

            float cornerDistance = ((circleDistanceX - squareSize / 2) * (circleDistanceX - squareSize / 2)) + ((circleDistanceZ - squareSize / 2) * (circleDistanceZ - squareSize / 2));

            return cornerDistance <= radius * radius;
        }

        Square RecalculateConfiguration(Square square, Vector3 rangeCenter, float radius, bool calledFromNeighbor, bool placeBlock)
        {
            // Change to config of top left
            if (IsSquareWithinRange(square.GetCenterOfTwoPoints(square.topLeft.position, square.squareCenter), rangeCenter, radius))
            {
                HandleNeighbourSquaresV2(square.topLeft, rangeCenter, false, false, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.ceilingTopLeft, rangeCenter, false, true, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.topWallLeft, rangeCenter, true, false, calledFromNeighbor, placeBlock);

                //Debug.Log("Hit Top Left: " + square.topLeft.position + " , " + square.squareCenter + " = " + square.GetCenterOfTwoPoints(square.topLeft.position, square.squareCenter));
            }

            // Change to config of top right
            if (IsSquareWithinRange(square.GetCenterOfTwoPoints(square.topRight.position, square.squareCenter), rangeCenter, radius))
            {
                HandleNeighbourSquaresV2(square.topRight, rangeCenter, false, false, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.ceilingTopRight, rangeCenter, false, true, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.topWallRight, rangeCenter, true, false, calledFromNeighbor, placeBlock);

                //Debug.Log("Hit Top Right: " + square.topRight.position + " , " + square.squareCenter + " = " + square.GetCenterOfTwoPoints(square.topRight.position, square.squareCenter));
            }

            // Change to config of bottom left
            if (IsSquareWithinRange(square.GetCenterOfTwoPoints(square.bottomLeft.position, square.squareCenter), rangeCenter, radius))
            {
                HandleNeighbourSquaresV2(square.bottomLeft, rangeCenter, false, false, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.ceilingBottomLeft, rangeCenter, false, true, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.bottomWallLeft, rangeCenter, true, false, calledFromNeighbor, placeBlock);

                //Debug.Log("Hit Bottom Left: " + square.bottomLeft.position + " , " + square.squareCenter + " = " + square.GetCenterOfTwoPoints(square.bottomLeft.position, square.squareCenter));
            }

            // Change to config of bottom right
            if (IsSquareWithinRange(square.GetCenterOfTwoPoints(square.bottomRight.position, square.squareCenter), rangeCenter, radius))
            {
                HandleNeighbourSquaresV2(square.bottomRight, rangeCenter, false, false, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.ceilingBottomRight, rangeCenter, false, true, calledFromNeighbor, placeBlock);
                HandleNeighbourSquaresV2(square.bottomWallRight, rangeCenter, true, false, calledFromNeighbor, placeBlock);

                //Debug.Log("Hit Bottom Right: " + square.bottomRight.position + " , " + square.squareCenter + " = " + square.GetCenterOfTwoPoints(square.bottomRight.position, square.squareCenter));
            }

            return square;
        }

        void HandleNeighbourSquaresV2(ControlNode controlNode, Vector3 hitPos, bool isWall, bool isCeiling, bool calledFromNeighbor, bool placeBlock)
        {
            List<Square> neighbours;

            if (isWall)
            {
                neighbours = meshData.squareGrid.sharedWallControlNodes[controlNode];
            }
            else if (isCeiling)
            {
                neighbours = meshData.squareGrid.sharedCeilingControlNodes[controlNode];
            }
            else
            {
                neighbours = meshData.squareGrid.sharedControlNodes[controlNode];
            }

            if (placeBlock)
            {
                controlNode.active = false;
            }
            else
            {
                controlNode.active = true;
            }

            foreach (Square square in neighbours)
            {
                square.GetConfiguration();
            }

            //if (!calledFromNeighbor && controlNode.controlNodeType != ControlNodeType.CentralNode) WorldGenerator.TriggerNeighborUpdate(hitPos);
        }

        List<Square> CheckForEdgeNeighbours(List<Square> neighbours, ControlNode controlNode)
        {
            int count = neighbours.Count;

            // Not an edge node
            if (count == 4)
            {
                return neighbours;
            }

            if (count == 2) // Edge of chunk
            {

            }
            else if (count == 1) // Corner of chunk
            {

            }

            return neighbours;
        }

        void TriangulateSquare(Square square, MeshData meshData)
        {
            // Determine what the square's mesh should look like based on the configuration of the square.
            // There are 16 possible configurations.
            switch (square.configuration)
            {
                case 0:
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingTopRight, square.ceilingBottomRight, square.ceilingBottomLeft);
                    break;

                // 1 points:
                case 1:
                    MeshFromPoints(meshData, false, square.centerLeft, square.centerBottom, square.bottomLeft);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingTopRight, square.ceilingBottomRight, square.ceilingCenterBottom, square.ceilingCenterLeft);
                    MeshFromPoints(meshData, true, square.centerLeft, square.centerWallLeft, square.centerWallBottom, square.centerBottom);
                    break;
                // .__.__.
                // |  |  |
                // .__.__.
                // |\ |  |
                // .+\.__.
                case 2:
                    MeshFromPoints(meshData, false, square.bottomRight, square.centerBottom, square.centerRight);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingTopRight, square.ceilingCenterRight, square.ceilingCenterBottom, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerBottom, square.centerWallBottom, square.centerWallRight, square.centerRight);
                    break;
                // .__.__.
                // |  |  |
                // .__.__.
                // |  | /|
                // .__./+.
                case 4:
                    MeshFromPoints(meshData, false, square.topRight, square.centerRight, square.centerTop);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingCenterTop, square.ceilingCenterRight, square.ceilingBottomRight, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerRight, square.centerWallRight, square.centerWallTop, square.centerTop);
                    break;
                // .__.__.
                // |  |\+|
                // .__._\.
                // |  |  |
                // .__.__.
                case 8:
                    MeshFromPoints(meshData, false,square.topLeft, square.centerTop, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterLeft, square.ceilingCenterTop, square.ceilingTopRight, square.ceilingBottomRight, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerTop, square.centerWallTop, square.centerWallLeft, square.centerLeft);
                    break;
                // .__.__.
                // |+/|  |
                // ./_.__.
                // |  |  |
                // .__.__.

                // 2 points:
                case 3:
                    MeshFromPoints(meshData, false, square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingTopRight, square.ceilingCenterRight, square.ceilingCenterLeft);
                    MeshFromPoints(meshData, true, square.centerLeft, square.centerWallLeft, square.centerWallRight, square.centerRight);
                    break;
                // .__.__.
                // |  |  |
                // .__.__.
                // |++|++|
                // .++.++.
                case 6:
                    MeshFromPoints(meshData, false, square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingCenterTop, square.ceilingCenterBottom, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerBottom, square.centerWallBottom, square.centerWallTop, square.centerTop);
                    break;
                // .__.__.
                // |  |++|
                // .__.++.
                // |  |++|
                // .__.++.
                case 9:
                    MeshFromPoints(meshData, false, square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterTop, square.ceilingTopRight, square.ceilingBottomRight, square.ceilingCenterBottom);
                    MeshFromPoints(meshData, true, square.centerTop, square.centerWallTop, square.centerWallBottom, square.centerBottom);
                    break;
                // .__.__.
                // |++|  |
                // .++.__.
                // |++|  |
                // .++.__.
                case 12:
                    MeshFromPoints(meshData, false, square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterLeft, square.ceilingCenterRight, square.ceilingBottomRight, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerRight, square.centerWallRight, square.centerWallLeft, square.centerLeft);
                    break;
                // .__.__.
                // |++|++|
                // .++.++.
                // |  |  |
                // .__.__.
                case 5:
                    MeshFromPoints(meshData, false, square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingCenterTop, square.ceilingCenterLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterBottom, square.ceilingCenterRight, square.ceilingBottomRight);
                    MeshFromPoints(meshData, true, square.centerLeft, square.centerWallLeft, square.centerWallTop, square.centerTop);
                    MeshFromPoints(meshData, true, square.centerRight, square.centerWallRight, square.centerWallBottom, square.centerBottom);
                    break;
                // .__.__.
                // | /|++|
                // ./+.++.
                // |++|+/|
                // .++./_.
                case 10:
                    MeshFromPoints(meshData, false, square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterTop, square.ceilingTopRight, square.ceilingCenterRight);
                    MeshFromPoints(meshData, false, square.ceilingCenterLeft, square.ceilingCenterBottom, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerTop, square.centerWallTop, square.centerWallRight, square.centerRight);
                    MeshFromPoints(meshData, true, square.centerBottom, square.centerWallBottom, square.centerWallLeft, square.centerLeft);
                    break;
                // .__.__.
                // |++|\ |
                // .++.+\.
                // |\+|++|
                // ._\.++.

                // 3 points:
                case 7:
                    MeshFromPoints(meshData, false, square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingTopLeft, square.ceilingCenterTop, square.ceilingCenterLeft);
                    MeshFromPoints(meshData, true, square.centerLeft, square.centerWallLeft, square.centerWallTop, square.centerTop);
                    break;
                // .__.__.
                // | /|++|
                // ./+.++.
                // |++|++|
                // .++.++.
                case 11:
                    MeshFromPoints(meshData, false, square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterTop, square.ceilingTopRight, square.ceilingCenterRight);
                    MeshFromPoints(meshData, true, square.centerTop, square.centerWallTop, square.centerWallRight, square.centerRight);
                    break;
                // .__.__.
                // |++|\ |
                // .++.+\.
                // |++|++|
                // .++.++.
                case 13:
                    MeshFromPoints(meshData, false, square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterBottom, square.ceilingCenterRight, square.ceilingBottomRight);
                    MeshFromPoints(meshData, true, square.centerRight, square.centerWallRight, square.centerWallBottom, square.centerBottom);
                    break;
                // .__.__.
                // |++|++|
                // .++.++.
                // |++|+/|
                // .++./_.
                case 14:
                    MeshFromPoints(meshData, false, square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                    MeshFromPoints(meshData, false, square.ceilingCenterLeft, square.ceilingCenterBottom, square.ceilingBottomLeft);
                    MeshFromPoints(meshData, true, square.centerBottom, square.centerWallBottom, square.centerWallLeft, square.centerLeft);
                    break;
                // .__.__.
                // |++|++|
                // .++.++.
                // |\+|++|
                // ._\.++.

                // 4 point:
                case 15:
                    MeshFromPoints(meshData, false, square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                    break;
                // .__.__.
                // |++|++|
                // .++.++.
                // |++|++|
                // .++.++.
                default:
                    Debug.LogWarning("The case that you are attempting to add doesn't exist.");
                    break;
            }
        }

        void MeshFromPoints(MeshData meshData, bool isWall, params Node[] points)
        {
            AssignVertices(points, isWall, meshData);

            if (points.Length >= 3) CreateTriangle(meshData, points[0], points[1], points[2]);
            if (points.Length >= 4) CreateTriangle(meshData, points[0], points[2], points[3]);
            if (points.Length >= 5) CreateTriangle(meshData, points[0], points[3], points[4]);
            if (points.Length >= 6) CreateTriangle(meshData, points[0], points[4], points[5]);
        }

        void AssignVertices(Node[] points, bool isWallVert, MeshData meshData)
        {
            for (int i = 0; i < points.Length; i++)
            {
                // Vertex not assigned
                if (points[i].vertexIndex == -1)
                {
                    points[i].vertexIndex = meshData.vertices.Count;
                    meshData.vertices.Add(points[i].position);
                }
            }
        }

        void CreateTriangle(MeshData meshData, Node a, Node b, Node c)
        {
            meshData.AddTriangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;
        public Dictionary<ControlNode, List<Square>> sharedControlNodes = new Dictionary<ControlNode, List<Square>>();
        public Dictionary<ControlNode, List<Square>> sharedCeilingControlNodes = new Dictionary<ControlNode, List<Square>>();
        public Dictionary<ControlNode, List<Square>> sharedWallControlNodes = new Dictionary<ControlNode, List<Square>>();

        public SquareGrid(int[,] map, float squareSize, float wallHeight)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
            ControlNode[,] ceilingControlNodes = new ControlNode[nodeCountX, nodeCountY];
            ControlNode[,] wallControlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    // Determines center of generated control node.
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize, nodeCountX, x, y);

                    Vector3 ceilingPos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, wallHeight, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    ceilingControlNodes[x, y] = new ControlNode(ceilingPos, map[x, y] == 1, squareSize, nodeCountX, x, y);

                    Vector3 wallPos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, wallHeight, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    wallControlNodes[x, y] = new ControlNode(wallPos, map[x, y] == 1, squareSize, nodeCountX, x, y);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(
                        controlNodes[x, y + 1],
                        controlNodes[x + 1, y + 1],
                        controlNodes[x, y],
                        controlNodes[x + 1, y],
                        ceilingControlNodes[x, y + 1],
                        ceilingControlNodes[x + 1, y + 1],
                        ceilingControlNodes[x, y],
                        ceilingControlNodes[x + 1, y],
                        wallControlNodes[x, y + 1],
                        wallControlNodes[x + 1, y + 1],
                        wallControlNodes[x, y],
                        wallControlNodes[x + 1, y],
                        x,
                        y);

                    AssignSharedControlNodes(squares[x, y]);
                }
            }
        }

        void AssignSharedControlNodes(Square currentSquare)
        {
            // Assign Floor Nodes
            AssignControlNode(currentSquare.topLeft, currentSquare, false, false);
            AssignControlNode(currentSquare.topRight, currentSquare, false, false);
            AssignControlNode(currentSquare.bottomRight, currentSquare, false, false);
            AssignControlNode(currentSquare.bottomLeft, currentSquare, false, false);

            // Assign Ceiling Nodes
            AssignControlNode(currentSquare.ceilingTopLeft, currentSquare, false, true);
            AssignControlNode(currentSquare.ceilingTopRight, currentSquare, false, true);
            AssignControlNode(currentSquare.ceilingBottomRight, currentSquare, false, true);
            AssignControlNode(currentSquare.ceilingBottomLeft, currentSquare, false, true);

            // Assign Wall Nodes
            AssignControlNode(currentSquare.topWallLeft, currentSquare, true, false);
            AssignControlNode(currentSquare.topWallRight, currentSquare, true, false);
            AssignControlNode(currentSquare.bottomWallRight, currentSquare, true, false);
            AssignControlNode(currentSquare.bottomWallLeft, currentSquare, true, false);
        }

        void AssignControlNode(ControlNode controlNode, Square currentSquare, bool isWallNode, bool isCeilingNode)
        {
            if (isWallNode)
            {
                if (sharedWallControlNodes.ContainsKey(controlNode))
                {
                    sharedWallControlNodes[controlNode].Add(currentSquare);
                }
                else
                {
                    sharedWallControlNodes.Add(controlNode, new List<Square>());
                    sharedWallControlNodes[controlNode].Add(currentSquare);
                }
            }
            else if (isCeilingNode)
            {
                if (sharedCeilingControlNodes.ContainsKey(controlNode))
                {
                    sharedCeilingControlNodes[controlNode].Add(currentSquare);
                }
                else
                {
                    sharedCeilingControlNodes.Add(controlNode, new List<Square>());
                    sharedCeilingControlNodes[controlNode].Add(currentSquare);
                }
            }
            else
            {
                
                if (sharedControlNodes.ContainsKey(controlNode))
                {
                    sharedControlNodes[controlNode].Add(currentSquare);
                }
                else
                {
                    sharedControlNodes.Add(controlNode, new List<Square>());
                    sharedControlNodes[controlNode].Add(currentSquare);
                }
            }
        }
    }

    public class Square
    {
        public int health = 10;

        // Corner Nodes - Control Nodes
        public ControlNode topLeft;
        public ControlNode topRight;
        public ControlNode bottomLeft;
        public ControlNode bottomRight;

        // Corner Ceiling Nodes - Control Nodes
        public ControlNode ceilingTopLeft;
        public ControlNode ceilingTopRight;
        public ControlNode ceilingBottomLeft;
        public ControlNode ceilingBottomRight;

        // Corner Wall Nodes = Control Nodes
        public ControlNode topWallLeft;
        public ControlNode topWallRight;
        public ControlNode bottomWallLeft;
        public ControlNode bottomWallRight;

        // Central Nodes - Non-control Nodes
        public Node centerLeft;
        public Node centerTop;
        public Node centerRight;
        public Node centerBottom;

        // Central Ceiling Nodes - Non-control Nodes
        public Node ceilingCenterLeft;
        public Node ceilingCenterTop;
        public Node ceilingCenterRight;
        public Node ceilingCenterBottom;

        // Central Wall Nodes - Non-control Nodes
        public Node centerWallLeft;
        public Node centerWallTop;
        public Node centerWallRight;
        public Node centerWallBottom;

        public Vector3 squareCenter;

        public int xIndex;
        public int yIndex;
        public int configuration;

        public Square
            (
            ControlNode _topLeft, 
            ControlNode _topRight,
            ControlNode _bottomLeft,
            ControlNode _bottomRight,
            ControlNode _ceilingTopLeft,
            ControlNode _ceilingTopRight,
            ControlNode _ceilingBottomLeft,
            ControlNode _ceilingBottomRight,
            ControlNode _topWallLeft,
            ControlNode _topWallRight,
            ControlNode _bottomWallLeft,
            ControlNode _bottomWallRight,
            int x,
            int y
            )
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomLeft = _bottomLeft;
            bottomRight = _bottomRight;

            ceilingTopLeft = _ceilingTopLeft;
            ceilingTopRight = _ceilingTopRight;
            ceilingBottomLeft = _ceilingBottomLeft;
            ceilingBottomRight = _ceilingBottomRight;

            topWallLeft = _topWallLeft;
            topWallRight = _topWallRight;
            bottomWallLeft = _bottomWallLeft;
            bottomWallRight = _bottomWallRight;

            centerLeft = bottomLeft.above;
            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;

            ceilingCenterLeft = ceilingBottomLeft.above;
            ceilingCenterTop = ceilingTopLeft.right;
            ceilingCenterRight = ceilingBottomRight.above;
            ceilingCenterBottom = ceilingBottomLeft.right;

            centerWallLeft = bottomWallLeft.above;
            centerWallTop = topWallLeft.right;
            centerWallRight = bottomWallRight.above;
            centerWallBottom = bottomWallLeft.right;

            xIndex = x;
            yIndex = y;

            squareCenter = GetCenterOfTwoPoints(topLeft.position, bottomRight.position);

            GetConfiguration();
        }

        public Vector3 GetCenterOfTwoPoints(Vector3 point1, Vector3 point2)
        {
            // Midpoint Formula = (x1 + x2) / 2, (y1 + y2) / 2
            return new Vector3((point1.x + point2.x) / 2, 0, (point1.z + point2.z) / 2);
        }

        public void GetConfiguration(bool logConfig = false)
        {
            configuration = 0;

            if (topLeft.active) configuration += 8;
            if (topRight.active) configuration += 4;
            if (bottomLeft.active) configuration += 1;
            if (bottomRight.active) configuration += 2;

            if (logConfig) Debug.Log(configuration);
        }

        public void ResetVertexIndexes()
        {
            // Ugly and repetative

            topLeft.vertexIndex = -1;
            topRight.vertexIndex = -1;
            bottomLeft.vertexIndex = -1;
            bottomRight.vertexIndex = -1;

            ceilingTopLeft.vertexIndex = -1;
            ceilingTopRight.vertexIndex = -1;
            ceilingBottomLeft.vertexIndex = -1;
            ceilingBottomRight.vertexIndex = -1;

            topWallLeft.vertexIndex = -1;
            topWallRight.vertexIndex = -1;
            bottomWallLeft.vertexIndex = -1;
            bottomWallRight.vertexIndex = -1;

            centerLeft.vertexIndex = -1;
            centerTop.vertexIndex = -1;
            centerRight.vertexIndex = -1;
            centerBottom.vertexIndex = -1;

            ceilingCenterLeft.vertexIndex = -1;
            ceilingCenterTop.vertexIndex = -1;
            ceilingCenterRight.vertexIndex = -1;
            ceilingCenterBottom.vertexIndex = -1;

            centerWallLeft.vertexIndex = -1;
            centerWallTop.vertexIndex = -1;
            centerWallRight.vertexIndex = -1;
            centerWallBottom.vertexIndex = -1;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;
        public int uvIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above;
        public Node right;
        public ControlNodeType controlNodeType;

        public ControlNode(Vector3 _pos, bool _active, float squareSize, int nodeCount, int x, int y) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
            controlNodeType = DetermineNodeType(nodeCount, x, y);
        }

        ControlNodeType DetermineNodeType(int nodeCount, int x, int y)
        {
            // Determines whether a Node is on an edge, corner, or center of the square grid / current chunk.
            if (x == 0)
            {
                if (y == 0) return ControlNodeType.BottomLeftCorner;
                else if (y + 1 == nodeCount) return ControlNodeType.TopLeftCorner;
                else return ControlNodeType.LeftEdge;
            }
            else if (x + 1 == nodeCount)
            {
                if (y == 0) return ControlNodeType.BottomRightCorner;
                else if (y + 1 == nodeCount) return ControlNodeType.TopRightCorner;
                else return ControlNodeType.RightEdge;
            }
            else
            {
                if (y == 0) return ControlNodeType.BottomEdge;
                else if (y + 1 == nodeCount) return ControlNodeType.TopEdge;
                else return ControlNodeType.CentralNode;
            }
        }
    }

    public enum ControlNodeType
    {
        CentralNode,
        LeftEdge,
        TopEdge,
        RightEdge,
        BottomEdge,
        TopLeftCorner,
        TopRightCorner,
        BottomLeftCorner,
        BottomRightCorner
    }

    public class MeshData
    {
        public SquareGrid squareGrid;

        // Floor Mesh
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> uvs;
        public Vector2[] uvs2;

        public MeshData(SquareGrid _squareGrid)
        {
            squareGrid = _squareGrid;

            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        public void ClearBasicMeshData()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            //Debug.Log("Vertices: " + mesh.vertices.Length + " UVS: " + uvs2.Length);
            //mesh.uv = uvs.ToArray();
            //mesh.uv = uvs2;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            mesh.MarkDynamic();

            return mesh;
        }
    }
}

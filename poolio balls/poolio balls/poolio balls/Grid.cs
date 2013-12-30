using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace poolio_balls
{
    public class Grid
    {
        public GridNode[,] nodes;
        public int NodeSize { get; private set; }

        public Grid(int gridWidth, int gridHeight, int nodeSize)
        {
            int nodeArrayWidth = gridWidth / nodeSize;
            int nodeArrayHeight = gridHeight / nodeSize;

            if (gridWidth / (float)nodeSize != gridWidth / nodeSize)
                nodeArrayWidth++;
            if (gridHeight / (float)nodeSize != gridHeight / nodeSize)
                nodeArrayHeight++;

            nodes = new GridNode[nodeArrayWidth, nodeArrayHeight];
            NodeSize = nodeSize;

            createNodes(nodeArrayWidth, nodeArrayHeight);
            calculateNodeNeighbors(nodeArrayWidth, nodeArrayHeight);
        }

        void createNodes(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new GridNode(new Rectangle(x * NodeSize, y * NodeSize, NodeSize, NodeSize));
                }
            }
        }

        void calculateNodeNeighbors(int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int s = 0; s < height; s++)
                {
                    GridNode tile = nodes[i, s];

                    if (i - 1 >= 0)
                    {
                        GridNode neighbor = nodes[i - 1, s];
                        tile.Neighbors.Add(neighbor);
                        tile.WestNeighbor = neighbor;
                    }
                    if (i + 1 < width)
                    {
                        GridNode neighbor = nodes[i + 1, s];
                        tile.Neighbors.Add(neighbor);
                        tile.EastNeighbor = neighbor;
                    }
                    if (s - 1 >= 0)
                    {
                        GridNode neighbor = nodes[i, s - 1];
                        tile.Neighbors.Add(neighbor);
                        tile.NorthNeighbor = neighbor;
                    }
                    if (s + 1 < height)
                    {
                        GridNode neighbor = nodes[i, s + 1];
                        tile.Neighbors.Add(neighbor);
                        tile.SouthNeighbor = neighbor;
                    }
                    if (i - 1 >= 0 && s - 1 >= 0)
                    {
                        GridNode neighbor = nodes[i - 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                        tile.NorthWestNeighbor = neighbor;
                    }
                    if (i - 1 >= 0 && s + 1 < height)
                    {
                        GridNode neighbor = nodes[i - 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                        tile.SouthWestNeighbor = neighbor;
                    }
                    if (i + 1 < width && s - 1 >= 0)
                    {
                        GridNode neighbor = nodes[i + 1, s - 1];
                        tile.Neighbors.Add(neighbor);
                        tile.NorthEastNeighbor = neighbor;
                    }
                    if (i + 1 < width && s + 1 < height)
                    {
                        GridNode neighbor = nodes[i + 1, s + 1];
                        tile.Neighbors.Add(neighbor);
                        tile.SouthEastNeighbor = neighbor;
                    }
                }
            }
        }

        public GridNode NodeAt(float x, float y)
        {
            int gridX = (int)(x / NodeSize);
            int gridY = (int)(y / NodeSize);

            return nodes[gridX, gridY];
        }
    }

    public class GridNode
    {
        public List<GridNode> Neighbors = new List<GridNode>();
        public GridNode NorthWestNeighbor, NorthNeighbor, NorthEastNeighbor,
            EastNeighbor, SouthEastNeighbor, SouthNeighbor, SouthWestNeighbor,
            WestNeighbor;

        public List<Ball> BallsContained = new List<Ball>();
        public List<Polygon> PolygonsContained = new List<Polygon>();

        public Rectangle Rectangle { get; private set; }

        public GridNode(Rectangle rect)
        {
            Rectangle = rect;
        }
    }
}

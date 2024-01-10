using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Classification
{


    class DBSCAN
    {
        private List<Point> Points;
        private List<List<Point>> clusters;

        public DBSCAN(List<Point> points)
        {
            Points = points;
            clusters = new List<List<Point>>();
        }

        public List<List<Point>> Cluster(float epsilon, int minPoints, float[] featureWeights)
        {
            List<Point> visited = new List<Point>();
            //Debug.LogError($"Visited: {visited.Count}");

            foreach (Point point in Points)
            {
                if (visited.Contains(point))
                    continue;

                visited.Add(point);

                List<Point> neighbors = GetNeighbors(point, epsilon, featureWeights);
                //Debug.LogError($"Neighbors: {neighbors.Count}");

                if (neighbors.Count >= minPoints)
                {
                    float clusterHue =  Random.Range(0f, 1f);
                    //Debug.LogError($"Create Cluster");
                    List<Point> cluster = ExpandCluster(point, neighbors, epsilon, minPoints, visited, featureWeights, clusterHue);
                    clusters.Add(cluster);
                }
            }

            return clusters;
        }

        private List<Point> ExpandCluster(Point point, List<Point> neighbors, float epsilon, int minPoints,
            List<Point> visited, float[] featureWeights, float clusterHue)
        {
            List<Point> cluster = new List<Point> {point};
            Queue<Point> queue = new Queue<Point>(neighbors);

            point.ClusterHue = clusterHue; 
            //Debug.LogError($"Add Point {point.Name} to cluster");
            while (queue.Count > 0)
            {
                Point currentPoint = queue.Dequeue();

                if (!visited.Contains(currentPoint))
                {
                    visited.Add(currentPoint);
                    List<Point> currentNeighbors = GetNeighbors(currentPoint, epsilon, featureWeights);

                    if (currentNeighbors.Count > 1)
                    {
                        foreach (Point newNeighbor in currentNeighbors)
                        {
                            if (!visited.Contains(newNeighbor))
                            {
                                queue.Enqueue(newNeighbor);
                            }
                        }
                    }

                    if (!IsInAnyCluster(currentPoint))
                    {
                        currentPoint.ClusterHue = clusterHue;
                        cluster.Add(currentPoint);
                        //Debug.LogError($"Add Point {currentPoint.Name} to cluster");
                    }

                }

            }

            return cluster;
        }

        private List<Point> GetNeighbors(Point point, float epsilon, float[] featureWeights)
        {
            List<Point> neighbors = new List<Point>();

            foreach (Point otherPoint in Points)
            {
                if (point.WeightedDistanceTo(otherPoint, featureWeights) <= epsilon)
                {
                    neighbors.Add(otherPoint);
                }
            }

            return neighbors;
        }

        private bool IsInAnyCluster(Point point)
        {
            return clusters.Any(cluster => cluster.Contains(point));
        }
    }

    public class Point
    {
        public float[] Coordinates { get; }
        public float ClusterHue;
        public string Name;

        public Point(float[] coordinates, string name)
        {
            Coordinates = coordinates;
            ClusterHue = 0.267f;
            Name = name;
        }

        public float WeightedDistanceTo(Point other, float[] featureWeights)
        {
            float sum = 0;

            for (int i = 0; i < Coordinates.Length; i++)
            {
                float difference = Mathf.Abs(Coordinates[i] - other.Coordinates[i]);
                sum += difference / featureWeights[i];
            }
            sum /= featureWeights.Length;
            //Debug.LogError($"Distance between {Name} and {other.Name} is {sum}");
            return sum;
        }
    }
}

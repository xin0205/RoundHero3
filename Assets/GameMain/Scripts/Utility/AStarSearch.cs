﻿using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using UnityEngine;

namespace RoundHero
{
    public class AStarSearch
    {
        
        public enum EPointType
        {
            Empty = 0,
            Pass = 1,
            Obstacle = 2,
        }
        
        public enum EDirection
        {
            Left,
            LeftUp,
            Up,
            RightUp,
            Right,
            RightDown,
            Down,
            LeftDown,
            None,
        }
        
        public static Dictionary<Vector2Int, EDirection> Coord2DirectMap = new()
        {
            [new(-1, 0)] = EDirection.Left,

            [new(-1, 1)] = EDirection.LeftUp,

            [new(0, 1)] = EDirection.Up,

            [new(1, 1)] = EDirection.RightUp,

            [new(1, 0)] = EDirection.Right,

            [new(1, -1)] = EDirection.RightDown,

            [new(0, -1)] = EDirection.Down,

            [new(-1, -1)] = EDirection.LeftDown,

        };
        
        
        private const int OBLIQUE = 10;
        private const int STEP = 10;
        private const int WHEEL = 20;
        private const int OBSTACLE = 100000;
        
        private EPointType[,] MazeArray { get; set; }
        private List<Point> CloseList;
        private List<Point> OpenList;
        private Vector2Int EndCoord;

        private bool isOblique;
        private bool IsIgnoreCorner;
        private bool isWheel = false;

        public void Init(EPointType[,] maze, bool isIgnoreCorner, bool isQblique = false)
        {
            this.MazeArray = maze;
            OpenList = new List<Point>(MazeArray.Length);
            CloseList = new List<Point>(MazeArray.Length);

            this.isOblique = isQblique;
            this.IsIgnoreCorner = isIgnoreCorner;
        }
        

        public static List<Vector2Int> GetPathList(EPointType[,] maze, Vector2Int start, Vector2Int end, bool isQblique = true, bool isIgnoreCorner = true)
        {
            var aStarSearch = new AStarSearch();
            var startPoint = new Point(start);
            var endPoint = new Point(end);
            
            var point = aStarSearch.FindPath(maze, startPoint, endPoint, isQblique, isIgnoreCorner);
            
            var paths = new List<Vector2Int>();
            if (point == null)
                return paths;
            
           if (point.F > OBSTACLE)
                return paths;
            
            while (point != null)
            {
                paths.Add(new Vector2Int(point.X, point.Y));
                point = point.ParentPoint;
            }

            paths.Reverse();

            return paths;
        }

        public Point FindPath(EPointType[,] maze, Point start, Point end, bool isQblique = true, bool isIgnoreCorner = true)
        {
            EndCoord = new Vector2Int(end.X, end.Y);
            Init(maze, isIgnoreCorner, isQblique);
            
            OpenList.Add(start);
            while (OpenList.Count != 0)
            {
                //找出F值最小的点
                var tempStart = MinPoint(ref OpenList);
                OpenList.RemoveAt(0);
                CloseList.Add(tempStart);
     
                //找出它相邻的点
                var surroundPoints = SurroundPoints(tempStart);
                foreach (Point point in surroundPoints)
                {
                    if (OpenList.Exists(point))
                    {
                        var point2 = OpenList.Get(point);
                        //计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                        FoundPoint(tempStart, point2);
                    }
                        
                    else
                        //如果它们不在开始列表里, 就加入, 并设置父节点,并计算GHF
                        NotFoundPoint(tempStart, end, point);
                }

                var endPoint = OpenList.Get(end);
                if (endPoint != null && WheelCount(endPoint) == 0)
                {
                    //endPoint.H = OBSTACLE;
                    endPoint.CalcF();
                }
                
                if (endPoint != null && endPoint.F < OBSTACLE)
                    return OpenList.Get(end);
                
            }
            return OpenList.Get(end);
        }

        private int WheelCount(Point point)
        {
            var wheelCount = 0;
            while (point != null)
            {
                var p = point.ParentPoint;
                if (p != null)
                {
                    var pp = p.ParentPoint;
                    if (pp != null)
                    {
                        if (!(pp.X - p.X == p.X - point.X && pp.Y - p.Y == p.Y - point.Y))
                        {
                            wheelCount++;
                        }
                    }
                }
                
                point = point.ParentPoint;
                
            }

            return wheelCount;
        }

        private void FoundPoint(Point tempStart, Point point)
        {
            var G = CalcG(tempStart, point);
            var direction = CalcDirection(tempStart, point);
            var wheelCount = CalcWheelCount(tempStart, point);
            if (G < point.G || G == point.G && wheelCount < point.WheelCount)
            {
                point.ParentPoint = tempStart;
                point.G = G;
                point.Direction = direction;
                point.WheelCount = wheelCount;
                point.CalcF();
            }
        }

        private void NotFoundPoint(Point tempStart, Point end, Point point)
        {
            point.ParentPoint = tempStart;
            point.Direction = CalcDirection(tempStart, point);
            point.WheelCount = CalcWheelCount(tempStart, point);
            point.G = CalcG(tempStart, point);                    
            point.H = CalcH(end, point);
            point.CalcF();
            OpenList.Add(point);
        }


        private EDirection CalcDirection(Point prePoint, Point curPoint)
        {
            if (prePoint == null || curPoint == null)
                return EDirection.None;
            

            return CalcDirection(new Vector2Int(prePoint.X, prePoint.Y), new Vector2Int(curPoint.X, curPoint.Y));
        }
        
        private EDirection CalcDirection(Vector2Int preCoord, Vector2Int curCoord)
        {
            var deltaCoord = curCoord - preCoord;
            
            if (!Coord2DirectMap.ContainsKey(deltaCoord))
            {
                return EDirection.None;
            }
            
            return Coord2DirectMap[deltaCoord];
        }

        private int CalcG(Point start, Point point)
        {
            int G = (Math.Abs(point.X - start.X) + Math.Abs(point.Y - start.Y)) == 2 ?  OBLIQUE : STEP;
            if (start.ParentPoint != null)
            {
                if (start.ParentPoint.X - start.X != start.X - point.X ||
                    start.ParentPoint.Y - start.Y != start.Y - point.Y ||
                    (point.X == EndCoord.x && point.Y == EndCoord.y))
                {
                    // && !(point.X == EndCoord.x && point.Y == EndCoord.y)
                    if (!CanReach(start.X, start.Y))
                    {
                        G = OBSTACLE;
                    }
                    else
                    {
                        G = WHEEL;
                    }
                }
                
            }

            int parentG = point.ParentPoint != null ? point.ParentPoint.G : 0;
            return G + parentG;
        }
        
        private int CalcWheelCount(Point start, Point point)
        {
            var preDirection = CalcDirection(start.ParentPoint, start);
            var direction = CalcDirection(start, point);

            if (preDirection == EDirection.None || direction == EDirection.None)
                return start.WheelCount;
            
            if (preDirection == direction)
                return start.WheelCount;

            return start.WheelCount + 1;
            
            
        }

        private int CalcH(Point end, Point point)
        {
            int step = Math.Abs(point.X - end.X) + Math.Abs(point.Y - end.Y);
            return step * STEP;
        }

        //获取某个点周围可以到达的点
        public List<Point> SurroundPoints(Point point)
        {
            var surroundPoints = new List<Point>(9);
            var coord = new Vector2Int(point.X, point.Y);
            
            
            for (int x = point.X - 1; x <= point.X + 1; x++)
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                {
                    if(x < 0 || x >= MazeArray.GetLength(0) || y < 0 || y >= MazeArray.GetLength(1))
                        continue;

                    var direct = CalcDirection(coord, new Vector2Int(x, y));
                    
                    if (!isOblique && Math.Abs(x - point.X) + Math.Abs(y - point.Y) == 1 || isOblique)
                    {
                        if (CanPass(point, x, y, direct))
                            surroundPoints.Add(x, y);
                    }
                    
                }
            return surroundPoints;
        }

        //在二维数组对应的位置不为障碍物
        private bool CanPass(int x, int y)
        {
            //  ||(x == EndCoord.x && y == EndCoord.y)
            return MazeArray[x, y] == EPointType.Empty || MazeArray[x, y] == EPointType.Pass;
        }
        
        private bool CanReach(int x, int y)
        {
            return MazeArray[x, y] == EPointType.Empty;
        }

        public bool CanPass(Point start, int x, int y, EDirection direction)
        {
            //
            if (!CanPass(x, y) || CloseList.Exists(x, y, direction))
                return false;
            if (Math.Abs(x - start.X) + Math.Abs(y - start.Y) == 1)
                return true;
                
            //如果是斜方向移动, 判断是否 "拌脚"
            if (CanPass(Math.Abs(x - 1), y) && CanPass(x, Math.Abs(y - 1)))
                return true;
                
            return IsIgnoreCorner;
            
        }
        
        public static Point MinPoint(ref List<Point> points)
        {
            points = points.OrderBy(p => p.F).ToList();
            return points[0];
            
        }
        
        
    }

    //Point 类型
    public class Point
    {
        public Point ParentPoint { get; set; }
        public int F { get; set; }  //F=G+H
        public int G { get; set; }
        public int H { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public AStarSearch.EDirection Direction  { get; set; } = AStarSearch.EDirection.None;
        
        public int WheelCount { get; set; }

        public Point(Vector2Int point)
        {
            this.X = point.x;
            this.Y = point.y;
        }
        
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public void CalcF()
        {
            this.F = this.G + this.H;
        }
        
        public static Point operator -(Point v1, Point v2)
        {
            return new Point(v1.X - v2.X, v1.Y - v2.Y);
        }
    }

//对 List<Point> 的一些扩展方法
    public static class ListHelper
    {
        public static bool Exists(this List<Point> points, Point point)
        {
            foreach (Point p in points)
                if ((p.X == point.X) && (p.Y == point.Y) && p.Direction == point.Direction)
                    return true;
            return false;
        }

        public static bool Exists(this List<Point> points, int x, int y, AStarSearch.EDirection direction)
        {
            foreach (Point p in points)
                if ((p.X == x) && (p.Y == y) && p.Direction == direction)
                    return true;
            return false;
        }

        

        public static void Add(this List<Point> points, int x, int y)
        {
            Point point = new Point(x, y);
            points.Add(point);
        }

        public static Point Get(this List<Point> points, Point point)
        {
            foreach (Point p in points)
                if ((p.X == point.X) && (p.Y == point.Y))
                    return p;
            return null;
        }
        
        public static Point Get(this List<Point> points, int x, int y)
        {
            foreach (Point p in points)
                if ((p.X == x) && (p.Y == y))
                    return p;
            return null;
        }

        public static void Remove(this List<Point> points, int x, int y)
        {
            foreach (Point point in points)
            {
                if (point.X == x && point.Y == y)
                    points.Remove(point);
            }
        }

    }
}
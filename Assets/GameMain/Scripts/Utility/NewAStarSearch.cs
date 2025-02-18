// using System;
// using System.Collections.Generic;
//
// public enum Direction
// {
//     Up,
//     Down,
//     Left,
//     Right,
//     None
// }
//
// public class Node
// {
//     public int X { get; set; }
//     public int Y { get; set; }
//     public Direction FromDirection { get; set; }
//     public int G { get; set; }
//     public int H { get; set; }
//     public int Turns { get; set; }
//     public Node Parent { get; set; }
//     public int F => G + H;
// }
//
// public struct NodePriority : IComparable<NodePriority>
// {
//     public int F { get; set; }
//     public int Turns { get; set; }
//
//     public int CompareTo(NodePriority other)
//     {
//         int compare = F.CompareTo(other.F);
//         return compare != 0 ? compare : Turns.CompareTo(other.Turns);
//     }
// }
//
// public class PriorityQueue<T> where T : Node
// {
//     private T[] items;
//     private Comparison<T> comparison;
//     private int count;
//     public int Count { get => count; }
//     public int Capacity { get => items == null ? 0 : items.Length; }
//
//     public PriorityQueue()
//     {
//         items = new T[10];
//         comparison = (x, y) => x.GetHashCode().CompareTo(y.GetHashCode());
//     }
//
//     public PriorityQueue(Comparison<T> comparison) : this()
//     {
//         this.comparison = comparison;
//     }
//
//     public void Enqueue(T item)
//     {
//         if (count >= Capacity)
//             Expansion();
//         items[count] = item;
//
//         int cur = count++;
//         if (cur == 0)
//             return;
//         int parent = cur;
//         T oldValue;
//         T newValue;
//         do
//         {
//             cur = parent;
//             parent = (cur - 1) / 2;
//             oldValue = items[parent];
//             Heapify(parent);
//             newValue = items[parent];
//         } while (!oldValue.Equals(newValue));
//     }
//
//     public T Dequeue()
//     {
//         if (count == 0)
//             throw new Exception("the queue is empty");
//
//         T result = items[0];
//         items[0] = default(T);
//
//         Swap(items, 0, count - 1);
//         count--;
//         if (count > 0)
//             Heapify(0);
//
//         return result;
//     }
//
//     public T Peek()
//     {
//         if (count == 0)
//             throw new Exception("the queue is empty");
//         return items[0];
//     }
//
//     private void Heapify(int node)
//     {
//         int lc = 2 * node + 1;
//         int rc = 2 * node + 2;
//         int min = node;
//         if (lc < count && comparison(items[min], items[lc]) > 0)
//             min = lc;
//         if (rc < count && comparison(items[min], items[rc]) > 0)
//             min = rc;
//         if (min != node)
//         {
//             Swap(items, node, min);
//             Heapify(min);
//         }
//     }
//
//     private void Swap(T[] array, int i, int j)
//     {
//         T t = array[i];
//         array[i] = array[j];
//         array[j] = t;
//     }
//
//     private void Expansion()
//     {
//         T[] newItems = new T[Capacity * 2];
//         for (int i = 0; i < count; i++)
//             newItems[i] = items[i];
//         items = newItems;
//     }
// }
//
// public class AStarWithTurnPenalty
// {
//     private readonly int[,] grid;
//     private readonly int width;
//     private readonly int height;
//
//     public AStarWithTurnPenalty(int[,] grid)
//     {
//         this.grid = grid;
//         width = grid.GetLength(0);
//         height = grid.GetLength(1);
//     }
//
//     public List<(int X, int Y)> FindPath((int X, int Y) start, (int X, int Y) end)
//     {
//         var open = new PriorityQueue<Node, NodePriority>();
//         var startNode = new Node
//         {
//             X = start.X,
//             Y = start.Y,
//             FromDirection = Direction.None,
//             G = 0,
//             H = CalculateH(start.X, start.Y, end.X, end.Y),
//             Turns = 0,
//             Parent = null
//         };
//         open.Enqueue(startNode, new NodePriority { F = startNode.F, Turns = startNode.Turns });
//
//         var visited = new Dictionary<(int X, int Y, Direction Dir), (int G, int Turns)>();
//         visited.Add((start.X, start.Y, Direction.None), (0, 0));
//
//         while (open.Count > 0)
//         {
//             var current = open.Dequeue();
//
//             if (current.X == end.X && current.Y == end.Y)
//                 return ReconstructPath(current);
//
//             foreach (Direction moveDir in new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right })
//             {
//                 int newX = current.X + GetDx(moveDir);
//                 int newY = current.Y + GetDy(moveDir);
//
//                 if (newX < 0 || newX >= width || newY < 0 || newY >= height)
//                     continue;
//
//                 if (grid[newX, newY] == 1)
//                     continue;
//
//                 int newTurns = current.FromDirection != Direction.None && moveDir != current.FromDirection 
//                     ? current.Turns + 1 
//                     : current.Turns;
//
//                 int newG = current.G + 1;
//                 int newH = CalculateH(newX, newY, end.X, end.Y);
//
//                 var newNode = new Node
//                 {
//                     X = newX,
//                     Y = newY,
//                     FromDirection = moveDir,
//                     G = newG,
//                     H = newH,
//                     Turns = newTurns,
//                     Parent = current
//                 };
//
//                 var key = (newX, newY, moveDir);
//                 if (visited.TryGetValue(key, out var existing))
//                 {
//                     if (newG < existing.G || (newG == existing.G && newTurns < existing.Turns))
//                     {
//                         visited[key] = (newG, newTurns);
//                         open.Enqueue(newNode, new NodePriority { F = newNode.F, Turns = newNode.Turns });
//                     }
//                 }
//                 else
//                 {
//                     visited.Add(key, (newG, newTurns));
//                     open.Enqueue(newNode, new NodePriority { F = newNode.F, Turns = newNode.Turns });
//                 }
//             }
//         }
//         return null;
//     }
//
//     private int CalculateH(int x, int y, int endX, int endY)
//     {
//         return Math.Abs(x - endX) + Math.Abs(y - endY);
//     }
//
//     private int GetDx(Direction dir)
//     {
//         return dir == Direction.Left ? -1 : dir == Direction.Right ? 1 : 0;
//     }
//
//     private int GetDy(Direction dir)
//     {
//         return dir == Direction.Up ? -1 : dir == Direction.Down ? 1 : 0;
//     }
//
//     private List<(int X, int Y)> ReconstructPath(Node node)
//     {
//         var path = new List<(int X, int Y)>();
//         while (node != null)
//         {
//             path.Add((node.X, node.Y));
//             node = node.Parent;
//         }
//         path.Reverse();
//         return path;
//     }
// }
//
// // 使用示例
// class Program
// {
//     static void Main()
//     {
//         // 示例网格（0可通过，1为障碍）
//         int[,] grid = new int[,]
//         {
//             {0, 0, 0, 0, 0},
//             {0, 1, 1, 1, 0},
//             {0, 1, 0, 0, 0},
//             {0, 0, 0, 1, 0},
//             {0, 1, 0, 0, 0}
//         };
//
//         var astar = new AStarWithTurnPenalty(grid);
//         var path = astar.FindPath((0, 0), (4, 4));
//
//         if (path != null)
//         {
//             Console.WriteLine("找到路径:");
//             foreach (var point in path)
//                 Console.WriteLine($"({point.X}, {point.Y})");
//         }
//         else
//         {
//             Console.WriteLine("未找到路径");
//         }
//     }
// }
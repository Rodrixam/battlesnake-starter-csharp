using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core;
using System.Linq;

namespace Starter.Api.Controllers
{
    class Node
    {
        public Node(Point point, int f, Node parent)
        {
            Cords = point;
            F = f;
            Parent = parent;
        }

        public Point Cords;
        public int F;
        public Node Parent;
    }

    [ApiController]
    public class SnakeController : ControllerBase
    {
        /// <summary>
        /// This request will be made periodically to retrieve information about your Battlesnake,
        /// including its display options, author, etc.
        /// </summary>
        [HttpGet("")]
        public IActionResult Index()
        {

            var response = new InitResponse
            {
                ApiVersion = "1",
                Author = "S. Espinoza, R. Sepulveda",
                Color = "#4C89C8",
                Head = "fang",
                Tail = "bolt"
            };

            return Ok(response);
        }


        /// <summary>
        /// Your Battlesnake will receive this request when it has been entered into a new game.
        /// Every game has a unique ID that can be used to allocate resources or data you may need.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("move")]
        public IActionResult Move(GameStatusRequest gameStatusRequest)
        {
            //Random rng = new Random();
            //A* por la comida
            Point head = gameStatusRequest.You.Body.ToList()[0];
            Node nextMoveNode = AStarFoodSetUp(gameStatusRequest, head);
            //Convertir a movimiento
            Point nextMove = nextMoveNode.Cords;
            string move = CalcMove(head, nextMove);

            var response = new MoveResponse
            {
                //Move = direction[rng.Next(direction.Count)]
                Move = move,
                Shout = "racial slur"
            };

            return Ok(response);

        }


        /// <summary>
        /// Your Battlesnake will receive this request whenever a game it was playing has ended.
        /// Use it to learn how your Battlesnake won or lost and deallocated any server-side resources.
        /// Your response to this request will be ignored.
        /// </summary>
        [HttpPost("end")]
        public IActionResult End(GameStatusRequest gameStatusRequest)
        {
            return Ok();
        }

        private Node AStarFoodSetUp(GameStatusRequest gameStatusRequest, Point head)
        {
            //Iniciar Listas
            List<Snake> snakes = gameStatusRequest.Board.Snakes.ToList();
            List<Point> foods = gameStatusRequest.Board.Food.ToList();

            //Crea matriz de obstáculos
            int height = gameStatusRequest.Board.Height;
            int width = gameStatusRequest.Board.Width;
            bool[,] obstacles = new bool[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    obstacles[i,j] = false;
                }
            }

            //Llenar matriz
            foreach (Snake snake in snakes)
            {
                List<Point> body = snake.Body.ToList();
                foreach (Point point in body)
                {
                    obstacles[height - point.Y,point.X] = true;
                }
            }

            //Iniciar el A* para cada comida en el tablero
            int lenght = 10000;
            List<Node> truePath = null;
            List<Node> path;
            foreach (Point food in foods)
            {
                path = AStar(head, food, obstacles, height);

                //Descartar caminos imposibles
                if (path == null) continue;

                //Buscar la comida más cercana
                if (path.Count < lenght)
                {
                    lenght = path.Count;
                    truePath = path;
                }
            }

            return truePath[0];
        }

        private List<Node> AStar(Point begin, Point end, bool[,] obstacles, int height)
        {
            List<Node> Open = new List<Node>();
            List<Point> Closed = new List<Point>();
            List<Node> Path = new List<Node>();
            int g = 0;
            int h = DistEst(begin, end);
            int f = g + h;
            Open.Add(new Node(begin, f, null));
            g++;
            while (Open.Count > 0)
            {
                Node node = Open[0];
                Open.RemoveAt(0);
                if (node.Cords == end)
                {
                    //Success
                    Path.Insert(Path.Count, node);
                    return Path;
                }
                List<Point> children = GetChildren(node.Cords);
                foreach (Point child in children)
                {
                    if (Closed.Exists(x => x.X == child.X && x.Y == child.Y))
                    {
                        continue;
                    }

                    if (!obstacles[height - child.Y, child.X])
                    {
                        h = DistEst(child, end);
                        f = g + h;
                        if (Open.Count == 0)
                        {
                            Open.Add(new Node(child, f, node));
                        }
                        else
                        {
                            for (int i = 0; i < Open.Count; i++)
                            {
                                if (Open[i].F > f)
                                {
                                    Open.Insert(i, new Node(child, f, node));
                                }
                            }
                        }
                    }
                }
                Closed.Add(node.Cords);
            }

            //failure
            return null;
        }

        private int DistEst(Point begin, Point end)
        {
            int x = begin.X - end.X;
            x = Math.Abs(x);
            int y = begin.Y - end.Y;
            y = Math.Abs(y);
            return x + y;
        }

        private List<Point> GetChildren(Point point)
        {
            List<Point> children = new List<Point>();
            children.Add(new Point(point.X + 1, point.Y));
            children.Add(new Point(point.X - 1, point.Y));
            children.Add(new Point(point.X, point.Y + 1));
            children.Add(new Point(point.X, point.Y - 1));
            return children;
        }

        private string CalcMove(Point begin, Point end)
        {
            string move = null;
            if (begin.Y < end.Y)
            {
                move = "up";
            }
            else if (begin.Y > end.Y)
            {
                move = "down";
            }
            else if (begin.X < end.X)
            {
                move = "right";
            }
            else if (begin.X > end.X)
            {
                move = "left";
            }
            return move;
        }
    }
}
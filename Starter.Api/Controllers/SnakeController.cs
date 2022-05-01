using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core;
using System.Linq;

namespace Starter.Api.Controllers
{


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
            Random rng = new Random();
            List<string> direction = GetDirections(gameStatusRequest);


            System.Diagnostics.Trace.TraceInformation("Moviendo left");
            var response = new MoveResponse
            {
                Move = "left",
                //Move = direction[rng.Next(direction.Count)],
                Shout = "I am moving!"
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


        private List<string> GetDirections(GameStatusRequest gameStatusRequest)
        {
            List<string> direction = new List<string>();

            Point head = gameStatusRequest.You.Head;
            Board board = gameStatusRequest.Board;
            //List<Point> body = gameStatusRequest.You.Body.ToList();
            List<Point> allBodies = new List<Point>();

            List<Snake> snakes = gameStatusRequest.Board.Snakes.ToList();
            foreach (Snake snake in snakes)
            {
                List<Point> body = snake.Body.ToList();
                allBodies.AddRange(body);
            }

            if (head.Y < board.Height - 1)
            {
                bool acceptable = true;
                foreach (Point bodyPart in allBodies)
                {
                    if (bodyPart.Y == head.Y + 1 && bodyPart.X == head.X)
                    {
                        acceptable = false;
                        break;
                    }
                }
                if (acceptable) { direction.Add("up"); }
            }

            if (head.X > 0)
            {
                bool acceptable = true;
                foreach (Point bodyPart in allBodies)
                {
                    if (bodyPart.X == head.X - 1 && bodyPart.Y == head.Y)
                    {
                        acceptable = false;
                        break;
                    }
                }
                if (acceptable) { direction.Add("left"); }
            }

            if (head.X < board.Width - 1)
            {
                bool acceptable = true;
                foreach (Point bodyPart in allBodies)
                {
                    if (bodyPart.X == head.X + 1 && bodyPart.Y == head.Y)
                    {
                        acceptable = false;
                        break;
                    }
                }
                if (acceptable) { direction.Add("right"); }
            }

            if (head.Y > 0)
            {
                bool acceptable = true;
                foreach (Point bodyPart in allBodies)
                {
                    if (bodyPart.Y == head.Y - 1 && bodyPart.X == head.X)
                    {
                        acceptable = false;
                        break;
                    }
                }
                if (acceptable) { direction.Add("down"); }
            }

            return direction;
        }
    }
}
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
        [HttpPost("start")]
        public IActionResult Start(GameStatusRequest gameStatusRequest)
        {
            return Ok();
        }


        /// <summary>
        /// This request will be sent for every turn of the game.
        /// Use the information provided to determine how your
        /// Battlesnake will move on that turn, either up, down, left, or right.
        /// </summary>
        [HttpPost("move")]
        public IActionResult Move(GameStatusRequest gameStatusRequest)
        {
            Random rng = new Random();
            List<string> direction = new List<string>();


            Point head = gameStatusRequest.You.Head;
            Board board = gameStatusRequest.Board;
            List<Point> body = gameStatusRequest.You.Body.ToList();

            
            if (head.Y < board.Height - 1 && !(body[1].Y > head.Y)) { direction.Add("up"); }
            if (head.X > 0 && !(body[1].X < head.X)) { direction.Add("left"); }
            if (head.X < board.Width - 1 && !(body[1].X > head.X)) { direction.Add("right"); }
            if (head.Y > 0 && !(body[1].Y < head.Y)) { direction.Add("down"); }

            var response = new MoveResponse
            {
                Move = direction[rng.Next(direction.Count)],
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
    }
}
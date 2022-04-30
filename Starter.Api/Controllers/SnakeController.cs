using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Core;

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
            var rng = new Random();
            var direction = new List<string>();

            Point head = gameStatusRequest.You.Head;
            List<Point> entireBody = (List<Point>)gameStatusRequest.You.Body;
            Point body = entireBody[1];
            Board board = gameStatusRequest.Board;

            if (body.Y < head.Y && head.Y < board.Height) { direction.Add("up"); }
            if (body.X < head.X && head.X > 0) { direction.Add("left"); }
            if (body.X > head.X && head.X < board.Width) { direction.Add("right"); }
            if (body.Y > head.Y && head.Y > 0) { direction.Add("down"); }//*/

            foreach(string dir in direction)
            {
                Console.WriteLine(dir);
            }

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
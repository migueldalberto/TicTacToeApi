using Microsoft.AspNetCore.Mvc;
using TicTacToeApi.TicTacToe;

namespace TicTacToeApi.Controllers;

[ApiController]
[Route("/")]
public class TicTacToeController : ControllerBase
{
    private readonly ILogger<TicTacToeController> _logger;

    public TicTacToeController(ILogger<TicTacToeController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "board")]
    public IActionResult Get([FromQuery] string board="         ")
    {
        Game game; 
        if (!TicTacToeBoard.IsBoardValid(board))
            return BadRequest("Invalid board");

        game = new Game(board, 'o');

        if (!game.PlayersTurn())
            return BadRequest("Not possibly O's turn");

        if (game.IsGameOver())
            return BadRequest("Game is over");

        return Ok(game.OptimalPlay());
    }

}

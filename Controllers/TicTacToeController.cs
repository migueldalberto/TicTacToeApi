using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Get([FromQuery] string board)
    {
        TicTacToeGame game; 
        if (!TicTacToeGame.IsValidBoard(board))
            return BadRequest();

        game = new TicTacToeGame(board, 'o');

        if (!game.IsOsTurn())
            return BadRequest();

        if (game.IsGameOver())
            return BadRequest();

        return Ok(game.OptimalPlay());
    }

}

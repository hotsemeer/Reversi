using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Reversi_api.Data;
using Reversi_api.Models;
using Reversi_api.Resources;

namespace Reversi_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ReversiContext _context;

        public GamesController(ReversiContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
          if (_context.Games == null)
          {
              return NotFound();
          }
            return await _context.Games.ToListAsync();
        }

        [Authorize]
        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameResource>> GetGame(int id)
        {
          if (_context.Games == null)
          {
              return NotFound();
          }
            var game = await _context.Games
                .Include(g => g.PlayerBlack)
                .Include(g => g.PlayerWhite)
                .Include(g => g.Columns)
                .ThenInclude(column => column.Rows)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var Gamestatus = FormatGameStatus(game);
            var gameResource = new GameResource(game.Id, game.PlayerWhite, game.PlayerBlack, game.Description, Gamestatus);

            return gameResource;
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<GameResource>> PostGame(Game game)
        {
            if (_context.Games == null) {
                return Problem("Entity set 'ReversiContext.Games' is null.");
            }

            int userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var user = _context.Player.Find(userId);

            Random random = new Random();
            if (random.Next(2) == 1)
            {
                game.PlayerBlack = user;
            }
            else
            {
                game.PlayerWhite = user;
            }

            game.Columns = new List<Column>(9);

            for (int i = 0; i < 9; i++)
            {
                Column column = new Column();
                game.Columns.Add(column);

                column.Rows = new List<Row>(9);

                for (int j = 0; j < 9; j++)
                {
                    Row row = new Row();
                    column.Rows.Add(row);
                }
            }

            game.Columns[3].Rows[3].Value = 1;
            game.Columns[4].Rows[4].Value = 1;
            game.Columns[3].Rows[4].Value = 2;
            game.Columns[4].Rows[3].Value = 2;


            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var Gamestatus = FormatGameStatus(game);
            var gameResource = new GameResource(game.Id, game.PlayerWhite, game.PlayerBlack, game.Description, Gamestatus);
            //var gameResource = new GameResource(game.Id, game.PlayerWhiteId, game.PlayerBlackId, game.Description, Gamestatus.ToJson());

            return CreatedAtAction("GetGame", new { id = game.Id }, gameResource);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            if (_context.Games == null)
            {
                return NotFound();
            }
            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private List<List<int>> FormatGameStatus(Game game)
        {
            //int[,] numbers = new int[9, 9];
            List<List<int>> numbers = new List<List<int>>();

            for (int i = 0; i < 9; i++)
            {
                numbers.Add(new List<int>());
                for (int j = 0; j < 9; j++)
                {
                    numbers[i].Add(game.Columns[i].Rows[j].Value);
                }
            }

            return numbers;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskConsumerAPI.Models;

namespace TaskConsumerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterTokensController : ControllerBase
    {
        private readonly RegisterTokenContext _context;

        public RegisterTokensController(RegisterTokenContext context)
        {
            _context = context;
        }

        // GET: api/RegisterTokens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegisterToken>>> GetRegisterTokens()
        {
            return await _context.RegisterTokens.ToListAsync();
        }

        // GET: api/RegisterTokens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisterToken>> GetRegisterToken(int id)
        {
            var registerToken = await _context.RegisterTokens.FindAsync(id);

            if (registerToken == null)
            {
                return NotFound();
            }

            return registerToken;
        }

        // PUT: api/RegisterTokens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegisterToken(int id, RegisterToken registerToken)
        {
            if (id != registerToken.Id)
            {
                return BadRequest();
            }

            _context.Entry(registerToken).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisterTokenExists(id))
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

        // POST: api/RegisterTokens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegisterToken>> PostRegisterToken(RegisterToken registerToken)
        {
            _context.RegisterTokens.Add(registerToken);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegisterToken", new { id = registerToken.Id }, registerToken);
        }

        // DELETE: api/RegisterTokens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegisterToken(int id)
        {
            var registerToken = await _context.RegisterTokens.FindAsync(id);
            if (registerToken == null)
            {
                return NotFound();
            }

            _context.RegisterTokens.Remove(registerToken);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegisterTokenExists(int id)
        {
            return _context.RegisterTokens.Any(e => e.Id == id);
        }
    }
}

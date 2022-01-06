#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTestApp.Model;

namespace MyTestApp.Controllers
{
    [Route("api/BankAccounts")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly BankAccountContext _context;

        public BankAccountController(BankAccountContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(long id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            return bankAccount;
        }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(long id, BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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
        
        [HttpPatch("putmoney/{id}/{money}")]
        public async Task<IActionResult> PutMoney(long id, decimal money)
        {
            if (!BankAccountExists(id))
            {
                return BadRequest();
            }
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            bankAccount.Balance += money;

            _context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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

        [HttpPatch("withdraw/{id}/{money}")]
        public async Task<IActionResult> WithdrawMoney(long id, decimal money)
        {
            if (!BankAccountExists(id))
            {
                return BadRequest();
            }
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount.Balance < money) return Problem("There're isufficient funds to write off");
            
            bankAccount.Balance -= money;
            _context.Entry(bankAccount).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostBankAccount(BankAccount bankAccount)
        {
            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBankAccount),bankAccount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(long id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankAccountExists(long id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }
    }
}

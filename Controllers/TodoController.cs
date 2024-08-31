using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var query = "SELECT * FROM TodoItems";
            var todoItems = await _context.TodoItems.FromSqlRaw(query).ToListAsync();

            return Ok(todoItems);
        }

        // GET: api/TodoItems/1
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
        {
            // Use parameterized query to prevent SQL injection
            var query = "SELECT * FROM TodoItems WHERE Id = {0}";
            var todoItem = await _context.TodoItems
                                        .FromSqlRaw(query, id)
                                        .FirstOrDefaultAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            // Check parameters
            if (string.IsNullOrEmpty(todoItem.Name))
            {
            return BadRequest();
            }

            var query = "INSERT INTO TodoItems (Id, Name, IsComplete) VALUES ({0}, {1}, {2})";
            _context.Database.ExecuteSqlRaw(query, todoItem.Id, todoItem.Name, todoItem.IsComplete);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, new { status_code = 201, message = "Created successfully", data = todoItem });
        }

        // PUT: api/TodoItems/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem updatedItem)
        {
            if (id != updatedItem.Id || string.IsNullOrEmpty(updatedItem.Name))
            {
                return BadRequest(new { message = "Invalid item ID or missing item name." });
            }

            if (!await TodoItemExists(id))
            {
                return NotFound(new { message = "Todo item not found." });
            }

            // Use parameterized SQL query for update
            var query = "UPDATE TodoItems SET Name = {0}, IsComplete = {1} WHERE Id = {2}";
            await _context.Database.ExecuteSqlRawAsync(query, updatedItem.Name, updatedItem.IsComplete, id);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues
                return Conflict(new { message = "A concurrency error occurred while updating the item.", details = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                return StatusCode(500, new { message = "An error occurred while updating the item.", details = ex.Message });
            }

            return Ok(new { status_code = 201, message = "Updated successfully", data = updatedItem });
        }

        // DELETE: api/TodoItems/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var deleteQuery = "DELETE FROM TodoItems WHERE Id = {0}";
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(deleteQuery, id);

            if (rowsAffected > 0)
            {
                // If the deletion was successful, return a custom response.
                return Ok(new { status_code = 204, message = "Deleted successfully" });
            }
            else
            {
                // If no rows were affected, return a not found response.
                return NotFound(new { status_code = 404, message = "Item not found" });
            }
        }

        private async Task<bool> TodoItemExists(int id)
        {
            var query = "SELECT COUNT(*) FROM TodoItems WHERE Id = {0}";
            var count = await _context.TodoItems.FromSqlRaw(query, id).CountAsync();

            return count > 0;
        }
    }
}
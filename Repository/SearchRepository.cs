using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using TaskStatus = ToDoList.Models.TaskStatus;

namespace ToDoList.Repository
{
    public class SearchRepository
    {
        #region Fields

        private readonly ToDoListContext _context;

        #endregion

        #region Constructor
        public SearchRepository(ToDoListContext context)
        {
            _context = context;
        }

        #endregion

        #region Get Todos By Status
        public async Task<List<ToDoTask>> GetTodosByStatus(bool completed)
        {
            var result = await _context.Tasks.Where(t => t.Status == (completed ? TaskStatus.Completed : TaskStatus.InCompleted)).ToListAsync();
            return result;
        }
        #endregion

        #region Get Todos By DueDate
        public async Task<List<ToDoTask>> GetTodosByDueDateAsync(DateTime due_date)
        {
            var result = await _context.Tasks.Where(t => t.DueDate == due_date).ToListAsync();
            return result;
        }
        #endregion

        #region Get Todos By Priority
        public async Task<List<ToDoTask>> GetTodosByPriority(TaskPriority taskPriority)
        {
            var result = await _context.Tasks.Where(t => t.Priority == taskPriority).ToListAsync();
            return result;
        }
        #endregion

    }
}

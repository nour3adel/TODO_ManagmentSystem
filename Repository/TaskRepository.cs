using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Repository
{
    public class TaskRepository
    {
        ToDoListContext _context;
        public TaskRepository(ToDoListContext context )
        {
            _context = context;
        }
        public async Task<List<ToDoTask>> GetAll()
        {
            return await _context.Tasks.ToListAsync();
        }
        public async Task<ToDoTask> Get(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }
        public async Task Add(ToDoTask _task)
        {
            await _context.Tasks.AddAsync(_task);
        }
        public void Update(ToDoTask _task)
        {
             _context.Entry(_task).State = EntityState.Modified;

        }
        public async Task Delete(int id)
        {
            ToDoTask _task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(_task);
        }
        public async Task Save()
        {

            await _context.SaveChangesAsync();
        }
    }
}

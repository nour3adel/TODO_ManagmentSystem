using ToDoList.Models;
using ToDoList.Repository;

namespace ToDoList.UnitOfWorks
{
    public class UnitOfWork
    {
        ToDoListContext _context;
        SearchRepository searchRepository;
        TaskRepository taskRepository;

        public UnitOfWork(ToDoListContext context)
        {
            _context = context;
        }
        public SearchRepository SearchRepository { 
            get 
            {
                if (searchRepository == null)
                    searchRepository = new SearchRepository(_context);
                return searchRepository; 
            } 
        }   
        public TaskRepository TaskRepository
        {
            get
            {
                if(taskRepository == null)
                    taskRepository = new TaskRepository(_context);
                return taskRepository;
            }
        }
        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

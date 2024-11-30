using Microsoft.EntityFrameworkCore;

namespace ToDoList.Models
{
    public class ToDoListContext:DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options):base(options) 
        {
            
        }
        public virtual DbSet<ToDoTask> Tasks { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }
        [Required , StringLength(50)]

        public string Title { get; set; }
        [Required , StringLength(250)]
        public string Description { get; set; }
        [Column(TypeName = "date")]

        public DateTime DueDate { get; set; }
        [Required]
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        [Column(TypeName = "date")]

        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ToDoList.DTOs;
using ToDoList.Models;
using ToDoList.UnitOfWorks;
using TaskStatus = ToDoList.Models.TaskStatus;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TODOsController : ControllerBase
    {
        #region fields

        private readonly UnitOfWork _unit;
        IMapper _mapper;

        #endregion

        #region Constrcutor
        public TODOsController(UnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }
        #endregion

        #region Get all Todos

        [HttpGet("Todos")]
        [SwaggerOperation(summary: "Display all Todos", description: "Get All Todos From Database")]
        [SwaggerResponse(200, "Successfully retrieved Todos", typeof(List<TodoDTO>))]
        [SwaggerResponse(404, "No Todos found")]

        public async Task<IActionResult> GetAllTodos()
        {
            var todos = await _unit.TaskRepository.GetAll();
            if (!todos.Any())
            {
                return NotFound();
            }
            var todosDTO = _mapper.Map<List<TodoDTO>>(todos);
            return Ok(todosDTO);
        }

        #endregion

        #region Get Todo By ID

        [HttpGet("Todos/{id}")]
        [SwaggerOperation(Summary = "Get todo by ID", Description = "Fetches a task using the task ID.")]
        [SwaggerResponse(200, "Successfully retrieved task", typeof(TodoDTO))]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> GetTodoByID(int id)
        {
            var todo = await _unit.TaskRepository.Get(id);
            if (todo == null)
            {
                return NotFound($"No Todos With This ID ={id} is Found");
            }
            var todoDTO = _mapper.Map<ToDoTask>(todo);
            return Ok(todoDTO);
        }

        #endregion


        #region Get tasks by their status

        [HttpGet("tasks/{completed:bool}")]
        [SwaggerOperation(Summary = "Retrieve tasks by their completion status",
                 Description = "Fetches tasks based on whether they are completed or not. The `completed` parameter indicates whether to retrieve completed or incomplete tasks.")]
        [SwaggerResponse(200, "Successfully retrieved tasks", typeof(List<TodoDTO>))]
        [SwaggerResponse(404, "No tasks found for the given completion status")]
        public async Task<IActionResult> GetAllCompletedTasks(bool completed)
        {
            var result = await _unit.SearchRepository.GetTodosByStatus(completed);
            if (!result.Any()) return NotFound("No Completed Tasks");
            var resultDTO = _mapper.Map<List<TodoDTO>>(result);
            return Ok(resultDTO);
        }
        #endregion

        #region Get Todo By DueDate
        [HttpGet("TodoByDueDate")]
        [SwaggerOperation(Summary = "Get Todos by their DueDate",
                 Description = "<h2>Get Todos by their DueDate</h2>")]
        [SwaggerResponse(200, "Successfully retrieved todos", typeof(List<TodoDTO>))]
        [SwaggerResponse(404, "No todo found for the given due date")]
        public async Task<IActionResult> GetTasksByDueDate(DateTime due_date)
        {
            List<ToDoTask> tasks = await _unit.SearchRepository.GetTodosByDueDateAsync(due_date);
            if (!tasks.Any()) return NotFound();
            var tasksDTO = _mapper.Map<List<TodoDTO>>(tasks);
            return Ok(tasksDTO);
        }
        #endregion

        #region  Get tasks by Priority
        [HttpGet("tasksByPriority")]
        [SwaggerOperation(Summary = "Retrieve tasks by priority",
                  Description = "Fetches tasks based on their priority level. The `taskPriority` parameter indicates the level of priority to filter tasks.")]
        [SwaggerResponse(200, "Successfully retrieved tasks by priority", typeof(List<TodoDTO>))]
        [SwaggerResponse(404, "No tasks found with the specified priority")]
        public async Task<IActionResult> GetTasksByPriority(TaskPriority taskPriority)
        {
            List<ToDoTask> tasks = await _unit.SearchRepository.GetTodosByPriority(taskPriority);
            if (!tasks.Any()) return NotFound();
            var tasksDTO = _mapper.Map<List<TodoDTO>>(tasks);
            return Ok(tasksDTO);
        }
        #endregion

        #region Add New TODO

        [HttpPost("AddTodo")]
        [SwaggerOperation(Summary = "Add a new Todo", Description = "Creates a New Todo and add it to database.")]
        [SwaggerResponse(201, "Todo successfully created", typeof(TodoDTO))]
        [SwaggerResponse(400, "Invalid todo data")]
        public async Task<IActionResult> CreateTodo(TodoDTO todoDTO)
        {
            if (todoDTO == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = _mapper.Map<ToDoTask>(todoDTO);
            await _unit.TaskRepository.Add(todo);
            await _unit.Save();
            return CreatedAtAction("GetTodoByID", new { id = todo.Id }, todoDTO);
        }
        #endregion

        #region  Mark todo as completed

        [HttpPut("{id}/Complete")]
        [SwaggerOperation(Summary = "Mark todo as completed", Description = "Marks a todo as completed and update it in database.")]
        [SwaggerResponse(200, "Successfully marked the todo as completed", typeof(TodoDTO))]
        [SwaggerResponse(400, "Invalid todo ID")]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> MarkTaskAsCompleted(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid task ID.");
            }
            ToDoTask task = await _unit.TaskRepository.Get(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }
            task.Status = TaskStatus.Completed;
            _unit.TaskRepository.Update(task);
            await _unit.Save();
            var updateTaskDTO = _mapper.Map<TodoDTO>(task);
            return Ok(updateTaskDTO);
        }
        #endregion


        #region  Mark todo as incompleted

        [SwaggerOperation(Summary = "Mark task as incomplete", Description = "Marks a task as incomplete using the specified task ID.")]
        [SwaggerResponse(200, "Successfully marked the task as incomplete", typeof(TodoDTO))]
        [SwaggerResponse(400, "Invalid task ID")]
        [SwaggerResponse(404, "Task not found")]
        [HttpPut("{id}/InComplete")]
        public async Task<IActionResult> MarkTaskAsInCompleted(int id)
        {
            if (id <= 0) return BadRequest("Invalid task ID.");
            ToDoTask task = await _unit.TaskRepository.Get(id);
            if (task == null) return NotFound($"Task with ID {id} not found.");
            task.Status = TaskStatus.InCompleted;
            _unit.TaskRepository.Update(task);
            await _unit.Save();
            var updateTaskDTO = _mapper.Map<TodoDTO>(task);
            return Ok(updateTaskDTO);
        }

        #endregion

        #region Edit Todo Details
        [HttpPut("EditTodo/{id}")]
        [SwaggerOperation(Summary = "Edit Todo Details", Description = "Updates an existing task with the new details.")]
        [SwaggerResponse(200, "Todo successfully updated", typeof(TodoDTO))]
        [SwaggerResponse(400, "Invalid todo ID or todo data")]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> EditTodo(int id, TodoDTO todoDTO)
        {
            if (id <= 0) return BadRequest("Invalid todo ID. ");
            ToDoTask _task = await _unit.TaskRepository.Get(id);
            if (_task == null)
            {
                return NotFound($"Todo with ID {id} not found.");
            }
            _mapper.Map(todoDTO, _task);
            _unit.TaskRepository.Update(_task);
            await _unit.Save();
            var updateTodoDTO = _mapper.Map<TodoDTO>(_task);
            return Ok(updateTodoDTO);
        }
        #endregion

        #region Edit todo priority

        [HttpPut("{id}/Priority")]
        [SwaggerOperation(Summary = "Update task priority", Description = "Updates the priority of the task with the specified ID.")]
        [SwaggerResponse(200, "Successfully updated the task priority", typeof(TodoDTO))]
        [SwaggerResponse(400, "Invalid task ID or priority")]
        [SwaggerResponse(404, "Task not found")]
        public async Task<IActionResult> UpdateTaskPriority(int id, [FromQuery] TaskPriority priority)
        {
            if (id <= 0) return BadRequest("Invalid task ID.");
            ToDoTask task = await _unit.TaskRepository.Get(id);
            if (task == null) return NotFound($"Task with ID {id} not found.");
            task.Priority = priority;
            _unit.TaskRepository.Update(task);
            await _unit.Save();
            var updateTaskDTO = _mapper.Map<TodoDTO>(task);
            return Ok(updateTaskDTO);
        }

        #endregion

        #region Delete Todo

        [HttpDelete("DeleteTodo/{id}")]
        [SwaggerOperation(Summary = "Delete a todo", Description = "Deletes a todo using the todo ID.")]
        [SwaggerResponse(200, "Todo successfully deleted")]
        [SwaggerResponse(400, "Invalid todo ID")]
        [SwaggerResponse(404, "Todo not found")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            if (id <= 0) return BadRequest("Invalid task ID.");
            ToDoTask task = await _unit.TaskRepository.Get(id);
            if (task == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }
            await _unit.TaskRepository.Delete(id);
            await _unit.Save();
            List<ToDoTask> tasks = await _unit.TaskRepository.GetAll();
            var tasksDTO = _mapper.Map<List<TodoDTO>>(tasks);
            return Ok(tasksDTO);
        }

        #endregion


    }
}

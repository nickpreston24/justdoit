using justdoit.Models;
using justdoit.Pages.Sandbox;

public interface ITodosRepository
{
    Task<List<Todo>> GetAll();
    Task<List<Todo>> Search(Todo search);
    Task<Todo> GetById(int id);
    Task<int> Create(params Todo[] model);
    Task Update(int id, Todo model);
    Task<int> Delete(int id);
    Task<int> GetRowCount();
    Task<List<string>> FindTables();
}
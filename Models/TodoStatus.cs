using CodeMechanic.Types;

namespace justdoit.Models;


//
// public class TodoStatus : Enumeration
// {
//     public static TodoStatus Done = new TodoStatus(1, nameof(Done));
//     public static TodoStatus Pending = new TodoStatus(2, nameof(Pending));
//     public static TodoStatus WIP = new TodoStatus(3, nameof(WIP));
//     public static TodoStatus Postponed = new TodoStatus(4, nameof(Postponed));
//     public static TodoStatus Unknown = new TodoStatus(5, nameof(Unknown));
//
//     public TodoStatus(int id, string name) : base(id, name)
//     {
//     }
//
//     public static implicit operator TodoStatus(string status)
//     {
//         if (status.IsEmpty())
//             return Unknown;
//         var
//             found = TodoStatus
//                 .GetAll<TodoStatus>()
//                 .SingleOrDefault(x => x.Name.Equals(status, StringComparison.CurrentCultureIgnoreCase));
//         return found;
//     }
// }

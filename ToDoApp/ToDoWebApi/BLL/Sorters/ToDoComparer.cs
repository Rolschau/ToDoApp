using ToDoWebApi.BLL.Models;

namespace ToDoWebApi.BLL.Sorters
{
    /// <summary>
    /// Sorts ToDo items.
    /// </summary>
    public class ToDoComparer : IComparer<ToDoDTO>
    {
        public int Compare(ToDoDTO? x, ToDoDTO? y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            if (x.Order < y.Order) return -1;
            if (x.Order > y.Order) return 1;
            return 0; //Equal
        }
    }
}
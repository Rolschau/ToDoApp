namespace ToDoWebApi.BLL.Sorters
{
    /// <summary>
    /// Sorts integer strings the same way as integers.
    /// This is VERY fast.
    /// </summary>
    public class DigitStringComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            //If either number has more digits that the other, its higher
            if (x.Length < y.Length) return -1;
            if (x.Length > y.Length) return 1;

            //Perfectly safe - if we got here x.Length == y.Length
            for (int i = 0; i < x.Length; i++)
            {
                int charX = x[i];
                int charY = y[i];
                if (charX < charY) return -1;
                if (charX > charY) return 1;
            }
            return 0; //Equal
        }
    }
}
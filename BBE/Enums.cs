namespace BBE
{
    public enum Floor
    {
        Floor1 = 1,
        Floor2 = 2,
        Floor3 = 3,
        Endless = 4,
        Challenge = 5,
        None = 0
    }
    public class ModConvertor
    {
        public static Floor ToFloor(string name)
        {
            if (name == "Main1")
            {
                return Floor.Floor1;
            }
            if (name == "Main2")
            {
                return Floor.Floor2;
            }
            if (name == "Main3")
            {
                return Floor.Floor3;
            }
            if (name == "Endless1")
            {
                return Floor.Endless;
            }
            return Floor.None;
        }
    }
}

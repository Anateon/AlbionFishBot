namespace AlbionFishBot
{
    public class Poplovok
    {
        public float X;
        public float Y;
        public int ID;
        public int status;

        public override string ToString()
        {
            return $"ID:{ID}\tX:{X}\tY:{Y}\tS:{status}";
        }
    }
}
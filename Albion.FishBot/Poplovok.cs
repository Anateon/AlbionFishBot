namespace Albion.FishBot
{
    public class Poplovok
    {
        public float X;
        public float Y;
        public int ID;
        public int status;
        public int ownerID;

        public override string ToString()
        {
            return $"Owner:{ownerID}\tID:{ID}\tX:{X}\tY:{Y}\tS:{status}";
        }
    }
}
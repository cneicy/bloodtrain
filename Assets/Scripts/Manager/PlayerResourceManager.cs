using Entity;
using Utils;

namespace Manager
{
    public class PlayerResourceManager : Singleton<PlayerResourceManager>
    {
        public float Fuel
        {
            get => FindFirstObjectByType<Train>().GetComponent<Train>().fuel;
            set => FindFirstObjectByType<Train>().GetComponent<Train>().fuel = value;
        }

        public int BioMass { get; set; }
        public int MetalPiece { get; set; }
    }
}
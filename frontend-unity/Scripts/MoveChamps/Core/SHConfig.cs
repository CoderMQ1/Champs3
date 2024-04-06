// 
// 2023/12/19

namespace SquareHero
{
    public class SHConfig
    {
        public string VersionAddress;
        


        private static SHConfig _instance;
        public SHConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Load();
                }

                return _instance;
            }
        }

        public static SHConfig Load()
        {
            return new SHConfig();
        }
    }
}
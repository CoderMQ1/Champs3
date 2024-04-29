namespace champs3.Hotfix
{
    public static class TokenHelper
    {
        public static float TokenAccuracy = 1000000f;
        public static float ParseToken(this long self)
        {
            return (self / TokenAccuracy);
        }
        
        public static float ParseToken(this int self)
        {
            return (self / TokenAccuracy);
        }
        
    }
}
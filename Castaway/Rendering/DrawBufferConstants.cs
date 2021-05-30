namespace Castaway.Rendering
{
    public static class DrawBufferConstants
    {
        public const int Size = 3 + 4 + 3 + 3;
        
        public const int PositionX = 0;
        public const int PositionY = 1;
        public const int PositionZ = 2;

        public const int ColorR = 3;
        public const int ColorG = 4;
        public const int ColorB = 5;
        public const int ColorA = 6;

        public const int NormalX = 7;
        public const int NormalY = 8;
        public const int NormalZ = 9;

        public const int TextureU = 10;
        public const int TextureV = 11;
        public const int TextureT = 12;
    }
}
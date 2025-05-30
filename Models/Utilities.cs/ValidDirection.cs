namespace PixelWallEApp.Commands
{
    public static class IsDirectionValid
    {
        public static bool DirectionValid(int dirValue)
        {
            return dirValue >= -1 && dirValue <= 1;
        }
    }
}
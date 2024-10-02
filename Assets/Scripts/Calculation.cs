public static class Calculation
{
    public static int GetIndexFromCoordinate(int xCoordinate, int yCoordinate, int width)
    {
        return yCoordinate * width + xCoordinate;
    }
    
    public static bool GetRibValue(StitchScript stitch, int xCoordinate, int knit, int purl)
    {
        return stitch.isKnit = xCoordinate % (purl + knit) > purl - 1;
    }
}
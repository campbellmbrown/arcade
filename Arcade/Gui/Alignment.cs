namespace Arcade.Gui;

[Flags]
public enum Alignment
{
    None = 0,
    Left = 1,
    Right = 2,
    Top = 4,
    Bottom = 8,
    HCenter = 16,
    VCenter = 32,
    HStretch = 64,
    VStretch = 128,

    Center = HCenter | VCenter,
    Stretch = HStretch | VStretch,
}

public static class AlignmentExtensions
{
    public static int FlagCount(this Alignment alignment)
    {
        int count = 0;
        while (alignment != 0)
        {
            // Remove the rightmost bit
            alignment &= alignment - 1;
            count++;
        }
        return count;
    }
}

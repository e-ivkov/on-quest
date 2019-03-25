public enum LevelElement
{
    None = 0,
    Ogre = 1,
    Harpy = 2,
    Beast = 3,
    Skeleton = 4,
    Restarter = 5,
}

public static class LevelElementExtensions
{
    public static int GetElementType(this LevelElement element)
    {
        switch (element)
        {
            case LevelElement.Ogre:
            case LevelElement.Skeleton:
                return 1;
            case LevelElement.Harpy:
            case LevelElement.Beast:
                return 2;
        }
        return 0;
    }
}

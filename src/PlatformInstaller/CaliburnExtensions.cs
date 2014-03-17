using Caliburn.Micro;

public static class CaliburnExtensions
{

    public static T ShowDialog<T>(this IWindowManager windowManager) where T : new()
    {
        var rootModel = new T();
        windowManager.ShowDialog(rootModel);
        return rootModel;
    }
}
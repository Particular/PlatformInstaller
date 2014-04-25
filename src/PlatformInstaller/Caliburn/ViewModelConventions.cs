using System;

public static class ViewModelConventions
{
    
    public static Type GetViewForModel(Type type)
    {
        var viewName = type.Name.Replace("Model", "");
        return Type.GetType(viewName, true);
    }

    public static bool IsInstanceViewOrModel(Type type)
    {
        if (type.Name == typeof(ShellViewModel).Name)
        {
            return false;
        }
        if (type.Name == typeof(ShellView).Name)
        {
            return false;
        }
        return type.IsView() || type.IsViewModel();
    }

    public static bool IsViewModel(this Type type)
    {
        return type.Name.EndsWith("ViewModel");
    }

    public static bool IsView(this Type type)
    {
        return type.Name.EndsWith("View");
    }
}
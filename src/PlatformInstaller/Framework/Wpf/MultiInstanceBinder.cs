using System;
using System.Collections.Generic;
using System.ComponentModel;

public static class MultiInstanceBinder
{
    public static void BindActionToPropChanged(this IEnumerable<INotifyPropertyChanged> packages, Action action, string property)
    {
        foreach (var notifyPropertyChanged in packages)
        {
            notifyPropertyChanged.PropertyChanged += (x, y) =>
            {
                if (y.PropertyName == property)
                {
                    action();
                }
            };
        }
    }
}
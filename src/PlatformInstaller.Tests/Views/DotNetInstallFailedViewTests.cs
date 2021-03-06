﻿using System.Threading;
using NUnit.Framework;

[TestFixture]
public class DotNetInstallFailedViewTests
{
    [Test]
    [Explicit]
    [Apartment(ApartmentState.STA)]
    public void Show()
    {
        var model = GetModel();
        ViewTester.ShowView(model);
    }


    [Test]
    [Apartment(ApartmentState.STA)]
    [Explicit]
    public void Screenshot()
    {
        var model = GetModel();
        ViewTester.ScreenCapture(model);
    }

    static DotNetInstallFailedViewModel GetModel()
    {
        return new DotNetInstallFailedViewModel();
    }
}
using System;
using Microsoft.Win32;
using NUnit.Framework;

[TestFixture]
public class NewUserDetecterTests
{
    [Test]
    public void Should_classify_existing_nservicebus_key_as_a_existing_user()
    {
        var subKey = Guid.NewGuid().ToString();
        try
        {
            using (var regRoot = Registry.CurrentUser.CreateSubKey(subKey))
            using (regRoot.CreateSubKey("NServiceBus"))
            {
                Assert.False(NewUserDetecter.CheckForSubKey(regRoot));
            }
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(subKey);
        }
    }

    [Test]
    public void Should_classify_existing_particular_key_as_a_existing_user()
    {
        var subKey = Guid.NewGuid().ToString();
        try
        {
            using (var regRoot = Registry.CurrentUser.CreateSubKey(subKey))
            using (regRoot.CreateSubKey("ParticularSoftware"))
            {
                Assert.False(NewUserDetecter.CheckForSubKey(regRoot));
            }
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(subKey);
        }
    }


    [Test]
    public void Should_classify_as_new_user_when_both_nsb_and_platform_key_is_missing()
    {
        var subKey = Guid.NewGuid().ToString();
        try
        {
            using (var regRoot = Registry.CurrentUser.CreateSubKey(subKey))
            {
                Assert.True(NewUserDetecter.CheckForSubKey(regRoot));
            }
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(subKey);
        }
    }
}
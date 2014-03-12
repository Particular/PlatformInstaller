namespace PlatformInstaller.Tests
{
    using System;
    using Microsoft.Win32;
    using NUnit.Framework;

    [TestFixture]
    public class NewUserDetecterTests
    {
        [Test]
        public void Should_classify_existing_nservicebus_key_as_a_existing_user()
        {
            CreateSubKey("NServiceBus");

            Assert.False(detecter.IsNewUser());
        }

        void CreateSubKey(string keyName)
        {
            using (regRoot.CreateSubKey(keyName))
            { }
        }

        [Test]
        public void Should_classify_existing_particular_key_as_a_existing_user()
        {
            CreateSubKey("ParticularSoftware");

            Assert.False(detecter.IsNewUser());
        }


        [Test]
        public void Should_classify_as_new_user_when_both_nsb_and_platform_key_is_missing()
        {
            Assert.True(detecter.IsNewUser());
        }


        [SetUp]
        public void SetUp()
        {
            subKey = Guid.NewGuid().ToString();
            regRoot = Registry.CurrentUser.CreateSubKey(subKey);

            detecter = new NewUserDetecter(regRoot);
        }



        [TearDown]
        public void TearDown()
        {
            regRoot.Close();
            regRoot.Dispose();
            Registry.CurrentUser.DeleteSubKeyTree(subKey);
        }


        NewUserDetecter detecter;
        RegistryKey regRoot;
        string subKey;
    }
}
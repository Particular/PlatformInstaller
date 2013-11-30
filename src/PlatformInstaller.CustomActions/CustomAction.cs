using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace PlatformInstaller.CustomActions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.Text;
    using Ionic.Zip;
    using Microsoft.Deployment.WindowsInstaller;

    public class CustomActions
    {

        [CustomAction]
        public static ActionResult DownloadNuget(Session session)
        {
            Log(session, "Begin custom action DownloadApplication");

            var nugetName = session.Get("NUGET_NAME");

            var targetDir = session.Get("TARGET_NUGET_DIR");


            var urlToDownload = string.Format("http://nuget.org/api/v2/package/{0}", nugetName);


            var tempDownloadPath = Path.GetTempFileName();


            DownloadUrl(urlToDownload, tempDownloadPath);


            using (var zipFile = ZipFile.Read(tempDownloadPath))
            {
                zipFile.ToList().ForEach(entry =>
                {
                    var fileName = entry.FileName;

                    if (fileName.EndsWith(".exe") || fileName.EndsWith(".dll"))
                    {
                        entry.FileName = fileName.Split('/').Last();

                        Console.Out.WriteLine(entry.FileName);
                        entry.Extract(targetDir, ExtractExistingFileAction.OverwriteSilently);
                    }

                });

            }

            Log(session, "NuGet " + nugetName + " extracted to " + targetDir);

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult UninstallApplications(Session session)
        {
            string msiexecPath = session["SystemFolder"] + "msiexec.exe";
            session["INSTALLER_PATH"] = msiexecPath;
            string userMessage = "Uninstalling {0}.";
            string finalMessage= "";
            
            string selectedProd = session["NSB_PROP"];
            string prodSearch = session["NSB_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x " + session["NSB_PRODCODE"] + " /qn";
                finalMessage = string.Format(userMessage, session["NSB_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2); // we use 2 units for the percentage parameters because here we have only one action, i.e. just uninstall, without no download
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SC_PROP"];
            prodSearch = session["SC_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x " + session["SC_PRODCODE"] + " /qn";
                finalMessage = string.Format(userMessage, session["SC_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2);
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SI_PROP"];
            prodSearch = session["SI_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x " + session["SI_PRODCODE"] + " /qn";
                finalMessage = string.Format(userMessage, session["SI_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2);
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SP_PROP"];
            prodSearch = session["SP_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x " + session["SP_PRODCODE"] + " /qn";
                finalMessage = string.Format(userMessage, session["SP_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2);
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SM_PROP"];
            prodSearch = session["SM_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x " + session["SM_PRODCODE"] + " /qn";
                finalMessage = string.Format(userMessage, session["SM_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2);
                //uninstall app
                session.DoAction("RunExe");
            }

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult DownloadSamplesforSelectedApplications(Session session)
        {
            Log(session, "Begin custom action DownloadSamplesforSelectedApplications");

            string userMessage = "Downloading samples for {0}.";
            string finalMessage = "";

            string selectedSamples = session["SAMP_PROP"];

            string selectedProd = session["SC_PROP"];
            if (!String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(selectedSamples))
            {
            }

            selectedProd = session["SI_PROP"];
            if (!String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(selectedSamples))
            {
            }

            selectedProd = session["SP_PROP"];
            if (!String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(selectedSamples))
            {
            }

            selectedProd = session["SM_PROP"];
            if (!String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(selectedSamples))
            {
            }

            selectedProd = session["NSB_PROP"];
            if (!String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(selectedSamples))
            {
                session["SAMPLE_APPLICATION"] = "nservicebus";
                session["TARGET_SAMPLE_DIR"] = session["NSB_INSTALL_DIR"] + "\\samples";

                finalMessage = string.Format(userMessage, session["NSB_PROD_NAME"]);
                StatusMessage(session, finalMessage);
                IncrementProgressBar(session, 2); // the same as for the uninstall, it is just one operation

                session.DoAction("DownloadSamples");
            }

            Log(session, "End custom action DownloadSamplesforSelectedApplications");

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult DownloadTools(Session session)
        {
            //TODO 

            return ActionResult.Success;
        }



        [CustomAction]
        public static ActionResult DownloadApplication(Session session)
        {
            Log(session, "Begin custom action DownloadApplication");
            
            // property used to check if the download is complete or not
            // must be the first thing executed, otherwise the value of the property PRODUCT_PROP could be overwritten
            string productProp = session["PRODUCT_PROP"];

            var appName = session.Get("APPLICATION_NAME");

            var extractionDir = session.Get("TARGET_APP_DIR");

          //  MessageBox.Show("Please attach a debugger to rundll32.exe.", "Attach");

            var appInfoUrl = string.Format("http://particular.net/api/products/{0}/current", appName);

            // URL used for testing, TO BE commented for product builds
            //var appInfoUrl = string.Format("http://dl.dropboxusercontent.com/u/5392761/Elance/NServiceBus/bundles/{0}", appName + ".zip");


            string urlToDownload = null;

            using (var client = new WebClient())
            {
                string json;
                try
                {
                    json = client.DownloadString(appInfoUrl);
                }
                catch (WebException)
                {
                    appInfoUrl = string.Format("http://particular.net/api/products/{0}/prerelease", appName);

                    json = client.DownloadString(appInfoUrl);

                }
                
            
         
                urlToDownload = json
                    .Replace("\"", "")
                    .Split(new[] { "Uri:" }, StringSplitOptions.RemoveEmptyEntries)
                    .Last()
                    .Split(',')
                    .First()
                 ;
            }

            var fileName = urlToDownload.Split('/').Last();
            
            CreateDir(extractionDir);
            DownloadUrl(urlToDownload, Path.Combine(extractionDir, fileName));

            Log(session, "App " + appName + " extracted to " + extractionDir);
            
            if ( !String.IsNullOrEmpty(productProp) )
                session[productProp] = "DownloadComplete";

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult DownloadApps(Session session)
        {
            Log(session, "Begin custom action DownloadApps");

            string[] fullFilePaths;
            string downloadUserMessage = "Downloading selected applications";
            StatusMessage(session, downloadUserMessage);

            string selectedProd = session["NSB_PROP"];
            string prodSearch = session["NSB_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //download application
                session["APPLICATION_NAME"] = session["NSB_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "Application-" + session["NSB_PROD_NAME"];
                session["TARGET_APP_DIR_NSB"] = session["TARGET_APP_DIR"]; // used to launch the installer as TARGET_APP_DIR is overwritten by the next download
                session["PRODUCT_PROP"] = "NSB_DOWNLOAD";
                
                session.DoAction("DownloadApplication");
            }


            selectedProd = session["SC_PROP"];
            prodSearch = session["SC_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //download application
                session["APPLICATION_NAME"] = session["SC_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "Application-" + session["SC_PROD_NAME"];
                session["TARGET_APP_DIR_SC"] = session["TARGET_APP_DIR"]; // used to launch the installer as TARGET_APP_DIR is overwritten by the next download
                session["PRODUCT_PROP"] = "SC_DOWNLOAD";

                session.DoAction("DownloadApplication");
            }

            selectedProd = session["SI_PROP"];
            prodSearch = session["SI_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //download application
                session["APPLICATION_NAME"] = session["SI_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "Application-" + session["SI_PROD_NAME"];
                session["TARGET_APP_DIR_SI"] = session["TARGET_APP_DIR"]; // used to launch the installer as TARGET_APP_DIR is overwritten by the next download
                session["PRODUCT_PROP"] = "SI_DOWNLOAD";

                session.DoAction("DownloadApplication");
            }

            selectedProd = session["SP_PROP"];
            prodSearch = session["SP_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //download application
                session["APPLICATION_NAME"] = session["SP_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "Application-" + session["SP_PROD_NAME"];
                session["TARGET_APP_DIR_SP"] = session["TARGET_APP_DIR"]; // used to launch the installer as TARGET_APP_DIR is overwritten by the next download
                session["PRODUCT_PROP"] = "SP_DOWNLOAD";

                session.DoAction("DownloadApplication");
            }

            selectedProd = session["SM_PROP"];
            prodSearch = session["SM_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //download application
                session["APPLICATION_NAME"] = session["SM_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "Application-" + session["SM_PROD_NAME"];
                session["TARGET_APP_DIR_SM"] = session["TARGET_APP_DIR"]; // used to launch the installer as TARGET_APP_DIR is overwritten by the next download
                session["PRODUCT_PROP"] = "SM_DOWNLOAD";

                session.DoAction("DownloadApplication");
            }

            Log(session, "End custom action DownloadApps");
            
            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult InstallApps(Session session)
        {
            Log(session, "Begin custom action InstallApps");

            string[] fullFilePaths;
            string installUserMessage = "Installing {0}.";
            string finalMessage = "";

            string selectedProd = session["NSB_PROP"];
            string prodSearch = session["NSB_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //increment progress bar to indicate download action
                IncrementProgressBar(session);

                session["DOWNLOAD_CONFIRMATION_PROP"] = "NSB_DOWNLOAD";
                session.DoAction("CanBeInstalled");
                if (!String.IsNullOrEmpty(session["INSTALLER_VALID"]))
                {
                    //install application
                    fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR_NSB"]);
                    session["INSTALLER_PATH"] = fullFilePaths[0];
                    session["INSTALLER_COMMANDLINE"] = "";
                    finalMessage = string.Format(installUserMessage, session["NSB_PROD_NAME"]);
                    StatusMessageActionData(session, finalMessage);

                    session.DoAction("RunExe");
                    IncrementProgressBar(session);
                }
            }


            selectedProd = session["SC_PROP"];
            prodSearch = session["SC_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //increment progress bar to indicate download action
                IncrementProgressBar(session);

                session["DOWNLOAD_CONFIRMATION_PROP"] = "SC_DOWNLOAD";
                session.DoAction("CanBeInstalled");
                if (!String.IsNullOrEmpty(session["INSTALLER_VALID"]))
                {
                    //install application
                    fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR_SC"]);
                    session["INSTALLER_PATH"] = fullFilePaths[0];
                    session["INSTALLER_COMMANDLINE"] = "";
                    finalMessage = string.Format(installUserMessage, session["SC_PROD_NAME"]);
                    StatusMessageActionData(session, finalMessage);

                    session.DoAction("RunExe");
                    IncrementProgressBar(session);

                }

            }

            selectedProd = session["SI_PROP"];
            prodSearch = session["SI_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //increment progress bar to indicate download action
                IncrementProgressBar(session);

                session["DOWNLOAD_CONFIRMATION_PROP"] = "SI_DOWNLOAD";
                session.DoAction("CanBeInstalled");
                if (!String.IsNullOrEmpty(session["INSTALLER_VALID"]))
                {
                    //install application
                    fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR_SI"]);
                    session["INSTALLER_PATH"] = fullFilePaths[0];
                    session["INSTALLER_COMMANDLINE"] = "";
                    finalMessage = string.Format(installUserMessage, session["SI_PROD_NAME"]);
                    StatusMessageActionData(session, finalMessage);

                    session.DoAction("RunExe");
                    IncrementProgressBar(session);
                }
            }

            selectedProd = session["SP_PROP"];
            prodSearch = session["SP_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //increment progress bar to indicate download action
                IncrementProgressBar(session);

                session["DOWNLOAD_CONFIRMATION_PROP"] = "SP_DOWNLOAD";
                session.DoAction("CanBeInstalled");
                if (!String.IsNullOrEmpty(session["INSTALLER_VALID"]))
                {
                    //install application
                    fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR__SP"]);
                    session["INSTALLER_PATH"] = fullFilePaths[0];
                    session["INSTALLER_COMMANDLINE"] = "";
                    finalMessage = string.Format(installUserMessage, session["SP_PROD_NAME"]);
                    StatusMessageActionData(session, finalMessage);

                    session.DoAction("RunExe");
                    IncrementProgressBar(session);
                }

            }

            selectedProd = session["SM_PROP"];
            prodSearch = session["SM_SEARCH"];
            if (!String.IsNullOrEmpty(selectedProd) && String.IsNullOrEmpty(prodSearch))
            {
                //increment progress bar to indicate download action
                IncrementProgressBar(session);

                session["DOWNLOAD_CONFIRMATION_PROP"] = "SM_DOWNLOAD";
                session.DoAction("CanBeInstalled");
                if (!String.IsNullOrEmpty(session["INSTALLER_VALID"]))
                {
                    //install application
                    fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR_SM"]);
                    session["INSTALLER_PATH"] = fullFilePaths[0];
                    session["INSTALLER_COMMANDLINE"] = "";
                    finalMessage = string.Format(installUserMessage, session["SM_PROD_NAME"]);
                    StatusMessageActionData(session, finalMessage);
                    
                    session.DoAction("RunExe");
                    IncrementProgressBar(session);
                }
            }

            Log(session, "End custom action InstallApps");


            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult DownloadSamples(Session session)
        {
            Log(session, "Begin custom action DownloadSamples");

            var targetDir = session.Get("TARGET_SAMPLE_DIR");

            var application = session.Get("SAMPLE_APPLICATION");

            foreach (var repoToDownload in GetSampleReposForApp(application))
            {
                DownloadSampleRepo(repoToDownload, targetDir);

                Log(session, "Sample " + repoToDownload + " extracted to " + targetDir);
            }
            
            return ActionResult.Success;
        }


        private static IEnumerable<string> GetSampleReposForApp(string application)
        {
            switch (application.ToLower())
            {
                case "nservicebus":
                    return new[]
                    {
                        "Particular/NServiceBus.NHibernate.Samples",
                        "Particular/NServiceBus.SqlServer.Samples",
                        "Particular/NServiceBus.RabbitMQ.Samples",
                        "Particular/NServiceBus.ActiveMQ.Samples",
                        "Particular/NServiceBus.Msmq.Samples",
                        "Particular/NServiceBus.Azure.Samples"
                    };
                default:
                    throw new InvalidOperationException("Unknown app - " + application);
            }
        }

        private static void DownloadSampleRepo(string repositoryId, string targetDir)
        {
            var repoName = repositoryId.Split('/').Last();


            var urlToDownload = string.Format("http://github.com/{0}/archive/master.zip", repositoryId);


            var zipFileName = Path.GetTempFileName();



            DownloadUrl(urlToDownload, zipFileName);


            using (var zipFile = ZipFile.Read(zipFileName))
            {
                zipFile.ToList().ForEach(entry =>
                {
                    var fileName = entry.FileName.Replace(repoName + "-master/", "");

                    if (!string.IsNullOrEmpty(fileName) &&
                        !fileName.StartsWith(".") && //no . files
                        fileName.Contains("/")) // nothing in the root
                    {
                        entry.FileName = entry.FileName.Replace(repoName + "-master/", "");

                        Console.Out.WriteLine(entry.FileName);
                        entry.Extract(targetDir, ExtractExistingFileAction.OverwriteSilently);
                    }
                });
            }
        }


        static void DownloadUrl(string urlToDownload, string targetPath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(urlToDownload, targetPath);
            }
        }



        [CustomAction]
        public static ActionResult RunExe(Session session)
        {
            var installerPath = session["INSTALLER_PATH"];

            var startInfo = new ProcessStartInfo(installerPath)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = session["INSTALLER_COMMANDLINE"],
                WorkingDirectory = Path.GetTempPath()
            };

            session.Log("Executing {0} {1}", startInfo.FileName, startInfo.Arguments);

            using (var process = new Process())
            {
                var output = new StringBuilder();
                var error = new StringBuilder();

                process.StartInfo = startInfo;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        error.AppendLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                session.Log(output.ToString());
                session.Log(error.ToString());

                if (process.ExitCode != 0)
                {
                    return ActionResult.Failure;
                }

                return ActionResult.Success;
            }
        }




        [CustomAction]
        public static ActionResult ValidateUrl(Session session)
        {
            Log(session, "Begin custom action ValidateUrl");

            // getting URL from property
            string url = session.Get("INST_URI");

            var connectionSuccessful = false;

            try
            {

                if (url == null)
                {
                    return ActionResult.Success;
                }

                var request = WebRequest.Create(url);
                request.Timeout = 2000;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(response.StatusDescription);
                    }

                    string version = response.Headers["X-Particular-Version"];

                    if (!string.IsNullOrEmpty(version))
                    {
                        session.Set("REPORTED_VERSION", version.Split(' ').First());
                    }
                }

                connectionSuccessful = true;
            }
            catch (Exception ex)
            {
                Log(session, ex.ToString());
            }

            if (connectionSuccessful)
            {
                session.Set("VALID_URL", "Valid_Url");
            }
            else
            {
                session.Set("VALID_URL", "Invalid_Url");
            }

            Log(session, "End custom action ValidateUrl");

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult InitialiseProgressBar(Session session)
        {
            Log(session, "Begin custom action InitialiseProgressBar");

            int totalStatements = Int32.Parse(session["CHANGED_SELECTIONS"]);// the property is set by the HTML host

            //set the number of segments for the progress bar. doubled because we have download and install actions
            ResetProgressBar(session, totalStatements*2);

            Log(session, "End custom action InitialiseProgressBar");

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult CanBeInstalled(Session session)
        {
            Log(session, "Begin custom action CanBeInstalled");

            /*
             * Checks and waits for the download to be complete before launching the installer.
             */

            int retries = 450; // retry/wait for 15 minutes for each app to download
            string productProp = session["DOWNLOAD_CONFIRMATION_PROP"];
            
            //reset the property, which can be set by previous call
            session["INSTALLER_VALID"] = "";

            while (true)
            {
                if (--retries == 0)
                {
                    Log(session, "Begin custom action CanBeInstalled");

                    return ActionResult.Success;
                }
                else if (!String.IsNullOrEmpty(session[productProp]))
                {
                    session["INSTALLER_VALID"] = "set";
                    
                    Log(session, "End custom action CanBeInstalled");

                    return ActionResult.Success;
                }

                else

                    Thread.Sleep(2000); // wait 2 seconds and check again if the download is complete
            }
        }


        public static void StatusMessage(Session session, string status)
        {
            Record record = new Record(3);
            record[1] = "callAddProgressInfo";
            record[2] = status;
            record[3] = "Incrementing tick [1] of [2]";

            session.Message(InstallMessage.ActionStart, record);
        }

        public static void StatusMessageActionData(Session session, string status)
        {
            Record record = new Record(1);
            record[1] = status;

            session.Message(InstallMessage.ActionData, record);
        }
        
        public static MessageResult ResetProgressBar(Session session, int totalStatements)
        {
            var record = new Record(3);
            record[1] = 0; // "Reset" message 
            record[2] = totalStatements;  // total ticks 
            record[3] = 0; // forward motion 
            return session.Message(InstallMessage.Progress, record);
        }

        public static MessageResult IncrementProgressBar(Session session, int progressPercentage = 1 /*1 by default, i.e. one unit for each download or install action*/)
        {
            var record = new Record(3);
            record[1] = 2; // "ProgressReport" message 
            record[2] = progressPercentage; // ticks to increment 
            record[3] = 0; // ignore 
            return session.Message(InstallMessage.Progress, record);
        }


        static void CreateDir(string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }
            
            Directory.CreateDirectory(targetDir);
        }


        static void Log(Session session, string message)
        {
            LogAction(session, message);
        }


        static void CaptureOut(Action execute, Session session)
        {
            var sb = new StringBuilder();
            var standardOut = Console.Out;
            using (var stringWriter = new StringWriter(sb))
            {
                Console.SetOut(stringWriter);

                try
                {
                    execute();
                }
                catch (Exception)
                {
                    var windowsIdentity = WindowsIdentity.GetCurrent();
                    if (windowsIdentity != null)
                    {
                        session.Log("Running as: {0}", windowsIdentity.Name);
                    }
                    else
                    {
                        session.Log("Running as: {0}", Environment.UserName);
                    }
                    throw;
                }
                finally
                {
                    session.Log(sb.ToString());

                    Console.SetOut(standardOut);
                }
            }

        }

        public static Action<Session, string> LogAction = (s, m) => s.Log(m);

        public static Func<Session, string, string> GetAction = (s, key) => s[key];

        public static Action<Session, string, string> SetAction = (s, key, value) => s[key] = value;
    }

    public static class SessionExtentions
    {
        public static string Get(this Session session, string key)
        {
            return CustomActions.GetAction(session, key);
        }

        public static void Set(this Session session, string key, string value)
        {
            CustomActions.SetAction(session, key, value);
        }
    }
}


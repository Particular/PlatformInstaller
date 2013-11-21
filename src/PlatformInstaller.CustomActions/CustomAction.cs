using System.Collections.Generic;

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

            string selectedProd = session["SI_PROP"];
            string prodSearch = session["SI_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x" + session["SI_PRODCODE"] + "/qn";
                session["INSTALLER_PROP_NAME"] = "UNINST_SI";
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SP_PROP"];
            prodSearch = session["SP_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x" + session["SP_PRODCODE"] + "/qn";
                session["INSTALLER_PROP_NAME"] = "UNINST_SP";
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SC_PROP"];
            prodSearch = session["SC_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x" + session["SC_PRODCODE"] + "/qn";
                session["INSTALLER_PROP_NAME"] = "UNINST_SC";
                //uninstall app
                session.DoAction("RunExe");
            }

            selectedProd = session["SM_PROP"];
            prodSearch = session["SM_SEARCH"];
            if (String.IsNullOrEmpty(selectedProd) && !String.IsNullOrEmpty(prodSearch))
            {
                session["INSTALLER_COMMANDLINE"] = "/x" + session["SM_PRODCODE"] + "/qn";
                session["INSTALLER_PROP_NAME"] = "UNINST_SM";
                //uninstall app
                session.DoAction("RunExe");
            }

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult DownloadSamplesforSelectedApplications(Session session)
        {
            Log(session, "Begin custom action DownloadSamplesforSelectedApplications");

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
                session["DOWNLOAD_PROP_NAME"] = "DWLD_NSB_SAMP";

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

            var appName = session.Get("APPLICATION_NAME");

            var extractionDir = session.Get("TARGET_APP_DIR");


            var appInfoUrl = string.Format("http://particular.net/api/products/{0}/current", appName);

            // URL used for testing, TO BE commented for product builds
            //var urlToDownload = string.Format("http://dl.dropboxusercontent.com/u/5392761/Elance/NServiceBus/bundles/{0}", appName + ".zip");


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

            DownloadUrl(urlToDownload, Path.Combine(extractionDir, fileName));


            Log(session, "App " + appName + " extracted to " + extractionDir);

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult InstallApps(Session session)
        {
            string[] fullFilePaths;
            string selectedProd = session["NSB_PROP"];
            if (!String.IsNullOrEmpty(selectedProd))
            {
                //download application
                session["APPLICATION_NAME"] = session["NSB_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "NServiceBus";
                session["DOWNLOAD_PROP_NAME"] = "DWLD_NSB";

                session.DoAction("DownloadApplication");

                //install application
                fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR"]);
                session["INSTALLER_PATH"] = fullFilePaths[0];
                session["INSTALLER_COMMANDLINE"] = "";
                session["INSTALLER_PROP_NAME"] = "INST_NSB";

                session.DoAction("RunExe");
            }


            selectedProd = session["SC_PROP"];
            if (!String.IsNullOrEmpty(selectedProd))
            {
                //download application
                session["APPLICATION_NAME"] = session["SC_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "ServiceControl";
                session["DOWNLOAD_PROP_NAME"] = "DWLD_SC";

                session.DoAction("DownloadApplication");

                //install application
                fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR"]);
                session["INSTALLER_PATH"] = fullFilePaths[0];
                session["INSTALLER_COMMANDLINE"] = "";
                session["INSTALLER_PROP_NAME"] = "INST_SC";

                session.DoAction("RunExe");
            }

            selectedProd = session["SI_PROP"];
            if (!String.IsNullOrEmpty(selectedProd))
            {
                //download application
                session["APPLICATION_NAME"] = session["SI_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "ServiceInsight";
                session["DOWNLOAD_PROP_NAME"] = "DWLD_SI";

                session.DoAction("DownloadApplication");

                //install application
                fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR"]);
                session["INSTALLER_PATH"] = fullFilePaths[0];
                session["INSTALLER_COMMANDLINE"] = "";
                session["INSTALLER_PROP_NAME"] = "INST_SI";

                session.DoAction("RunExe");
            }

            selectedProd = session["SP_PROP"];
            if (!String.IsNullOrEmpty(selectedProd))
            {
                //download application
                session["APPLICATION_NAME"] = session["SP_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "ServicePulse";
                session["DOWNLOAD_PROP_NAME"] = "DWLD_SP";

                session.DoAction("DownloadApplication");

                //install application
                fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR"]);
                session["INSTALLER_PATH"] = fullFilePaths[0];
                session["INSTALLER_COMMANDLINE"] = "";
                session["INSTALLER_PROP_NAME"] = "INST_SP";

                session.DoAction("RunExe");
            }

            selectedProd = session["SM_PROP"];
            if (!String.IsNullOrEmpty(selectedProd))
            {
                //download application
                session["APPLICATION_NAME"] = session["SM_PROD_NAME"];
                session["TARGET_APP_DIR"] = session["TempFolder"] + "ServiceMatrix";
                session["DOWNLOAD_PROP_NAME"] = "DWLD_SM";

                session.DoAction("DownloadApplication");

                //install application
                fullFilePaths = Directory.GetFiles(session["TARGET_APP_DIR"]);
                session["INSTALLER_PATH"] = fullFilePaths[0];
                session["INSTALLER_COMMANDLINE"] = "";
                session["INSTALLER_PROP_NAME"] = "INST_SM";

                session.DoAction("RunExe");
            }

            Log(session, "End custom action DownloadandInstallSelectedApplications");


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
                //adding property that will be used to report application dowload progress
                // the HTML host leasons for MsiPropertyChanged installer event, so we can update the progress bar when a property gets set
                string propName = session.Get("DOWNLOAD_PROP_NAME");
                if (!String.IsNullOrEmpty(propName))
                {
                    session.Set(propName, "set");
                }


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

                //adding property that will be used to report application install progress
                // the HTML host leasons for MsiPropertyChanged installer event, so we can update the progress bar when a property gets set
                string propName = session["INSTALLER_PROP_NAME"];
                if (!String.IsNullOrEmpty(propName))
                    session[propName] = "set";

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


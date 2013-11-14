namespace PlatformInstaller.CustomActions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Principal;
    using System.Text;
    using Microsoft.Deployment.WindowsInstaller;

    public class CustomActions
    {

        [CustomAction]
        public static ActionResult DownloadSamples(Session session)
        {
            Log(session, "Begin custom action ValidateUrl");

            var repositoryId = session.Get("SAMPLE_REPOSITORY");

            var targetDir = session.Get("TARGET_DOWNLOAD_DIR");

            var urlToDownload = string.Format("http://github.com/{0}/archive/master.zip", repositoryId);

            var targetPath = Path.Combine(targetDir, repositoryId.Replace("/", "_") + ".zip");

            using (var client = new WebClient())
            {
                client.DownloadFile(urlToDownload, targetPath);
            }


            Log(session, "End custom action ValidateUrl");

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult UnpackSamples(Session session)
        {
            Log(session, "Begin custom action ValidateUrl");

            //var repositoryId = session.Get("SAMPLE_REPOSITORY");
            //var urlToDownload = string.Format("http://github.com/{0}/archive/master.zip", repositoryId);

            //var targetPath = Path.Combine(Path.GetTempPath(), repositoryId.Replace("/", "_") + ".zip");

            //using (var client = new WebClient())
            //{
            //    client.DownloadFile(urlToDownload, targetPath);
            //}

            Log(session, "End custom action ValidateUrl");

            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult RunExe(Session session)
        {
            var customActionData = session[CustomActionData.PropertyName];

            var startInfo = new ProcessStartInfo(customActionData)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
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
                        session.Set("REPORTED_VERSION",version.Split(' ').First());
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


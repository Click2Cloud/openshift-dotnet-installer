﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Click2Cloud.Openshift.Common.Utils;
using Click2Cloud.Openshift.Runtime.Config;
using Click2Cloud.Openshift.Utilities;

namespace Click2Cloud.Openshift.Runtime
{
    public class ApplicationRepository
    {
        string GIT 
        {
            get 
            {
                string gitPath = Path.Combine(NodeConfig.Values["SSHD_BASE_DIR"], @"bin\git.exe");

                return String.Format("\"{0}\"", gitPath);
            }
        }

        string TAR 
        {
            get
            {
                string tarPath = Path.Combine(NodeConfig.Values["SSHD_BASE_DIR"], @"bin\tar.exe");

                return String.Format("\"{0}\"", tarPath);
            }
        }

        private const string GIT_INIT = @"{0} init
{0} config user.email ""builder@example.com""
{0} config user.name ""Template builder""
{0} config core.logAllRefUpdates true
{0} add -f .
{0} commit -a -m ""Creating template";

        private const string GIT_INIT_BARE = @"{0} init --bare
{0} config core.logAllRefUpdates true";

        private const string GIT_LOCAL_CLONE = @"{0} clone --bare --no-hardlinks template {1}.git
set GIT_DIR=./{1}.git
{0} config core.logAllRefUpdates true
{0} repack";

        private const string GIT_URL_CLONE = @"{0} clone --bare --no-hardlinks '{1}' {2}.git
set GIT_DIR=./{2}.git 
{0} config core.logAllRefUpdates true
{3}
{0} repack";

        private const string GIT_URL_CLONE_RESET = @"git reset --soft {0}";

        private const string GIT_ARHIVE = @"{0} archive --format=tar {1} | (cd {2} & {3} --warning=no-timestamp -xf -)";

        private const string GIT_DESCRIPTION = @"{0} application {1}";

        private const string GIT_CONFIG = @"[user]
  name = OpenShift System User
[gc]
  auto = 100
";

        private const string GIT_GET_SHA1 = @"set -xe;
{0} rev-parse --short {1}";

        private const string PRE_RECEIVE = @"gear prereceive";

        private const string POST_RECEIVE = "gear postreceive";

        public ApplicationContainer Container { get; set; }
        public string RepositoryPath { get; set; }
        string cartridgeName;
        string applicationName;

        public ApplicationRepository(ApplicationContainer container) : this(container, null) { }

        public ApplicationRepository(ApplicationContainer container, string path)
        {
            this.Container = container;
            this.RepositoryPath = path ?? Path.Combine(container.ContainerDir, "git", string.Format("{0}.git", container.ApplicationName));
        }

        public string PopulateFromUrl(string cartridgeName, string templateUrl)
        {            
            if (Exists())
                return null;
            string repoSpec;
            string commit;
            try
            {
                Git.SafeCloneSpec(templateUrl, out repoSpec, out commit, Git.ALLOWED_SCHEMES);
            }
            catch
            {
                throw new Exception("CLIENT_ERROR: The provided source code repository URL is not valid");
            }
            if (repoSpec == null)
            {
                throw new Exception(string.Format("CLIENT_ERROR: Source code repository URL protocol must be one of: {0}", string.Join(", ", Git.ALLOWED_SCHEMES)));
            }

            string gitPath = Path.Combine(this.Container.ContainerDir, "git");
            Directory.CreateDirectory(gitPath);

            this.cartridgeName = cartridgeName;
            this.applicationName = this.Container.ApplicationName;

            string reset = string.Empty;
            if(!string.IsNullOrEmpty(commit))
            {
                reset = string.Format(GIT_URL_CLONE_RESET, commit);
            }
            string script = string.Format(GIT_URL_CLONE, GIT, repoSpec, this.applicationName, reset);
            RunCmd(script, gitPath);
            Configure();
            return string.Empty;
        }

        public string PopulateFromCartridge(string cartridgeName)
        {
            if (Exists())
                return null;

            Directory.CreateDirectory(Path.Combine(this.Container.ContainerDir, "git"));

            string[] locations = new string[] {
                Path.Combine(this.Container.ContainerDir, cartridgeName, "template"),
                Path.Combine(this.Container.ContainerDir, cartridgeName, "template.git"),
                Path.Combine(this.Container.ContainerDir, cartridgeName, "usr", "template"),
                Path.Combine(this.Container.ContainerDir, cartridgeName, "usr", "template.git")
            };

            string template = null;
            foreach (string dir in locations)
            {
                if (Directory.Exists(dir))
                {
                    template = dir;
                    break;
                }
            }
            if (template == null)
            {
                return null;
            }

            this.cartridgeName = cartridgeName;
            this.applicationName = this.Container.ApplicationName;

            if (template.EndsWith(".git"))
            {
                DirectoryUtil.DirectoryCopy(template, RepositoryPath, true);
            }
            else
            {
                BuildBare(template);
            }
            Configure();

            return template;
        }

        private void BuildBare(string path)
        {
            string template = Path.Combine(this.Container.ContainerDir, "git", "template");            
            if (Directory.Exists(template))
            {
                Directory.Delete(template, true);                
            }
            Directory.CreateDirectory(template);
            string gitPath = Path.Combine(this.Container.ContainerDir, "git");
            DirectoryUtil.DirectoryCopy(path, template, true);
            RunCmd(string.Format(GIT_INIT, GIT), template);
            RunCmd(string.Format(GIT_LOCAL_CLONE, GIT, this.Container.ApplicationName), gitPath);
        }

        public void Configure()
        {
            Container.SetRWPermissions(this.RepositoryPath);
            string hooks = Path.Combine(this.RepositoryPath, "hooks");
            Container.SetRoPermissions(hooks);
            File.WriteAllText(Path.Combine(this.RepositoryPath, "description"), string.Format(GIT_DESCRIPTION, this.cartridgeName, this.applicationName));
            File.WriteAllText(Path.Combine(this.Container.ContainerDir, ".gitconfig"), GIT_CONFIG);
            File.WriteAllText(Path.Combine(hooks, "pre-receive"), PRE_RECEIVE);
            File.WriteAllText(Path.Combine(hooks, "post-receive"), POST_RECEIVE);
        }

        public void Archive(string destination, string refId)
        {            
            if (!Exists())
                return;
            if (Directory.GetFiles(RepositoryPath).Length == 0)
            {
                return;
            }
            DirectoryUtil.EmptyDirectory(destination);
            Directory.CreateDirectory(destination);
            string command = string.Format(GIT_ARHIVE, GIT, refId, destination, TAR);
            RunCmd(command, RepositoryPath);
        }

        public bool Exists()
        {
            return Directory.Exists(this.RepositoryPath);
        }

        public bool Empty()
        {
            if (!Exists())
                return false;
            return Directory.GetFiles(this.RepositoryPath).Length == 0;
        }

        public bool FileExists(string filename, string refId)
        {
            ProcessResult pr = ProcessExtensions.RunCommandAndGetOutput("cmd", string.Format("git ls-tree {0} -- {1}", refId, filename), this.RepositoryPath);
            if(pr.ExitCode == 0)
            {
                return true;
            }
            return false;
        }

        public string GetSha1(string refId)
        {
            string cmd = string.Format(GIT_GET_SHA1, GIT, refId);

            string tempfile = Path.GetTempFileName();
            string batfile = tempfile + ".bat";
            File.WriteAllText(batfile, cmd);

            string arguments = "/c " + batfile;

            ProcessResult result = ProcessExtensions.RunCommandAndGetOutput("cmd.exe", arguments, this.RepositoryPath);

            File.Delete(tempfile);
            File.Delete(batfile);

            if (result.ExitCode == 0)
            {
                return result.StdOut;
            }
            else
            {
                Logger.Error("GetSHA failed with error code {0}; out: {1}; err: {2}", result.ExitCode, result.StdOut , result.StdErr);
                return string.Empty;
            }
        }

        private void RunCmd(string cmd, string dir)
        {            
            string batfile = Path.Combine(this.Container.ContainerDir, string.Format("{0}cmd.bat", Guid.NewGuid().ToString("N")));

            File.WriteAllText(batfile, string.Format("cd /D {0}\r\n{1}", dir, cmd));

            this.Container.RunProcessInContainerContext(dir, string.Format("cmd.exe /c {0}", batfile));

            File.Delete(batfile);
        }
    }
}

﻿// Code Linker originally by @CADbloke (Ewen Wallace) 2015
// More info, repo and MIT License at https://github.com/CADbloke/CodeLinker

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeLinker
{
    internal static class ProjectMaker
    {
        /// <summary> Copies and links a List of Projects into the <c> destinationFolder </c>.
        ///    <para> File is Linked from the first <c> projectsToLink </c> for each if there is more than 1 source. </para> </summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null or empty. </exception>
        /// <param name="projectsToLink"> a List of projects to Link. </param>
        /// <param name="destinationFolder"> Pathname of the destination folder. Empty string throws <c>ArgumentNullException</c></param>
        /// <param name="createSubFolders"> True (default) if you want each now project in its own subfolder. </param>
        internal static void NewProject(List<ProjectToLink> projectsToLink, string destinationFolder, bool createSubFolders = true)
        {
            if (projectsToLink == null)
                throw new ArgumentNullException(nameof(projectsToLink));
            if (string.IsNullOrEmpty(destinationFolder))
                throw new ArgumentNullException(nameof(destinationFolder));

            var destinationProjects = new HashSet<string>(projectsToLink.Select(p => p.DestinationProjectName));
            Log.WriteLine("Recycling " + destinationProjects.Count + " Project(s) to " + destinationFolder);

            foreach (string destinationProject in destinationProjects)
            {
                string destinationProjPath = createSubFolders
                    ? Path.Combine(destinationFolder + "\\" + Path.GetFileNameWithoutExtension(destinationProject), destinationProject)
                    : Path.Combine(destinationFolder, destinationProject);
                if (File.Exists(destinationProjPath))
                {
                    bool overwriteExisting = YesOrNo.Ask(destinationProjPath + Environment.NewLine +
                                                         "already exists!" + Environment.NewLine +
                                                         "Overwrite it ?");
                    if (!overwriteExisting)
                        continue;
                }

                List<string> sources = projectsToLink
                    .Where(d => d.DestinationProjectName == destinationProject)
                    .Select(s => s.SourceProject).ToList();
                if (sources.Count != 1)
                {
                    string message = destinationProject + "has " + sources.Count + " source Projects." + Environment.NewLine;
                    for (var index = 0; index < sources.Count; index++)
                    {
                        string source = sources[index];
                        message += index + 1 + ". " + source + Environment.NewLine;
                    }
                    message += "Continue (Y) or skip (N) " + destinationProject + " or Cancel Everything?";

                    bool? carryOn = YesOrNo.OrCancel(message);
                    if (carryOn == null)
                    {
                        Log.WriteLine("User aborted All Recycling. " + message);
                        return; // bail out of everything
                    }
                    if (carryOn == false)
                    {
                        Log.WriteLine("User skipped one Linked Project. " + message);
                        continue; // skip just this Destination Project
                    }
                }

                if (sources.Any())
                {
                    Log.WriteLine("Recycling to :" + destinationProjPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationProjPath)); // safe. Doesn't care if it already exists.
                    File.Copy(sources[0], destinationProjPath, true);
                    var destinationProjXml = new DestinationProjXml(destinationProjPath);
                    destinationProjXml.ClearOldLinkedCode();
                    destinationProjXml.ClearStartPlaceholderContent();
                    destinationProjXml.AddExclusion("app.config");
                    Log.WriteLine("...because a linked App.config will cause problems when you change build settings.");
                    destinationProjXml.AddExclusion("packages.config");
                    Log.WriteLine("...because nuget's packages.config will cause problems when you change build settings.");
                    destinationProjXml.ClearAllExistingCodeExceptExplicitlyLinked();

                    foreach (string source in sources)
                    {
                        if (File.Exists(source))
                            destinationProjXml.AddSource(PathMaker.MakeRelativePath(destinationProjPath, source));
                        else
                            Log.WriteLine("Bad Source in: " + destinationProjPath + Environment.NewLine 
                                        + "       Source: " + source + Environment.NewLine);
                    }

                    destinationProjXml.Save();

                    App.ParseCommands(new[] {destinationProjPath});
                }
            }
        }

        internal static void NewProject(ProjectToLink projectsToLink, string destinationFolder, bool createSubFolders = true)
        {
            NewProject(new List<ProjectToLink> { projectsToLink }, destinationFolder, createSubFolders);
        }

        internal static void NewProject(string projectToLink, string destinationFolder, bool createSubFolders = true)
        {
            NewProject(new List<ProjectToLink> { new ProjectToLink { SourceProject = projectToLink } }, destinationFolder, createSubFolders);
        }
    }
}

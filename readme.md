# Code Linker 
## Recycles Source code between CSPROJ files

**Wait, no I don't.** Code Linker creates links in the destination `.csproj` project file to the code files in the source `.csproj`, automating the process of [adding existing files as-a-link](https://msdn.microsoft.com/en-us/library/windows/apps/jj714082(v=vs.105).aspx) into the destination project. The files are added as relative paths (back to the original) if they are already relative links out. If they were originally absolute paths then that is preserved.  If they already have a link I'll try not to break it.
 
See the [Wiki](https://github.com/CADbloke/CodeLinker/wiki) for more detailed info.

There's a [page on the GUI app](https://github.com/CADbloke/CodeLinker/wiki/Using-the-GUI-App) and one for the [Command line app](https://github.com/CADbloke/CodeLinker/wiki/Command-Line).

More instructions coming soon. Your [feedback](https://github.com/CADbloke/CodeLinker/issues) would be more than welcome.

There is a log file called *CodeLinkerLog.txt* saved in the same folder as the executable. If you use this as a *pre/post-build process* The Visual Studio output window will have some summary information but the details will be in the log file. Good luck finding anything in the output window anyway 

[![Join the chat at https://gitter.im/CADbloke/CodeLinker](https://badges.gitter.im/CADbloke/CodeLinker.svg)](https://gitter.im/CADbloke/CodeLinker?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

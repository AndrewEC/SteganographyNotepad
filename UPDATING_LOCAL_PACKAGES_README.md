# Updating Local Packages
1. From the `SteganographyApp` folder, not the `SteganographyNotepad` folder, run the script `./run_package.ps1`
2. Copy the following files into the `SteganographyNotepad/local-packages` folder:
    * SteganographyApp\SteganographyApp.Common\bin\Release\SteganographyApp.Common.&lt;version&gt;.nupkg
    * SteganographyApp\SteganographyApp.Common.Arguments\bin\Release\SteganographyApp.Common.Arguments.&lt;version&gt;.nupkg
3. Clear the nuget cache with the command: `dotnet nuget locals all --clear`
4. Install the new dependencies: `dotnet restore`
5. If step 4 was successful then run the command: `dotnet publish -c Release`
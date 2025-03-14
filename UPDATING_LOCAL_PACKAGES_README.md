# Updating Local Packages
1. From the `SteganographyApp` folder, not the `SteganographyNotepad` folder, run the script `./RunPackage.ps1`
2. Delete the .nupkg files that exist in the `SteganographyNotepad/local-packages` folder.
3. Copy the following files into the `SteganographyNotepad/local-packages` folder:
    * `SteganographyApp\SteganographyApp.Common\bin\Release\SteganographyApp.Common.<version>.nupkg`
    * `SteganographyApp\SteganographyApp.Common.Arguments\bin\Release\SteganographyApp.Common.Arguments.<version>.nupkg`
4. Run the following two commands to ensure the dependencies are installed:
    * `dotnet add package SteganographyApp.Common --version <version>`
    * `dotnet add package SteganographyApp.Common.Arguments --version <version>`
4. If you are using VS Code you may need to close and re-open it to see the changes.
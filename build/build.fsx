#r @"..\packages\FAKE.3.14.4/tools/FakeLib.dll"
open Fake

RestorePackages()

let product = "AhoCorasick.Net"
let description = "Implementation of Aho-Corasick algorithm on .NET"
let copyright = "Copyright © 2015"
let authors = [ "Alexandr Nikitin" ]
let company = "Alexandr Nikitin"
let tags = ["aho-corasick"]
let version = "0.6.1-beta"

let buildDir = "output"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@  product
let packagingSourceDir = packagingRoot @@  product + ".Source"
let nugetPath = "../.nuget/nuget.exe"

Target "Clean" (fun _ ->
    CleanDirs [buildDir; packagingRoot]
)

Target "Default" (fun _ ->
    trace "Hello World from FAKE"
)

Target "Build" (fun _ ->
    !! "../src/AhoCorasick.Net/**/*.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "Build-Output: "
)

open Fake.AssemblyInfoFile

Target "AssemblyInfo" (fun _ ->
    let assemblyInfoVersion = version + ".0"
    CreateCSharpAssemblyInfo "../src/AhoCorasick.Net/Properties/AssemblyInfo.cs"
        [Attribute.Title product
         Attribute.Description description
         Attribute.Copyright copyright
         Attribute.Guid "7e918fef-c4dc-4dc4-b366-f2fc38eced4c"
         Attribute.Product product
         Attribute.Version assemblyInfoVersion
         Attribute.FileVersion assemblyInfoVersion]
)

Target "NuGet" (fun _ ->
    let net45Dir = packagingDir @@ "lib/net45/"
    ensureDirectory net45Dir

    CopyFile net45Dir (buildDir @@ "AhoCorasick.Net.dll")
    CopyFile net45Dir (buildDir @@ "AhoCorasick.Net.pdb")

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = product
            Description = description
            Summary = description
            Tags = tags |> String.concat " "
            Version = version

            OutputPath = packagingRoot
            WorkingDir = packagingDir
            ToolPath = nugetPath

            Publish = true
            }) 
            "AhoCorasick.Net.nuspec"
)

Target "NuGetSource" (fun _ ->
    
    let contentDir = packagingSourceDir @@ "content"
    ensureDirectory contentDir

    CopyFile contentDir (buildDir @@ "C:/Users/a.nikitin/Documents/Projects/my/AhoCorasick.net/src/AhoCorasick.Net/AhoCorasickTree.cs")

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = product + ".Source"
            Description = description
            Summary = description
            Tags = tags |> String.concat " "
            Version = version

            OutputPath = packagingRoot
            WorkingDir = packagingSourceDir
            ToolPath = nugetPath

            Publish = true 
            }) 
            "AhoCorasick.Net.Source.nuspec"
)

"Clean"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "NuGet"
  ==> "NuGetSource"
  ==> "Default"

RunTargetOrDefault "Default"
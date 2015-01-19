#r @"..\packages\FAKE.3.14.4/tools/FakeLib.dll"
open Fake

RestorePackages()

let product = "AhoCorasick.Net"
let description = "Implementation of Aho-Corasick algorithm on .NET"
let copyright = "Copyright © 2015"
let authors = [ "Alexandr Nikitin" ]
let company = "Alexandr Nikitin"
let tags = ["aho-corasick"]
let version = "0.1.0"

let buildDir = "output"
let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@  product

Target "Clean" (fun _ ->
    CleanDir buildDir
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
    let nugetPath = "../.nuget/nuget.exe"
    let net45Dir = packagingDir @@ "lib/net45/"
    CleanDirs [packagingRoot]
    
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

            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = false 
            }) 
            "C:/Users/a.nikitin/Documents/Projects/my/AhoCorasick.net/build/AhoCorasick.Net.nuspec"
)

"Clean"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "NuGet"
  ==> "Default"

RunTargetOrDefault "Default"
#r @"..\packages\FAKE.3.14.4/tools/FakeLib.dll"
open Fake

RestorePackages()

let version = "0.1.0"
let buildDir = "output"

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

"Clean"
  ==> "Build"
  ==> "Default"

RunTargetOrDefault "Default"
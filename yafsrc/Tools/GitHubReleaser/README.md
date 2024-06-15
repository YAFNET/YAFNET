# GitHubReleaser
Simple way to automatically create GitHub releases with changelog and attachments out of issues & milestones.

![](img/flow.png)

## Functions
Creates a release with the version of a DLL or EXE file, e.g. `1.2.3.4`. The last version part is the build number.

GitHubReleaser searches for a milestone without the build number e.g. `1.2.3`. All issues in the milestone would be printed in the release changelog. You can also define which label should be in or filter some single issues out.

You can also attach files to the release e.g. ZIP-File with your binaries.  
If the release is a pre-release, all closed issues of the milestone are collected since last release (date). If the release is __NOT__ a pre-release, all issues of the milestone are collected (summary).

You can also update a release (changelog & attachments).

## Usage
Get a API-Token for your GitHub-User [here](https://github.com/settings/tokens). We need the scope `repo`.

Execute GitHubRelease via command line arguments:
```shell
GitHubReleaser.exe /github-repo:"MyUsername/MyRepo" /github-token:"5578d3a8e0ed1c524e3c8c832e1533d79b16ad3c" /file-for-version:"C:\test\MyDllOrExe.dll" /pre-release:"false" /issue-labels:"bug;Bug" "feature;Feature" /issue-filter-label:"not-in-changelog" /release-attachments:"C:\test\MyDllOrExe1.zip" "C:\test\MyDllOrExe2.zip" /create-changelog-file:"false"
```

Or open config file with GitHubRelease:
```shell
GitHubReleaser.exe "C:\test\MySettings.json"
```

Combining config file with commandline arguments is not supported yet.

## Parameter

| Name                        | Example                                               | Required |
| --------------------------- | ----------------------------------------------------- | -------- |
| `github-repo`               | _MyUsername/MyRepo_                                   | ✅        |
| `github-token`              | _5578d3a8e0ed1c524e3c8c832e1533d79b16ad3c_            | ✅        |
| `file-for-version`          | "C:\test\MyDllOrExe.dll"                              | ✅        |
| `pre-release`               | _false_                                               |          |
| `issue-labels`              | _"bug;Bug" "feature;Feature"_                         |          |
| `issue-filter-label`        | not-in-changelog                                      |          |
| `release-attachments`       | _"C:\test\MyDllOrExe1.zip" "C:\test\MyDllOrExe2.zip"_ |          |
| `update-only`               | _false_                                               |          |
| `create-changelog-file`     | _false_                                               |          |
| `draft`                     | _false_                                               |          |
| `delete-files-after-upload` | _false_                                               |          |


## Config file example

```json
{
  "IsChangelogFileCreationEnabled": false,
  "IsUpdateOnly": true,
  "ReleaseAttachments": [ "C:\\test\\MyDllOrExe1.zip", "C:\\test\\MyDllOrExe2.zip" ],
  "IssueFilterLabel": "not-in-changelog",
  "IssueLabels": {
    "bug": "Bug", 
    "feature": "Feature"
  },
  "IsPreRelease": false,
  "IsDraft": false,
  "DeleteFilesAfterUpload": false,
  "FileForVersion": "C:\\test\\MyDllOrExe.dll",
  "GitHubToken": "_5578d3a8e0ed1c524e3c8c832e1533d79b16ad3c_",
  "GitHubRepo": "MyUsername/MyRepo"
}
```

## Build
For debugging you can add a Secrets.cs to the folder Model:
![](img/build_secrets.png)

For testing we use [Smart Command Line Arguments](https://marketplace.visualstudio.com/items?itemName=MBulli.SmartCommandlineArguments) Visual Studio extension.  
With a test milestone and some test issues you can simple run throw the steps:

![](img/build_smart-command-line-arguments.png)

## Contributing
Feel free to make a pull-request 🦄
Or simple make an issue for a bug report or feature request 💖

Thanks to [Daniel Papp](https://github.com/DanielPa) for being a first class contributer.

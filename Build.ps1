git pull
& '.\Build project.bat'
$ver = Get-Content "last ver.txt"
$commitHash = git rev-parse HEAD
Set-Location ".\Builds\Build ver $ver"
out-file -append -filepath "./build info.txt" -encoding ASCII -inputobject "commit hash: $commitHash"
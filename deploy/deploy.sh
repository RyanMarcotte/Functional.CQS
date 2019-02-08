ApiKey=$1
Source=$2

nuget push ./src/Functional.CQS.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
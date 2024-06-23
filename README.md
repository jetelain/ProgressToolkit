# Progress Toolkit

Long-running task progress tracking toolkit.

The API is not yet stable and may be subject to change.

## General usage

You need to create a "Progress Render", to acquire an `IProgressScope` that will be the base to create detailed progress reports.

Multiple implementation exists (see below for example and preview) : 
 - Console (`ConsoleProgessRender`),
 - WPF (`ProgressViewModel`), 
 - Plain text (`TextProgressRender`)

On a scope you can create :
- sub scope : a progress compose of sub-progress, you can give an estimated number of sub-progress to have an overall progress
- integer/long based progress : your code have to report how many items it have been processed
- percent based progress : your code have to report a progress in percent

To ease inter-operability, provided interfaces derives from `IProgress<>`.

### Integer Progress (`IProgressInteger`)

```
using (var progress = scope.CreateInteger("Integer", 250))
{
    for (int i = 0; i < 250; i++)
    {
        // do something
        rep.ReportOneDone();
    }
}
```

or 

```
using (var progress = scope.CreateInteger("Integer", 250))
{
    for (int i = 0; i < 250; i++)
    {
        // do something
        rep.Repport(i);
    }
}
```

### Percent Progress (`IProgressPercent`)

```
using (var progress = scope.CreatePercent("Integer"))
{
    for (int i = 0; i < 250; i++)
    {
        // do something
        rep.Repport(i * 100.0 / 250);
    }
}
```

### Scope (`IProgressScope`)

## Console

Example: [ConsoleDemo](demo/ConsoleDemo/Program.cs)

## WPF


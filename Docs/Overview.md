# Planning and Application Overview

This application is designed to monitor one or more folders using one or more
folder watching technologies. The initial version of the application with not
monitor file modifications, only the presence of the files.

## Monitoring Mechanisms

For flexibility and performance, multiple monitoring paradigms will be provided.
Different monitoring techniques have different strengths and weaknesses, and
hopefully this will allow for the best from each. Multiple folder watching
techniques can be used in tandem without any adverse affects.

### Polling

Polling is the most straightforward monitoring mechanism. Polling uses a loop
with a given interval to periodically check a folder for files. Any files that
have been added since the last time the directory was scanned will be emitted. 

### FileSystemWatcher

The Dotnet [FileSystemWatcher](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=net-6.0)
allows us to monitor a directory for changes. When changes occur, an event is emitted.
Utilizing this class, we can create a watcher that alerts the application immediately
upon new files being added to a directory. Unfortunately, the FileSystemWatcher
has some limitations and weaknesses. In some cases, the buffer can become full,
and new files will be missed. In other cases, a FileSystemWatcher may fail to 
emit events properly if the folder being monitored is a network share.


## Application Flow

1. New file added to monitored folder.
2. File is picked up by a watcher
3. File is added to a "seen files" collection.
4. Watcher emits an application event denoting that a new file has been seen.
5. A dedicated worker service listens for new file events, when one is received,
   it begins working the new file queue (if it isn't already).
6. The worker service determines if there are any processors for each file, if 
   there isn't one, it ignores the file. If there is one, it places the file in
   the processing pipeline.


## Processing Pipeline

Once a file has been seen, it enters into the processing pipeline. The processing
pipeline allows developers to create unique processing scenarios for their different
use cases. The pipeline furnishes 3 interfaces that allow for extending and modifying
the default pipeline behavior: `IPreprocessingBehavior`, `IProcessingBehavior`,
and `IPostprocessingBehavior`.
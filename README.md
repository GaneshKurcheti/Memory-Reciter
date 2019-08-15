# Memory Reciter(BETA) - Essential memory profiler for a .NET Process

Memory Reciter is a free and opensource tool for finding memory leaks and optimizing the memory usage in .NET applications. You can track down the cause of memory problems, by finding out which objects use more memory and which objects are surviving the longest. Apart from identifying the memory leaks, it comes with added functionalities like analyzing the exe file to get the method level details, digging the pdb information and generating code that helps the user to optimize the code for effective execution. And yes you can visualize the heap contents using Memory Recitor.

_Memory reciter gives the user more  granular level data  i.e. user can view the very minute level details that an object comprises of while all other memory profilers limit data to object level .This functionality lets user to uniquely identify the object responsible for memory leak._**

## Memory Reciter Functionalities

* Taking multiple snapshots of the running process on different intervals.
* Using snapshot user can analyse:
  * What are all the objects(.NET & User Defined Objects) in the heap.
  * We can also examine each object data, properties, methods and their memory addresses.
  * App domain details with various dependencies(.dlls) injected at runtime & multiple threads created by the process.
  * We can also analyse the call stack of individual thread and get the current stack pointer(last executed method).
* User can also compare different  snapshots and view the changes(Objects Added/Removed) between them.
* User will be able to view hierarchical structure of the classes, methods, method invocations and variables present in an assembly.
* User will be able analyse method level ILCODE.
* User can generate project from the .exe file through decompilation.
* User can also examine PDB files.

## Usage

#### HomePage






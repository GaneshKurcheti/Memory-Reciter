# Memory Reciter(BETA) - A Memory Profiler for a Managed .NET process.

Memory Reciter is a free and opensource tool for finding memory leaks and optimizing the memory usage in .NET applications. You can track down the cause of memory problems, by finding out which objects use more memory and which objects are surviving the longest. Apart from identifying the memory leaks, it comes with added functionalities like analyzing the exe file to get the method level details, digging the pdb information and generating code that helps the user to optimize the code for effective execution. And yes you can visualize the heap contents using Memory Recitor.

_Memory reciter gives the user more  granular level data  i.e. user can view the very minute level details that an object comprises of while all other memory profilers limit data to object level .This functionality lets user to uniquely identify the object responsible for memory leak._**

Get the latest Release [here](https://github.com/GaneshKurcheti/Memo-Reciter/releases)

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

#### Home Window

![HomePage](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/Homepage-1.png)

![HomePage2](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/HomePage-2.png)


#### Profiling Window

![ProfilingPage1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-1.png)

![ProfilingPage2](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-2.png)

![ProfilingPage3](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-3.png)

![ProfilingPage4](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-4.png)

![ProfilingPage6](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-6.png)

![ProfilingPage7](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-7.png)

![ProfilingPage8](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-8.png)



#### Analyzing Window

![AnalyzingPage1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/AnalyzePage-1.png)


#### GenerateCode Window

![GenerateCode1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/GenerateCodePage.png)


#### Pdb Browser Window

![PdbBrowser](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/PdbBroswerPage.png)


## Issues

Nothing is perfect.....I agree that there is really a long way to go...... Please don't hesitate to create issues or suggestions. All issues will be reviewed and answered as soon as possible.

## Contributions

Contributions are always welcome!!! Please fork and create pull request(don't forget to follow the basic standards)... If you have any further questions let me know.















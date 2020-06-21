# Memory Reciter(BETA) - A free and open-source Memory Profiler for .NET applications.

Memory Reciter is a free and opensource memory profiler tool for finding memory leaks, profiling, comparing dumps or snapshots, identifying threads and optimizing the memory usage in .NET applications. You can track down the cause of memory problems, by finding out which objects use more memory and which objects are surviving the longest. Apart from identifying the memory leaks, it comes with added functionalities like analyzing the exe file to get the method level details, digging the pdb information and generating code that helps the user to optimize the code for effective execution. And yes you can visualize the heap contents using Memory Recitor. Most "System.OutOfMemoryException" and deadlocks in any .NET application can be solved using this memory profiler. 

_Memory reciter gives the user more  granular level data  i.e. user can view the very minute level details that an object comprises of while all other memory profilers limit data to object level .This functionality lets user to uniquely identify the object responsible for memory leak._**

Never go with "Out of memory" using meory reciter.........Get the latest Release of memory profiler [here](https://github.com/GaneshKurcheti/Memo-Reciter/releases)

## Memory Reciter Functionalities

* Taking multiple snapshots or memory dumps of the running process on different intervals which is further helpful to identify memory leak.
* Using snapshotor dump user can analyse:
  * What are all the objects(.NET & User Defined Objects) in the heap.
  * We can also examine each object data, properties, methods and their memory addresses.
  * App domain details with various dependencies(.dlls) injected at runtime & multiple threads created by the process.
  * We can also analyse the call stack of individual thread and get the current stack pointer(last executed method).
* User can also compare different dumps or snapshots and view the changes(Objects Added/Removed) between them which is helpful in identifying memory leaks.
* User will be able to view hierarchical structure of the classes, methods, method invocations and variables present in an assembly.
* User will be able analyse method level ILCODE.
* User can generate project from the .exe file through decompilation.
* User can also examine PDB files and get the data out of it.

## Usage

#### Home Window

Home window has of this memory profiler has two options
* Attach to running process and create dumps out of it.
* Launch a process and attach to it.

![HomePage](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/Homepage-1.png)

When choosed to attach to a running process... It will list out all current  running process in the system and we can select any process that we need to profile.

![HomePage2](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/HomePage-2.png)

On clicking on Browse on exe user  needs to select the exe file that profiler need to attach.

#### Profiling Window
On sucessfully attaching to a process... A new window opens with all options such as taking memory dumps or memory snapshots.

User must click on TAKE SNAPSHOT to anylyse the memory by taking the meomory dump at that point in the attached process.
This Data is highly usefull  for profiling the application.
![ProfilingPage1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-1.png)

Once the snapshot or dump is taken user  will profiler will give all details about the memory of application.
![ProfilingPage2](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-2.png)

User can dig deeper to object level details and their memory allocations. 
![ProfilingPage3](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-3.png)

It can even show you what are the fields in each object and even methods that we can access using object.
![ProfilingPage4](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-4.png)

We can compare dumps or snapshots to identify the possible memory leak.
![ProfilingPage6](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-6.png)

Threads and modules  of your application domain can also  be listed 
![ProfilingPage7](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-7.png)

DeadLock Detection using the current stack trace can be viewed using this memory profiler.
![ProfilingPage8](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/ProfilingPage-8.png)



#### Analyzing Window
Variables used in the method  level, Methods in the class level and classes in the assembly level can be viewed in this window.
![AnalyzingPage1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/AnalyzePage-1.png)


#### GenerateCode Window
Thanks to  open source library using which we can generate code from  exe file.
![GenerateCode1](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/GenerateCodePage.png)


#### Pdb Browser Window
Method addresss, variable mappings with indexes is  stored in the PDB files.
![PdbBrowser](https://github.com/GaneshKurcheti/Memo-Reciter/blob/master/Media/PdbBroswerPage.png)


Memory Reciter, A free and open source Memory profiler thus can highly used to debug and come across deadlocks, out of memory exceptions and many other errors or issues in the .NET applications.
## Issues
 
Nothing is perfect.....I agree that there is really a long way to go...... Please don't hesitate to create issues or suggestions. All issues will be reviewed and answered as soon as possible.
## Contributors Behind the scene

Kishore Ithadi
Ayush Thakur
Baresh Bairam
Shilpa Tippana

## Contributions

Contributions are always welcome!!! Please fork and create pull request(don't forget to follow the basic standards)... If you have any further questions let me know.















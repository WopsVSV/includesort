# includesort
Sorts all includes in a C/C++ project.
  
### Documentation
You can either run includesort through command-line or by a normal run.  
If ran normally, the app will use the relative path (current executing directory) and sort non-recursively every source file.  
The supported source file extensions are:
  * .cpp
  * .cc
  * .c
  * .h
  * .hpp
  
### Arguments
  * -h			> Displays the help file  
  * -p <path>	> Uses an alternative, specified path for executing  
  * -r			> Sorts recursively (in subdirectories as well)  
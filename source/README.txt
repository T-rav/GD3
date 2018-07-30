This application is used to build KPIs from git commit history.

It is a dotnet core console application.
To use the application make sure dotnet 2.1 is installed.

The application performs analysis in two modes : 
 + ranged - specify a start and end date to generate stats within
  == Options ==
    -p : Path to git repo
    -i : Ignore files with these patterns in the path. Use a ; to separate multiple values
    -h : Working hours in a week
    -d : Working days in a week
    -s : Start date of analysis
    -e : End date of analysis
    -b : Branch to analyze -> Optional, if not specified uses master.
  == Examples ==
    Analyzing master : dotnet Analyzer.dll ranged -p C:\Systems\pdf-poc -i node_modules;packages;.gitignore  -h 32 -d 4 -s 2018-07-23 -e 2018-07-27 
    Analyzing branch : dotnet Analyzer.dll ranged -p C:\Systems\pdf-poc -b origin/MyBranch -i node_modules;packages;.gitignore  -h 32 -d 4 -s 2018-07-23 -e 2018-07-27

 + full - use the entire history to generate stats
  == Options ==
    -p : Path to git repo
    -i : Ignore files with these patterns in the path. Use a ; to separate multiple values
    -h : Working hours in a week
    -d : Working days in a week
    -b : Branch to analyze -> Optional, if not specified uses master.
  == Examples ==
    Analyzing master : dotnet Analyzer.dll full -p C:\Systems\pdf-poc -i node_modules;packages;.gitignore  -h 32 -d 4
    Analyzing branch : dotnet Analyzer.dll full -p C:\Systems\pdf-poc -b origin/MyBranch -i node_modules;packages;.gitignore  -h 32 -d 4

Stats Explained:
There are individual and team stats.

  == Individual ==
  - Period Active Days 		-> How many days in the given period did the developer commit at least once
  - Active Days Per Week 	-> How many days in a week did the developer commit at least once
  - Commits / Day		-> On average how many commits per day did the developer make
  - Lines of Change Per Hour	-> How many lines did the developer change (add + delete) per hour
  - Impact			-> An approximation of cognitive load (New edits v Old edits factoring in number of edit locations)
  - Risk Factor			-> Lines of Change Per Hour / Commits Per Day (Are they making big changes without frequent integration. Are there going to be many merge conflicts?)
  - Lines Added			-> Number of lines added 
  - Lines Removed		-> Number of lines removed
  - Churn			-> Number of lines added v number removed
  - Rtt100			-> How long for the developer to add/remove/edit 100 lines of code (Think of this like the 100m dash for developers)
  - Ptt100			-> How long for the developer to add/edit 100 lines of code (A more refined 100m dash) - How long to make code that sticks.
  - Dtt100			-> Difference between raw (Rtt100) and production (Ptt100) - Big difference indicates possible lack of design considerations or that they in an experimental phase of the project. (they learning)

  == Team ==
  - Total Commits		-> Number of commits per day for the team
  - Active Developers		-> Number of developers committing per day
  - Velocity			-> How much forward progress is the team making. - Total Commits / Active Developers (More of a trending indicator, if high flux might be poor/changing requirements)




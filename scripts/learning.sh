#!/bin/sh

#P1] Dev | Active Days | Commits / Day

function calculateWorkingDays(){
    local startDate="2018-06-25"
    local today=$(date +%Y-%m-%d)
    local rawWorkingDays=$(( ($(date --date="$today" +%s) - $(date --date="$startDate" +%s) )/(60*60*24) ))
    local weekends=$(expr $rawWorkingDays / 5)
    local weekendDays=$(($weekends * 2))
    local totalWorkingDays=$(expr $rawWorkingDays - $weekendDays)
    
    echo $totalWorkingDays
}

function pad(){
    spaces=$[$2-$1]
    index=0
    while [ $index -lt $spaces ]; do
        echo -n " "
        index=$[$index+1]
    done
}

# todo : check for path and accept a from date argument
gitDirctory=$1
currentDirctory=$(pwd)
totalWorkingDays=$(calculateWorkingDays)

dataPath="$currentDirctory/data.txt"
rawCommitStats="$currentDirctory/rawCommitStats.txt"
individualCommitStats="$currentDirctory/individualCommitStats.txt"
activeDaysPerDeveloper="$currentDirctory/activeDaysPerDeveloper.txt"

teamTotalCommits=0

cd $gitDirctory

# fetch raw data
git log --all --numstat --date=short --pretty=format:'--%h--%ad--%aN' --no-renames > $dataPath

# list author with date and commits per day e.g  '8 2018-06-25 T-rav'
grep -- -- < $dataPath | awk -F'--' '{print $3" "$4}' | sort | uniq -c > $rawCommitStats

# total team commits
teamTotalCommits=$(awk '{s+=$1} END {print s}' $rawCommitStats)

# commits per person Person\tTotal Commits\tCommits Per Working Day
awk '{ arr[$3]+=$1 } END {for (key in arr) printf("%s\t%s\t%s\n", key, arr[key], arr[key]/$totalWorkingDays)}' $rawCommitStats  | sort +0n -1 > $individualCommitStats

# get active days per developer
grep -- -- < $dataPath | awk -F'--' '{print $3" "$4}' | sort | uniq | cut -d' ' -f2 | sort | uniq -c > $activeDaysPerDeveloper

# --- Print Dashboard ---
echo -e "\e[93mGD3 Stats for 2018-06-25 - $(date +%Y-%m-%d)\e[39m"
echo "---------------------------------------------------------"
echo -e "Developer      | Active Days   | Commits / Day | Impact" 
echo  "---------------------------------------------------------"

while read line
do
    developer=$(echo $line | cut -d' ' -f2)
    activeDays=$(echo $line | cut -d' ' -f1)
    commitsPerWorkingDay=$(grep "$developer" < $individualCommitStats | awk -F'\t' '{print $3}' )
    echo -n "$developer"
    pad ${#developer} 17 
    echo -n "$activeDays"
    pad ${#activeDays} 16
    echo -n "$commitsPerWorkingDay"
    pad ${#commitsPerWorkingDay} 16
    echo "todo"
done < $activeDaysPerDeveloper

# clean up
rm $dataPath $rawCommitStats $individualCommitStats $activeDaysPerDeveloper
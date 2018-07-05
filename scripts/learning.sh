#!/bin/sh

function calculateWorkingDays(){
    local startDate="2018-06-25"
    local today=$(date +%Y-%m-%d)
    local rawWorkingDays=$(( ($(date --date="$today" +%s) - $(date --date="$startDate" +%s) )/(60*60*24) ))
    local weekends=$(expr $rawWorkingDays / 7)
    local weekendDays=$(($weekends * 2))
    local result=$(expr $rawWorkingDays - $weekendDays)
    
    echo $result
}

function pad(){
    spaces=$[$2-$1]
    index=0
    while [ $index -lt $spaces ]; do
        echo -n " "
        index=$[$index+1]
    done
}

function ceiling () {
  DIVIDEND=${1}
  DIVISOR=${2}
  RESULT=$(( ( ( ${DIVIDEND} - ( ${DIVIDEND} % ${DIVISOR}) )/${DIVISOR} ) + 1 ))
  echo $RESULT
}

if [ "$#" -lt 1 ]; then
    echo "Illegal number of parameters - please pass the git repository path!"
    exit
fi
# ---------------- end functions ----------------

gitDirctory=$1
currentDirctory=$(pwd)
totalWorkingDays=$(calculateWorkingDays)
totalWorkingWeeks=$(ceiling $totalWorkingDays 5)

dataPath="$currentDirctory/data.txt"
rawCommitStats="$currentDirctory/rawCommitStats.txt"
individualCommitStats="$currentDirctory/individualCommitStats.txt"
activeDaysPerDeveloper="$currentDirctory/activeDaysPerDeveloper.txt"

cd $gitDirctory

# fetch raw data
git log --all --numstat --date=short --pretty=format:'--%h--%ad--%aN' --no-renames > $dataPath

# list author with date and commits per day e.g  '8 2018-06-25 T-rav'
grep -- -- < $dataPath | awk -F'--' '{print $3" "$4}' | sort | uniq -c > $rawCommitStats

# commits per person Person\tTotal Commits\tCommits Per Working Day
awk -v days="$totalWorkingDays" '{ arr[$3]+=$1 } END {for (key in arr) printf("%s\t%s\t%s\n", key, arr[key], arr[key]/days)}' $rawCommitStats  | sort +0n -1 > $individualCommitStats

# get active days per developer
grep -- -- < $dataPath | awk -F'--' '{print $3" "$4}' | sort | uniq | cut -d' ' -f2 | sort | uniq -c > $activeDaysPerDeveloper

# --- Print Dashboard ---
echo -e "\e[96mGD3 Stats - v0.9.2\e[39m"
echo -e "\e[93mFor 2018-06-25 - $(date +%Y-%m-%d)\e[39m"
echo "-----------------------------------------------------------------------------------"
echo -e "Developer      | Active Days | Active Days Per Week* | Commits / Day | Impact" 
echo  "---------------------------------------------------------------------------------"

while read line
do
    developer=$(echo $line | cut -d' ' -f2)
    activeDays=$(echo $line | cut -d' ' -f1)
    activeDaysPerWeek=$(( $activeDays*10 / $totalWorkingWeeks*10))
    deciamlActiveDaysPerWeek=$(echo $activeDaysPerWeek | sed 's/..$/.&/')
    commitsPerWorkingDay=$(grep "$developer" < $individualCommitStats | awk -F'\t' '{print $3}' )
    
    echo -n "$developer"
    pad ${#developer} 17 

    echo -n "$activeDays"
    pad ${#activeDays} 14

    echo -n "$deciamlActiveDaysPerWeek"
    pad ${#deciamlActiveDaysPerWeek} 24

    echo -n "$commitsPerWorkingDay"
    pad ${#commitsPerWorkingDay} 16

    echo "todo"
done < $activeDaysPerDeveloper

echo ""
echo "* Average is 3.2 days per week"

# clean up
rm $dataPath $rawCommitStats $individualCommitStats $activeDaysPerDeveloper
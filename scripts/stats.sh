#!/bin/sh

version="0.9.6.3"
startDate="NA"
endDate="NA"
function calculateWorkingDays(){
    local startDate=$1
    local endDate=$2
    local rawWorkingDays=$(( ($(date --date="$endDate" +%s) - $(date --date="$startDate" +%s) )/(60*60*24) ))
    local weekends=$(expr $rawWorkingDays / 7)
    local weekendDays=$(($weekends * 2))
    local result=$(expr $rawWorkingDays - $weekendDays)
    echo $result
}

function print(){
    line=$1
    echo -n "$line"
    pad $(echo $line | wc -c) $2 
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

function removeDecimal(){
    echo $(echo $1 |  sed 's/\.//g' | sed 's/^0*//')
}

function addDecimal(){
    formatedNumber=$(echo $1 | sed 's/..$/.&/')
    echo $(printf "%.2f" $formatedNumber)
}

function printDeveloperDashboard(){
    echo "---------------------------------------------------------------------------------------------------------"
    echo -e "\e[7mIndividual Developer Stats\e[27m"
    echo "---------------------------------------------------------------------------------------------------------"
    echo "Developer      | Period Active Days | Active Days Per Week | Commits / Day | Efficiency | Impact | PTT100" 
    echo "---------------------------------------------------------------------------------------------------------"

    local rowCount=0
    local teamActiveDays=0
    local teamActiveDaysPerWeek=0
    local teamCommitsPerDay=0
    while read line
    do
        local developer=$(echo $line | cut -d' ' -f2)
        local activeDays=$(echo $line | cut -d' ' -f1)
        local activeDaysPerWeek=$(( $activeDays*10 / $totalWorkingWeeks*10))
        local avgActiveDaysPerWeek=$(echo $activeDaysPerWeek | sed 's/..$/.&/')
        local commitsPerWorkingDay=$(grep "$developer" < $individualCommitStats | awk -F'\t' '{print $3}' | grep -o -P '.{0,4}\..{0,2}')
        
        print $developer 18
        print $activeDays 22
        print $(printf "%.2f" $avgActiveDaysPerWeek) 24
        print $(printf "%.2f" $commitsPerWorkingDay) 17
        print "todo" 14
        print "todo" 10
        echo "todo"

        rowCount=$(($rowCount+1))
        teamActiveDays=$(($teamActiveDays+$activeDays))
        teamActiveDaysPerWeek=$(($teamActiveDaysPerWeek+$(removeDecimal $avgActiveDaysPerWeek)))
        teamCommitsPerDay=$(($teamCommitsPerDay+$(removeDecimal $commitsPerWorkingDay)))
    done < $activeDaysPerDeveloper
    echo "---------------------------------------------------------------------------------------------------------"

    avgActiveDays=$(($teamActiveDays / $rowCount))
    avgDaysPerWeek=$(($teamActiveDaysPerWeek / $rowCount))
    avgCommitsPerDay=$(($teamCommitsPerDay / $rowCount))

    print "Averages" 18
    print "$(echo $(addDecimal $avgActiveDays))*" 22
    print "$(echo $(addDecimal $avgDaysPerWeek))**" 24
    print $(echo $(addDecimal $avgCommitsPerDay)) 17
    print "?***" 26
    print "?****" 21
    print "?*****" 0
    echo ""
    echo "---------------------------------------------------------------------------------------------------------"
    echo "* of $totalWorkingDays possible days"
    echo "** Global average is 3.2 days per week"
    echo "*** % of code written that was productive - it was not re-writen or deleted later."
    echo "**** Congative load carried when contributing."
    echo "***** Time taken to write 100 productive lines of code"
    echo "^^ A big missing piece is what percentage of hot-spot technical debt is this developer responsible for. No good being effiecent and impactful if all you do is leave a mess!"
}

function printTeamDashboard(){
    echo "---------------------------------------------------------------------------------------------------------"
    echo -e "\e[7mTeam Stats\e[27m"
    echo "---------------------------------------------------------------------------------------------------------"
    echo "Date           | Total Commits | Active Developers | Velocity | Efficiency " 
    echo "---------------------------------------------------------------------------------------------------------"
    local totalVelocity=0
    local lineCount=0
    while read line
    do
        local date=$(echo $line | cut -d' ' -f1)
        local totalCommits=$(echo $line | cut -d' ' -f2)
        local activeDevelopers=$(echo $line | cut -d' ' -f3)
        local velocity=$(echo $line | cut -d' ' -f4)
        print $date 18
        print $totalCommits 17
        print $activeDevelopers 21
        print $(printf "%.2f" $velocity) 12
        echo "todo"
        totalVelocity=$(($totalVelocity+$(removeDecimal $velocity)))
        lineCount=$(($lineCount+1))
    done < $teamCommitStats
    echo "---------------------------------------------------------------------------------------------------------"
    print "Averages" 54
    print "$(echo $( addDecimal $(( $totalVelocity / $lineCount )) ) )*"  12
    print "?**"
    echo ""
    echo "---------------------------------------------------------------------------------------------------------"
    echo "* Unlike traditional Agile velocity, this velocity metric is a bellwether that filters out item size "
    echo "and team size and is relatively agnostic about individual contributions."
    echo "The metric exist to anwser 'How is the flow of work in engineering?'"
    echo "** % of code written that was productive - it was not re-writen or deleted later."
}

# ---------------- end functions ----------------
if [ "$#" -lt 1 ]; then
    echo "Illegal number of parameters - please pass the git repository path!"
    exit
fi

developerToFilter="T-ravx"
gitDirctory=$1
currentDirctory=$(pwd)

dataPath="$currentDirctory/data.txt"
rawCommitStats="$currentDirctory/rawCommitStats.txt"
individualCommitStats="$currentDirctory/individualCommitStats.txt"
activeDaysPerDeveloper="$currentDirctory/activeDaysPerDeveloper.txt"
teamCommitStats="$currentDirctory/teamCommitStats.txt"

cd $gitDirctory

#calculate start and end dates based on activity
startDate=$(git log --date=short --pretty=format:'%ad' --no-renames | tail -n1)
endDate=$(git log --date=short --pretty=format:'%ad' --no-renames | head -n1)
totalWorkingDays=$(calculateWorkingDays $startDate $endDate)
totalWorkingWeeks=$(ceiling $totalWorkingDays 5)

# fetch raw data
git log --all --numstat --date=short --pretty=format:'--%h--%ad--%aN' --no-renames > $dataPath

# list author with date and commits per day e.g  '8 2018-06-25 T-rav'
# filter out my commits
grep -- -- < $dataPath | awk -F'--' '{print $3" "$4}' | sort | uniq -c | grep -v $developerToFilter > $rawCommitStats

# commits per person [Person\tTotal Commits\tCommits Per Working Day]
awk -v days="$totalWorkingDays" '{ commits[$3]+=$1 } END {for (key in commits) printf("%s\t%s\t%.2f\n", key, commits[key], commits[key]/days)}' $rawCommitStats  | sort +0n -1 > $individualCommitStats

# commits per day [Day\tCommits]
awk '{ commits[$2]+=$1; developers[$2]+=1 } END {for (key in commits) printf("%s\t%s\t%s\t%.2f\n", key, commits[key], developers[key], commits[key]/developers[key])}' $rawCommitStats  | sort +0n -1 > $teamCommitStats

# get active days per developer and filter out my commits
grep -- -- < $dataPath | grep -v $developerToFilter | awk -F'--' '{print $3" "$4}' | sort | uniq | cut -d' ' -f2 | sort | uniq -c > $activeDaysPerDeveloper

# --- Print Dashboards ---
echo -en "\e[96mGD3 Stats - v$version\e[39m"
echo -e " - \e[93mfor period $startDate - $endDate\e[39m"

printDeveloperDashboard
printTeamDashboard
# --- End Print Dashboards ---

# todo : print team stats scoped to sprint* churn can be calculated with x-ray tool
# team titans and code instances

# clean up
rm $dataPath $rawCommitStats $individualCommitStats $activeDaysPerDeveloper $teamCommitStats
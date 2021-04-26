pipeline {
  agent any
  environment {
    PROJECTPATH = "${WORKSPACE}\\CollegeProject\\CollegeProject.csproj"
    PROJECTSOLUTIONPATH = "${WORKSPACE}\\CollegeProject.sln"
    TESTPROJECTPATH = "${WORKSPACE}\\CollegeProject.Test\\CollegeProject.Test.csproj"
    TESTRESULTSPATH = "${WORKSPACE}\\TestResults\\"
    CODECOVERAGE = 'C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\TestAgent\\Team Tools\\Dynamic Code Coverage Tools'
    dotnet = 'C:\\Program Files (x86)\\dotnet\\'
  }
  triggers {
    pollSCM '*/1 * * * *'
  }
  stages {
    stage('Checkout') {
      steps {
        git url: 'https://github.com/helenmgriffin/CollegeProject.git', branch: 'master'
      }
    }
    stage('Restore Packages') {
      steps {
        bat "dotnet restore ${PROJECTSOLUTIONPATH}"
      }
    }
    stage('Build') {
      steps {
        bat "dotnet clean ${PROJECTSOLUTIONPATH}"
        bat "dotnet build ${PROJECTSOLUTIONPATH} --configuration Release"
      }
    }
    stage('Test: Unit Test') {
      steps {
        script {
          echo 'Unit Testing'
          bat "dotnet test ${TESTPROJECTPATH} --logger trx --results-directory ${TESTRESULTSPATH}"
        }
      }
    }
    stage('Test: Publish Unit Test Report') {
      steps {
        echo "publish the MSTest test Report"
        script {

          mstest testResultsFile: "**/*.trx"
        }
      }
    }
    stage('Publish to Local Folder') {
      steps {
        script {
          bat "dotnet publish ${PROJECTPATH} -p:PublishTrimmed=true -c Release -r win-x64 -o C:\\CollegeProjectWebSite\\Published${env.BUILD_NUMBER}"
          zip zipFile: "CollegeProject.zip", archive: true, dir: "C:\\CollegeProjectWebSite\\Published${env.BUILD_NUMBER}", overwrite: true
          archiveArtifacts artifacts: "CollegeProject.zip", fingerprint: true
        }
      }
    }
    stage('Build + SonarQube Analysis') {
      steps {
        script {
          def sqScannerMsBuildHome = tool 'SonarScanner for MSBuild'
          withSonarQubeEnv('Sonar8.7') {
            bat "dotnet ${sqScannerMsBuildHome}\\SonarScanner.MSBuild.dll begin /k:helenmgriffin.collegeproject /d:sonar.cs.vscoveragexml.reportsPaths=${WORKSPACE}\\TestResults\\xmlresults${env.BUILD_NUMBER}.coveragexml"
            bat "dotnet add ${TESTPROJECTPATH} package JUnitTestLogger --version 1.1.0"
            echo 'Convert .codecoverage files to coveragexml. so the code coverage can be read by SoanrQube'
            bat "dotnet test ${TESTPROJECTPATH} --logger \"junit;LogFilePath=\"${WORKSPACE}\"/TestResults/1.0.0.\"${env.BUILD_NUMBER}\"/results.xml\" --configuration release --collect \"Code coverage\""
            powershell ''
            '
            $destinationFolder = \"$env:WORKSPACE/TestResults\"
            if (!(Test - Path - path $destinationFolder)) {
              New - Item $destinationFolder - Type Directory
            }
            $file = Get - ChildItem - Path\ "$env:WORKSPACE/CollegeProject.Test/TestResults/*/*.coverage\"
            $file | Rename - Item - NewName testcoverage.coverage
            $renamedFile = Get - ChildItem - Path\ "$env:WORKSPACE/CollegeProject.Test/TestResults/*/*.coverage\"
            Copy - Item $renamedFile - Destination $destinationFolder ''
            '    
            bat "\"${CODECOVERAGE}\\CodeCoverage.exe\" analyze  /output:${WORKSPACE}\\TestResults\\xmlresults${env.BUILD_NUMBER}.coveragexml  ${WORKSPACE}\\TestResults\\testcoverage.coverage"
            bat "dotnet ${sqScannerMsBuildHome}\\SonarScanner.MSBuild.dll end"
          }
        }
      }
    }
  }
  post {
    always {
      //cleanWs()
      script {
        emailext body: "${currentBuild.currentResult}: Job   ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}",
          recipientProviders: [
            [$class: 'DevelopersRecipientProvider'],
            [$class: 'RequesterRecipientProvider']
          ],
          subject: "Jenkins Build ${currentBuild.currentResult}: Job ${env.JOB_NAME}"
      }
    }
  }
}

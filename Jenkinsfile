pipeline{
    agent any
    environment {
        PROJECTPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject\\CollegeProject.csproj'
        PROJECTSOLUTIONPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln'
        TESTPROJECTPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.Test\\CollegeProject.Test.csproj'
        dotnet ='C:\\Program Files (x86)\\dotnet\\'
        }
        triggers {
        pollSCM '*/1 * * * *'
    }
    stages{
      stage('Checkout') {
           steps {
             git url: 'https://github.com/helenmgriffin/CollegeProject.git', branch: 'master'
             }
      }
      stage('Restore Packages'){
           steps{
              bat "dotnet restore ${PROJECTSOLUTIONPATH}"
            }
      }
      stage('Build'){
           steps{
              bat "dotnet clean ${PROJECTSOLUTIONPATH}"
              bat "dotnet build ${PROJECTSOLUTIONPATH} --configuration Release"
            }
       }
        stage('Test: Unit Test'){
           steps {
             bat "dotnet test ${TESTPROJECTPATH} -l:trx;LogFileName=${WORKSPACE}\\TestResults\\TestOutput${env.BUILD_NUMBER}.trx"
             }
        }
        stage('Test: Publish Unit Test Report'){
           steps {
               echo "publish the MSTest test Report"
                 script{
                     mstest testResultsFile: 'TestResults/*.trx'
                 }
           }
        }
        stage('Publish to Local Folder'){
             steps{
                 script
                 {
                   bat "dotnet publish ${PROJECTPATH} -p:PublishTrimmed=true -c Release -r win-x64 -o published"
                   zip zipFile: "CollegeProject.zip", archive: false, dir: 'published', overwrite: true
                   archiveArtifacts artifacts: "Archive\\CollegeProject.zip", fingerprint: true
                 }
             }
        }
        /*stage('Build + SonarQube Analysis') {
            steps{
                script {
                    def sqScannerMsBuildHome = tool 'SonarScanner for MSBuild'
                    withSonarQubeEnv('Sonar8.7') {
                      bat "${sqScannerMsBuildHome}\\SonarScanner.MSBuild.exe begin /k:helenmgriffin.collegeproject"
                      bat 'dotnet build'
                      bat "${sqScannerMsBuildHome}\\SonarScanner.MSBuild.exe end"
                    }
                }
            }
        }*/
    }
    post{
      always{
          script
          {
            emailext body: "${currentBuild.currentResult}: Job   ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}",
            recipientProviders: [[$class: 'DevelopersRecipientProvider'], [$class: 'RequesterRecipientProvider']], 
            subject: "Jenkins Build ${currentBuild.currentResult}: Job ${env.JOB_NAME}"
          }
      }
    }
 }

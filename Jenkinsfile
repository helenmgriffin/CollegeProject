pipeline{
    agent any
    
    environment {
        dotnet ='C:\\Program Files (x86)\\dotnet\\'
        }
        
    triggers {
        pollSCM '* * * * *'
    }
    stages{
      stage('Checkout') {
           steps {
             git url: 'https://github.com/helenmgriffin/CollegeProject.git', branch: 'master'
             }
      }
      stage('Restore Packages'){
           steps{
              bat "dotnet restore C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln"
            }
      }
      stage('Clean'){
            steps{
                bat "dotnet clean C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln"
             }
      }
      stage('Build'){
           steps{
              bat "dotnet build C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln --configuration Release"
            }
       }
       stage('Test: Unit Test'){
           steps {
             bat "dotnet test C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.Test\\CollegeProject.Test.csproj"
             }
       }
       stage('Publish'){
             steps{
               bat "dotnet publish C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln  -c Release -o published"
             }
        }
    }
    post{
      always{
        emailext body: "${currentBuild.currentResult}: Job   ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}",
        recipientProviders: [[$class: 'DevelopersRecipientProvider'], [$class: 'RequesterRecipientProvider']],
        subject: "Jenkins Build ${currentBuild.currentResult}: Job ${env.JOB_NAME}"
        }
      }
 }

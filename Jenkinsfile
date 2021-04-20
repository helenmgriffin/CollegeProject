pipeline{
    agent any
    environment {
        PROJECTPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.sln'
        TESTPROJECTPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.Test\\CollegeProject.Test.csproj'
        TESTREPORTPATH = 'C:\\Users\\JenkinsServiceUser\\AppData\\Local\\Jenkins\\.jenkins\\workspace\\CollegeProject-Pipeline\\CollegeProject.Test\\CollegeProject.Test\\TestResults\\'
        dotnet ='C:\\Program Files (x86)\\dotnet\\'
        }
        triggers {
        pollSCM '*/15 * * * *'
    }
    stages{
      stage('Checkout') {
           steps { 
             git url: 'https://github.com/helenmgriffin/CollegeProject.git', branch: 'master'
             }
      }
      stage('Restore Packages'){
           steps{ 
              bat "dotnet restore ${PROJECTPATH}"
            } 
      }
      stage('Clean'){
            steps{
                bat "dotnet clean ${PROJECTPATH}"
             }
      }
      stage('Build'){
           steps{
              bat "dotnet build ${PROJECTPATH} --configuration Release"
            }
       }
       stage('Test: Unit Test'){
           steps {
             bat "dotnet test ${TESTPROJECTPATH} -l:trx;LogFileName=TestOutput${env.BUILD_NUMBER}.xml"
             }
       }
       //stage('Test: Publish Test Report'){
       //    steps { 
             //nunit testResultsPattern: "TestOutput${env.BUILD_NUMBER}.xml"
             //step([$class: 'NUnitPublisher', testResultsPattern: "TestOutput${env.BUILD_NUMBER}.xml", debug: false, keepJUnitReports: true, skipJUnitArchiver:false, failIfNoResults: true])  
        //   }
       // }
       stage('Publish'){
             steps{
               bat "dotnet publish ${PROJECTPATH}  -c Release -o published"
               //archiveArtifacts artifacts: 'published/*.zip', followSymlinks: false, onlyIfSuccessful: true
             }
        }
        stage('Docker Image: Build')
        {
            steps
            {
                bat 'docker build -t helenmgriffin/collegeproject:latest .'
            }
        }
        stage('Docker Image: Push to Docker Hub')
        {
            steps
            {                
                withCredentials([string(credentialsId: 'docker-pwd', variable: 'dockerHubPwd')]) {
                    bat "docker login -u helenmgriffin -p ${dockerHubPwd}" 
                }
                bat 'docker push helenmgriffin/collegeproject:latest'
            }
        }
        stage('Run CollegeProject-Pipeline-EC2 To Provision AWS EC2 Instance')
        {
            steps
            {
                script
                {
                    build job: 'CollegeProject-Pipeline-EC2', parameters: [string(name: 'AWS_DEFAULT_REGION', value: 'eu-west-1'), credentials(description: 'AWS ACCESS KEY ID is used to set the default region for the environment variables', name: 'AWS_ACCESS_KEY_ID', value: 'aws_access_key_id'), credentials(description: 'AWS SECRET ACCESS KEY is used to set the default region for the environment variables', name: 'AWS_SECRET_ACCESS_KEY', value: 'aws_secret_access_key')]
                }
            }
        }
    }
    post{ 
      always{
        nunit testResultsPattern: 'TestOutput${env.BUILD_NUMBER}.xml'
        emailext body: "${currentBuild.currentResult}: Job   ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}",
        recipientProviders: [[$class: 'DevelopersRecipientProvider'], [$class: 'RequesterRecipientProvider']],
        subject: "Jenkins Build ${currentBuild.currentResult}: Job ${env.JOB_NAME}"
        }
      }
 }

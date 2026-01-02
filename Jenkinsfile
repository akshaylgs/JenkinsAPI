pipeline {
    agent any

    environment {
        PROJECT_PATH = "JenkinsAPI/JenkinsApi.csproj"
        PUBLISH_PATH = "C:\\Users\\Admin\\Desktop\\JenkinsAPI"
    }

    stages {

        stage('Checkout Source') {
            steps {
                checkout scm
            }
        }

        stage('Verify .NET SDK') {
            steps {
                bat 'dotnet --version'
            }
        }

        stage('Restore Packages') {
            steps {
                bat "dotnet restore %PROJECT_PATH%"
            }
        }

        stage('Build') {
            steps {
                bat "dotnet build %PROJECT_PATH% -c Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                bat """
                dotnet publish %PROJECT_PATH% ^
                -c Release ^
                -o %PUBLISH_PATH% ^
                --no-build
                """
            }
        }

        stage('Deploy to IIS') {
            steps {
                bat """
                echo Restarting IIS...
                iisreset
                """
            }
        }
    }

    post {
        success {
            echo '✅ Deployment completed successfully'
        }
        failure {
            echo '❌ Deployment failed'
        }
    }
}

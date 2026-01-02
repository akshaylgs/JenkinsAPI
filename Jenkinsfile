pipeline {
    agent any

    environment {
        PROJECT_PATH = "D:\\Akshay\\Learning_Projects\\Backend\\JenkinsAPI\\JenkinsAPI\\JenkinsApi.csproj"
        PUBLISH_PATH = "C:\\Users\\Admin\\Desktop\\JenkinsAPI"
    }

    stages {

        stage('Checkout Source') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/akshaylgs/JenkinsAPI.git'
            }
        }

        stage('Verify .NET SDK') {
            steps {
                bat 'dotnet --version'
            }
        }

        stage('Restore') {
            steps {
                bat "dotnet restore ${PROJECT_PATH}"
            }
        }

        stage('Build') {
            steps {
                bat "dotnet build ${PROJECT_PATH} --configuration Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                bat """
                dotnet publish ${PROJECT_PATH} ^
                --configuration Release ^
                --output ${PUBLISH_PATH} ^
                --no-build
                """
            }
        }

        stage('Restart IIS') {
            steps {
                bat "iisreset"
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

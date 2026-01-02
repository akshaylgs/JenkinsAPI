pipeline {
    agent any

    environment {
        PROJECT_PATH = "C:\ProgramData\Jenkins\.jenkins\workspace\JenkinsAPI"
        IIS_SITE_PATH = "C:\\inetpub\\wwwroot\\JenkinsAPI"
        APP_POOL = "JenkinsAPI"
    }

    stages {

        stage('Verify .NET SDK') {
            steps {
                bat 'dotnet --version'
            }
        }

        stage('Restore') {
            steps {
                bat "dotnet restore %PROJECT_PATH%"
            }
        }

        stage('Build') {
            steps {
                bat "dotnet build %PROJECT_PATH% --configuration Release --no-restore"
            }
        }

        stage('Publish') {
            steps {
                bat """
                dotnet publish %PROJECT_PATH% ^
                --configuration Release ^
                --output %IIS_SITE_PATH% ^
                --no-build
                """
            }
        }

        stage('Restart IIS App Pool') {
            steps {
                bat """
                %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:%APP_POOL%
                %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:%APP_POOL%
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

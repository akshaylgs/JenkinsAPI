pipeline {
    agent { label 'iis' }

    environment {
        PROJECT_PATH  = "JenkinsAPI.csproj"
        IIS_SITE_PATH = "C:\\inetpub\\wwwroot\\JenkinsAPI"
        TEMP_PUBLISH  = "C:\\temp\\jenkins_publish"
        BACKUP_ROOT   = "C:\\inetpub\\backup\\JenkinsAPI"
        APP_POOL      = "JenkinsAPI"
		IGNORE_FILES = "appsettings.Production.json web.config appsettings.json"
		/* IGNORE_DIRS  = "Uploads Logs" */
    }

    options {
        timestamps()
    }

    stages {

        /* ===================== CI (ALL BRANCHES) ===================== */

        stage('Verify .NET SDK') {
            steps {
                bat 'dotnet --version'
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release --no-restore'
            }
        }

        /* ===================== CD (RELEASE ONLY) ===================== */

        stage('Publish to Temp Folder') {
            when { branch 'release' }
            steps {
                bat '''
                if exist "%TEMP_PUBLISH%" rmdir /s /q "%TEMP_PUBLISH%"
                dotnet publish --configuration Release --output "%TEMP_PUBLISH%" --no-build
                '''
            }
        }

        stage('Restart IIS App Pool') {
            when { branch 'release' }
            steps {
                bat '''
                %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:%APP_POOL%
                %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:%APP_POOL%
                '''
            }
        }
    }

    post {
        success {
            echo '✅ Pipeline completed successfully'
        }
        failure {
            echo '❌ Pipeline failed'
        }
    }
}

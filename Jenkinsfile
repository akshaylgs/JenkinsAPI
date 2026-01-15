pipeline {
    agent any

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

        stage('Create Versioned Backup') {
            when { branch 'release' }
            steps {
                powershell '''
                $timestamp = Get-Date -Format "yyyyMMdd_HHmm"
                $backupDir = "$env:BACKUP_ROOT\\backup_$timestamp"
                New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
                Copy-Item "$env:IIS_SITE_PATH\\*" $backupDir -Recurse -Force
                '''
            }
        }

        stage('Publish to Temp Folder') {
            when { branch 'release' }
            steps {
                bat '''
                if exist "%TEMP_PUBLISH%" rmdir /s /q "%TEMP_PUBLISH%"
                dotnet publish --configuration Release --output "%TEMP_PUBLISH%" --no-build
                '''
            }
        }

        stage('Stop IIS App Pool') {
			when { branch 'release' }
			steps {
				bat '%windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:%APP_POOL%'
			}
		}

		stage('Sync Files to IIS (Correct)') {
			when { branch 'release' }
			steps {
				script {
					def rc = bat(
						script: '''
						robocopy "%TEMP_PUBLISH%" "%IIS_SITE_PATH%" /E /XO ^
						/XF %IGNORE_FILES% ^
						/R:2 /W:2
						''',
						returnStatus: true
					)

					// Robocopy success codes: 0–7
					if (rc >= 8) {
						error "❌ Robocopy failed with exit code ${rc}"
					} else {
						echo "✅ Robocopy completed successfully (exit code ${rc})"
					}
				}
			}
		}

		stage('Start IIS App Pool') {
			when { branch 'release' }
			steps {
				bat '%windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:%APP_POOL%'
			}
		}


        stage('Approve IIS Restart') {
            when { branch 'release' }
            steps {
                timeout(time: 30, unit: 'MINUTES') {
                    input message: 'Approve RELEASE deployment?', ok: 'Approve'
                }
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

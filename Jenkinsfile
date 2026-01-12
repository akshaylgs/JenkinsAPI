pipeline {
    agent any

    environment {
        PROJECT_PATH  = "D:\\Akshay\\Learning_Projects\\Backend\\JenkinsAPI\\JenkinsAPI\\JenkinsApi.csproj"
        IIS_SITE_PATH = "C:\\inetpub\\wwwroot\\JenkinsAPI"
        TEMP_PUBLISH  = "C:\\temp\\jenkins_publish"
        BACKUP_ROOT   = "C:\\inetpub\\backup\\JenkinsAPI"
        APP_POOL      = "JenkinsAPI"
    }

    options {
        timestamps()
    }

    stages {

        /* ===================== CI ===================== */

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

        /* ===================== CD (RELEASE ONLY) ===================== */

        stage('Create Versioned Backup') {
            when {
                branch 'release'
            }
            steps {
                powershell '''
                $timestamp = Get-Date -Format "yyyyMMdd_HHmm"
                $backupDir = "$env:BACKUP_ROOT\\backup_$timestamp"

                New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
                Copy-Item "$env:IIS_SITE_PATH\\*" $backupDir -Recurse -Force

                Write-Host "Backup created: $backupDir"
                '''
            }
        }

        stage('Publish to Temp Folder') {
            when {
                branch 'release'
            }
            steps {
                bat """
                if exist "%TEMP_PUBLISH%" rmdir /s /q "%TEMP_PUBLISH%"
                dotnet publish %PROJECT_PATH% ^
                --configuration Release ^
                --output %TEMP_PUBLISH% ^
                --no-build
                """
            }
        }

        stage('Sync Files to IIS (Optimized)') {
            when {
                branch 'release'
            }
            steps {
                bat """
                robocopy "%TEMP_PUBLISH%" "%IIS_SITE_PATH%" /E /XO /R:2 /W:2
                """
            }
        }

        stage('Approve IIS Restart') {
			when {
				branch 'release'
			}
			steps {
				timeout(time: 30, unit: 'MINUTES') {
					input(
						message: 'Approve RELEASE deployment and IIS restart?',
						ok: 'Approve Deployment'
					)
				}
			}
		}

        stage('Restart IIS App Pool') {
            when {
                branch 'release'
            }
            steps {
                bat """
                %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:%APP_POOL%
                %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:%APP_POOL%
                """
            }
        }

        stage('Cleanup Old Backups (Keep Last 5)') {
            when {
                branch 'release'
            }
            steps {
                powershell '''
                Get-ChildItem "$env:BACKUP_ROOT" -Directory |
                Sort-Object LastWriteTime -Descending |
                Select-Object -Skip 5 |
                Remove-Item -Recurse -Force
                '''
            }
        }
    }

    /* ===================== ROLLBACK ===================== */

    post {
        failure {
            echo '❌ Deployment failed. Starting rollback...'

            powershell '''
            $latestBackup = Get-ChildItem "$env:BACKUP_ROOT" -Directory |
                            Sort-Object LastWriteTime -Descending |
                            Select-Object -First 1

            if ($latestBackup) {
                Write-Host "Restoring from $($latestBackup.FullName)"

                Remove-Item "$env:IIS_SITE_PATH\\*" -Recurse -Force
                Copy-Item "$($latestBackup.FullName)\\*" "$env:IIS_SITE_PATH" -Recurse -Force
            }
            '''

            bat """
            %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:%APP_POOL%
            """

            echo '🔁 Rollback completed'
        }

        success {
			echo '✅ Deployment completed successfully'

			emailext(
				subject: "✅ SUCCESS: ${env.JOB_NAME} #${env.BUILD_NUMBER}",
				body: """
		Build succeeded 🎉

		Job Name : ${env.JOB_NAME}
		Build No : ${env.BUILD_NUMBER}
		Branch   : ${env.BRANCH_NAME}

		Check console output for more details.
		""",
				to: 'akshay.k@helpxpress.com'
			)
		}

        always {
            echo '📌 Pipeline finished'
        }
    }
}

pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        bat(script: '"C:\\Program Files\\Unity\\Hub\\Editor\\2018.4.17f1\\Editor\\Unity.exe" -batchmode -projectPath "C:\\Program Files (x86)\\Jenkins\\workspace\\DATX05-Master_Thesis_master\\DATX05-Prototype-A" -executeMethod AutomatedBuild.BuildOnServer -quit', returnStatus: true, returnStdout: true, label: 'Build Unity Executables')
      }
    }

  }
  environment {
    Unity = ''
  }
}
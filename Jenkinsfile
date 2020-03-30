pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        bat(script: '"C:\\Program Files\\Unity\\Hub\\Editor\\2018.4.17f1\\Editor\\Unity.exe" -batchmode -projectPath "D:\\Unity\\DATX05-Master_Thesis\\DATX05-Prototype-A" -executeMethod AutomatedBuild.HelloWorld -quit', returnStatus: true, returnStdout: true, label: 'Build Unity Executables')
      }
    }

  }
}
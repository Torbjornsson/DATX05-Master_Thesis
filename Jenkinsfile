pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        bat(script: 'Build', returnStatus: true, returnStdout: true)
      }
    }

  }
}
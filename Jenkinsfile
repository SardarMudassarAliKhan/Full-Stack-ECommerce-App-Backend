pipeline {
    agent any
    
    options {
        disableConcurrentBuilds()
        buildDiscarder(logRotator(numToKeepStr: '5'))
    }  
    
    stages {
        // STEP 1: Crucial addition for pulling your Git repository files down
        stage('Checkout Source') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                script {
                    echo 'Cleaning up old production images...'
                    sh 'docker rmi ecom-backend:latest || true'

                    echo 'Building the new .NET 9 Docker image...'
                    sh 'docker build --no-cache -t ecom-backend:latest -f ECommerce.API/Dockerfile .'
                }
            }
        }

        stage('Push and Deploy') {
            steps {                
                script {
                    echo 'Stopping old active container if it exists...'
                    sh 'docker stop ecom-backend || true'
                    sh 'docker rm ecom-backend || true'
                    
                    echo 'Running new container on port 8202...'
                    sh 'docker run -d --restart always --name ecom-backend --env "ASPNETCORE_ENVIRONMENT=Development" --network zohan -p 8202:8080 ecom-backend:latest'
                }
            }
        }

        stage('Cleanup') {
            steps {
                script {
                    echo 'Cleaning up dangling images...'
                    sh 'docker image prune -f'
                }
            }
        }
    }
}
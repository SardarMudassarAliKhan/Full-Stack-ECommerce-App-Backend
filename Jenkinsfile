pipeline {
    agent any
    
    options {
        disableConcurrentBuilds()
        buildDiscarder(logRotator(numToKeepStr: '5'))
    }  
    
    environment {
        // Extracts 'dev', 'stage', or 'prod' from the end of your branch name
        ENV_NAME = "${BRANCH_NAME.tokenize('-')[-1]}"
        
        // Dynamically assigns a unique host port for each environment
        // Prod = 8202 | Stage = 8203 | Dev = 8204
        PORT = "${ENV_NAME == 'prod' ? '8202' : ENV_NAME == 'stage' ? '8203' : '8204'}"
        
        // Sets the correct ASP.NET Core environment variable value
        DOTNET_ENV = "${ENV_NAME == 'prod' ? 'Production' : ENV_NAME == 'stage' ? 'Staging' : 'Development'}"
    }

    stages {
        stage('Checkout') {
            steps {
                // Explicitly pulls the latest code for the active branch
                checkout scm
            }
        }

        stage('Build') {
            steps {
                script {
                    echo "Building Docker image for environment: ${ENV_NAME}..."
                    
                    // Tags the image cleanly using the environment suffix (e.g., ecom-backend:prod)
                    sh "docker build --no-cache -t ecom-backend:${ENV_NAME} -f ECommerce.API/Dockerfile ."
                }
            }
        }

        stage('Push and Deploy') {
            steps {                
                script {
                    echo "Stopping and removing old container: ecom-backend-${ENV_NAME} if running..."
                    sh "docker stop ecom-backend-${ENV_NAME} || true"
                    sh "docker rm ecom-backend-${ENV_NAME} || true"
                    
                    echo "Deploying new container on port ${PORT} with environment ${DOTNET_ENV}..."
                    sh """
                        docker run -d \
                        --restart always \
                        --name ecom-backend-${ENV_NAME} \
                        --env "ASPNETCORE_ENVIRONMENT=${DOTNET_ENV}" \
                        --network zohan \
                        -p ${PORT}:8080 \
                        ecom-backend:${ENV_NAME}
                    """
                }
            }
        }

        stage('Cleanup') {
            steps {
                script {
                    echo "Cleaning up dangling images and build artifacts..."
                    sh 'docker image prune -f'
                }
            }
        }
    }
}
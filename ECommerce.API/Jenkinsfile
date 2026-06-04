pipeline {
    agent any
	 options {
        disableConcurrentBuilds()
    }  
    stages {
        stage('Build') {
            steps {
                script {
                    // Remove previous image with the <none> tag  testing
                    sh 'docker rmi ecom-backend:latest || true'

                    // Build the new image with an explicit tag  
                    sh 'docker build --no-cache -t ecom-backend:latest -f ECommerce.API/Dockerfile .'

                    // Remove dangling images
                    sh 'docker images -q --filter "dangling=true" | xargs docker rmi || true'

                    // Clean up intermediate images
                    sh 'docker image prune -f'

                    // Custom cleanup script to remove any remaining <none> tagged images
                    sh '''
                        for image_id in $(docker images --filter "dangling=true" -q); do
                            docker rmi $image_id || true
                        done
                    '''
                }
            }
        }
        stage('Push and Deploy') {
            steps {                
                script {
                    // Stop and remove the container if it exists
                    sh 'docker stop ecom-backend || true'
                    sh 'docker rm ecom-backend || true'
                    
                    // Run the new container
                    sh 'docker run -d --restart always --name ecom-backend --env "ASPNETCORE_ENVIRONMENT=Development" --network zohan -p 8202:8080 ecom-backend:latest'

                     sh 'docker image prune -f'
                }
            }
        }
    }
}
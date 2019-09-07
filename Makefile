NO_COLOR=\x1b[0m
WARN_COLOR=\x1b[33;01m
WARN_SECRETS=$(WARN_COLOR)[REMEMBER TO RUN K8S SECRETS MANUALLY]$(NO_COLOR)

docker-all:
	@ eval $(minikube docker-env) && docker build -t ot-api:local -f src/API/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-api-identity:local -f src/API.Identity/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo:local -f src/Silo/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo-dashboard:local -f src/Silo.Dashboard/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-postgres:local -f postgres/Dockerfile postgres/

k8s:
	@ echo "$(WARN_SECRETS)"
	@ kubectl delete deployment api-deployment;  \
	  kubectl delete deployment api-identity-deployment;  \
	  kubectl delete deployment postgres-deployment;	 \
	  kubectl delete deployment silo-deployment;  \
	  kubectl delete deployment silo-dashboard-deployment; \
	  kubectl apply -f kubernetes


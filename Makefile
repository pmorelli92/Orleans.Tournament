NO_COLOR=\x1b[0m
WARN_COLOR=\x1b[33;01m
WARN_SECRETS=$(WARN_COLOR)[IN A REAL WORLD SOLUTION SECRETS MUST NOT BE INCLUDED IN THE REPOSITORY]$(NO_COLOR)

docker-all:
	@ eval $(minikube docker-env) && docker build -t ot-api:local -f src/API/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-api-identity:local -f src/API.Identity/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo:local -f src/Silo/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo-dashboard:local -f src/Silo.Dashboard/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-postgres:local -f postgres/Dockerfile postgres/

secrets:
	@ echo "$(WARN_SECRETS)"
	@ kubectl delete secret postgres-connection;  \
	  kubectl delete secret jwt-issuer-key;  \
	  kubectl create secret generic postgres-connection --from-literal=value="Server=192.168.99.100;Port=30700;User Id=dbuser;Password=dbpassword;Database=orleans-tournament"; \
	  kubectl create secret generic jwt-issuer-key --from-literal=value="mUL-M6N5]4;S9XHp"

k8s: secrets
	@ kubectl delete deployment api-deployment;  \
	  kubectl delete deployment api-identity-deployment;  \
	  kubectl delete deployment postgres-deployment;	 \
	  kubectl delete deployment silo-deployment;  \
	  kubectl delete deployment silo-dashboard-deployment; \
	  kubectl apply -f kubernetes


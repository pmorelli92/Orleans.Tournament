NO_COLOR=\x1b[0m
WARN_COLOR=\x1b[33;01m
WARN_SECRETS=$(WARN_COLOR)[IN A REAL WORLD SOLUTION SECRETS MUST NOT BE INCLUDED IN THE REPOSITORY]$(NO_COLOR)

docker/postgres:
	@ eval $(minikube docker-env) && docker build -t ot-postgres:local -f postgres/Dockerfile postgres/

docker-all: docker/postgres
	@ eval $(minikube docker-env) && docker build -t ot-api:local -f src/API/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-api-identity:local -f src/API.Identity/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo:local -f src/Silo/Dockerfile .
	@ eval $(minikube docker-env) && docker build -t ot-silo-dashboard:local -f src/Silo.Dashboard/Dockerfile .

k8s-load-img:
	kind load docker-image \
	ot-api:local \
	ot-api-identity:local \
	ot-silo:local \
	ot-silo-dashboard:local \
	ot-postgres:local

secrets:
	@ echo "$(WARN_SECRETS)"
	@ kubectl delete secret postgres-connection;  \
	  kubectl delete secret jwt-issuer-key;  \
	  kubectl create secret generic postgres-connection --from-literal=value="Server=postgres-service;Port=5432;User Id=dbuser;Password=dbpassword;Database=orleans-tournament"; \
	  kubectl create secret generic jwt-issuer-key --from-literal=value="mUL-M6N5]4;S9XHp"

k8s: secrets
	@ kubectl apply -f kubernetes

k8s-delete-apps:
	@ kubectl delete deployment api-deployment;  \
	  kubectl delete deployment api-identity-deployment;  \
	  kubectl delete deployment postgres-deployment;	 \
	  kubectl delete deployment silo-deployment;  \
	  kubectl delete deployment silo-dashboard-deployment; \

all: k8s-delete-apps docker-all k8s-load-img k8s


# kind create cluster --config=./kubernetes/kind-cluster.yaml

docker/build:
	docker build -t ot-postgres:local -f postgres/Dockerfile postgres/
	docker build -t ot-api:local -f src/API/Dockerfile .
	docker build -t ot-api-identity:local -f src/API.Identity/Dockerfile .
	docker build -t ot-silo:local -f src/Silo/Dockerfile .
	docker build -t ot-silo-dashboard:local -f src/Silo.Dashboard/Dockerfile .

# Second and third line are to enable metrics server
cluster/init:
	kind create cluster --config=./kind-cluster.yaml
	kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/download/v0.5.0/components.yaml
	kubectl patch -n kube-system deployment metrics-server --type=json -p '[{"op":"add","path":"/spec/template/spec/containers/0/args/-","value":"--kubelet-insecure-tls"}]'

cluster/images:
	-kind load docker-image \
	ot-api:local \
	ot-api-identity:local \
	ot-silo:local \
	ot-silo-dashboard:local \
	ot-postgres:local

cluster/apply:
	kubectl create secret generic postgres-connection --from-literal=value="Server=postgres-service;Port=5432;User Id=dbuser;Password=dbpassword;Database=orleans-tournament"; \
	kubectl create secret generic jwt-issuer-key --from-literal=value="mUL-M6N5]4;S9XHp"
	kubectl apply -f kubernetes

cluster/teardown:
	-kubectl delete secret postgres-connection
	-kubectl delete secret jwt-issuer-key
	-kubectl delete deployment api-deployment
	-kubectl delete deployment api-identity-deployment
	-kubectl delete deployment postgres-deployment
	-kubectl delete deployment silo-deployment
	-kubectl delete deployment silo-dashboard-deployment

run: docker/build cluster/init cluster/images cluster/apply

restart: docker/build cluster/images cluster/teardown cluster/apply

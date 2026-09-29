.PHONY: deps
deps:
	-docker rm -f my-postgres my-rabbitmq localstack redis 
	docker run --name my-postgres -e POSTGRES_PASSWORD=mypassword -e POSTGRES_DB=tiktok-clone -v pgdata:/var/lib/postgresql -p 5432:5432 -d postgres
	docker run -d --name my-rabbitmq -p 5672:5672 -p 15672:15672 -v rabbitmqdata:/var/lib/rabbitmq rabbitmq:4-management
	docker run -d --name redis -p 6379:6379 redis:latest

deps-clean:
	-docker volume rm pgdata
deps-down:
	-docker rm -f my-postgres my-rabbitmq redis 
	-docker rm -f my-postgres my-rabbitmq

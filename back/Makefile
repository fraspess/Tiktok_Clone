.PHONY: deps
deps:
	-docker rm -f my-postgres my-rabbitmq localstack redis 
	docker run --name my-postgres -e POSTGRES_PASSWORD=mypassword -e POSTGRES_DB=tiktok-clone -v pgdata:/var/lib/postgresql -p 5432:5432 -d postgres
	docker run -d --name my-rabbitmq -p 5672:5672 -p 15672:15672 -v rabbitmqdata:/var/lib/rabbitmq rabbitmq:4-management
	docker run -d --name redis -p 6379:6379 redis:latest
	docker run -d --name localstack -p 4566:4566 -v ./init-scripts:/etc/localstack/init/ready.d -e SERVICES=s3 -e AWS_DEFAULT_REGION=eu-central-1 localstack/localstack:3.8.0

deps-clean:
	-docker rm -f my-postgres my-rabbitmq localstack redis
	-docker volume rm pgdata rabbitmqdata
  
deps-down:
	-docker rm -f my-postgres my-rabbitmq redis localstack
	-docker rm -f my-postgres my-rabbitmq
  
create-bucket:
	docker exec back-localstack-1 sh -c "\
		export AWS_ACCESS_KEY_ID=test; \
		export AWS_SECRET_ACCESS_KEY=test; \
		aws --endpoint-url=http://localhost:4566 s3 mb s3://tiktok-clone-academy2026 && \
		aws --endpoint-url=http://localhost:4566 s3api put-bucket-cors \
			--bucket tiktok-clone-academy2026 \
			--cors-configuration '{\"CORSRules\": [{\"AllowedOrigins\": [\"*\"], \"AllowedMethods\": [\"GET\", \"PUT\", \"HEAD\"], \"AllowedHeaders\": [\"*\"], \"ExposeHeaders\": [\"ETag\"], \"MaxAgeSeconds\": 3000}]}'"

docker build -t touchpointmedical:014 -f .\TouchpointMedical.Site\Dockerfile .
docker save -o TPM.014.tar touchpointmedical:014

ssh touchpoint@gateway.touchpointmed.io
cd gateway/

docker load -i ../goldman2/TPM.014.tar
docker compose up -d --no-deps --force-recreate tp-gateway-app

docker compose exec tp-nginx nginx -t
docker compose exec tp-nginx nginx -s reload
docker exec -it tp-gateway-app sh

#!/bin/bash
set -euo pipefail

AGENT_WORKDIR="/opt/jenkins-agent"
# Outside the Jenkins workspace so a workspace wipe doesn't delete the certificate.
# The Jenkinsfile passes it to compose.server.yaml as CERTBOT_DIR.
CERTBOT_DIR="/opt/certbot"
DOMAIN="${duckdns_subdomain}.duckdns.org"

# Wait for the apt lock instead of failing when unattended-upgrades runs on first boot
echo 'DPkg::Lock::Timeout "300";' | sudo tee /etc/apt/apt.conf.d/99lock-timeout > /dev/null

# Install Java
sudo apt update
sudo apt install openjdk-21-jre openjdk-21-jdk -y

# Install Docker
sudo apt install ca-certificates curl -y
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

sudo tee /etc/apt/sources.list.d/docker.sources <<EOF
Types: deb
URIs: https://download.docker.com/linux/ubuntu
Suites: $(. /etc/os-release && echo "$${UBUNTU_CODENAME:-$$VERSION_CODENAME}")
Components: stable
Architectures: $(dpkg --print-architecture)
Signed-By: /etc/apt/keyrings/docker.asc
EOF

sudo apt update
sudo apt install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin -y

# Create jenkins user for agent
sudo useradd -m -d $${AGENT_WORKDIR} -s /bin/bash jenkins || true
sudo usermod -aG docker jenkins
sudo mkdir -p $${AGENT_WORKDIR}
sudo chown -R jenkins:jenkins $${AGENT_WORKDIR}

# Set SSH keys for master to agent connection
sudo mkdir -p $${AGENT_WORKDIR}/.ssh
sudo tee $${AGENT_WORKDIR}/.ssh/authorized_keys > /dev/null <<'KEYEOF'
${public_key}
KEYEOF

sudo chmod 700 $${AGENT_WORKDIR}/.ssh
sudo chmod 600 $${AGENT_WORKDIR}/.ssh/authorized_keys
sudo chown -R jenkins:jenkins $${AGENT_WORKDIR}/.ssh

echo "--- Jenkins Agent installation complete ---"
echo "SSH authorized_keys configured for jenkins user"
echo "Agent workdir: $${AGENT_WORKDIR}"
echo "Waiting for master to connect via SSH..."

# HTTPS certificate for the frontend
sudo mkdir -p $${CERTBOT_DIR}/conf $${CERTBOT_DIR}/www

# Renew daily through the running frontend (nginx serves /.well-known/acme-challenge/
# from the www dir), then reload nginx. certbot only renews when < 30 days are left.
sudo tee /etc/cron.d/certbot-renew > /dev/null <<EOF
0 3 * * * root docker run --rm -v $${CERTBOT_DIR}/conf:/etc/letsencrypt -v $${CERTBOT_DIR}/www:/var/www/certbot certbot/certbot renew --webroot -w /var/www/certbot --quiet && docker exec tiktok-frontend-1 nginx -s reload
EOF

# The public IP changes after a stop/start, so update DuckDNS on every boot too
sudo tee /etc/cron.d/duckdns > /dev/null <<EOF
@reboot root sleep 30 && curl -fsS "https://www.duckdns.org/update?domains=${duckdns_subdomain}&token=${duckdns_token}&ip=" > /dev/null
EOF
sudo chmod 600 /etc/cron.d/duckdns

# Point DuckDNS at this server (empty ip= means the IP the request comes from)
if [ "$(curl -fsS "https://www.duckdns.org/update?domains=${duckdns_subdomain}&token=${duckdns_token}&ip=")" != "OK" ]; then
  echo "DuckDNS update failed, check duckdns_subdomain and duckdns_token" >&2
  exit 1
fi

# Wait until the domain resolves to this server, then let Let's Encrypt's
# DNS cache (DuckDNS TTL is 60s) expire
PUBLIC_IP=$(curl -fsS https://checkip.amazonaws.com)
for i in $(seq 1 30); do
  [ "$(getent ahostsv4 "$${DOMAIN}" | awk 'NR==1 {print $1}')" = "$${PUBLIC_IP}" ] && break
  if [ "$i" -eq 30 ]; then
    echo "$${DOMAIN} still doesn't resolve to $${PUBLIC_IP}" >&2
    exit 1
  fi
  sleep 10
done
sleep 60

# Port 80 is still free here: the frontend container starts only after the first Jenkins build.
# If it fails, the frontend keeps restarting until the certificate exists, then comes up on its own.
sudo docker run --rm -p 80:80 \
  -v $${CERTBOT_DIR}/conf:/etc/letsencrypt \
  -v $${CERTBOT_DIR}/www:/var/www/certbot \
  certbot/certbot certonly --standalone -d "$${DOMAIN}" \
    %{ if letsencrypt_email != "" }--email "${letsencrypt_email}"%{ else }--register-unsafely-without-email%{ endif } \
    --agree-tos --no-eff-email --non-interactive %{ if letsencrypt_staging }--staging%{ endif }

echo "--- HTTPS certificate for $${DOMAIN} is in $${CERTBOT_DIR}/conf ---"

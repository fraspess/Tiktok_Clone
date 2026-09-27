#!/bin/bash
set -euo pipefail

# Wait for the apt lock instead of failing when unattended-upgrades runs on first boot
echo 'DPkg::Lock::Timeout "300";' | sudo tee /etc/apt/apt.conf.d/99lock-timeout > /dev/null

# Install Java
sudo apt update
sudo apt install fontconfig openjdk-21-jre openjdk-21-jdk -y

# Install Jenkins
sudo wget -O /etc/apt/keyrings/jenkins-keyring.asc \
  https://pkg.jenkins.io/debian-stable/jenkins.io-2026.key

echo "deb [signed-by=/etc/apt/keyrings/jenkins-keyring.asc] https://pkg.jenkins.io/debian-stable binary/" \
  | sudo tee /etc/apt/sources.list.d/jenkins.list > /dev/null

sudo apt update
sudo apt install jenkins -y

# Stop Jenkins to configure before first boot
sudo systemctl stop jenkins

# Install plugins via jenkins-plugin-manager
# jenkins-plugin-cli is only in the Docker image, not the Debian package.
# Download the plugin manager JAR and run it against the installed WAR.
PLUGIN_MGR_VERSION="2.14.0"
sudo wget -q -O /tmp/jenkins-plugin-manager.jar \
  "https://github.com/jenkinsci/plugin-installation-manager-tool/releases/download/$${PLUGIN_MGR_VERSION}/jenkins-plugin-manager-$${PLUGIN_MGR_VERSION}.jar"

sudo java -jar /tmp/jenkins-plugin-manager.jar \
  --war /usr/share/java/jenkins.war \
  --plugin-download-directory /var/lib/jenkins/plugins \
  --plugins \
    configuration-as-code \
    job-dsl \
    ssh-slaves \
    workflow-aggregator \
    git \
    docker-workflow \
    credentials-binding

sudo chown -R jenkins:jenkins /var/lib/jenkins/plugins

# Write secrets for JCasC (secrets directory approach)
# JCasC resolves variables from files in the secrets directory.
# File name = variable name, file content = value.
sudo mkdir -p /var/lib/jenkins/casc-secrets

sudo tee /var/lib/jenkins/casc-secrets/admin-password > /dev/null <<'PASSEOF'
${admin_password}
PASSEOF

sudo tee /var/lib/jenkins/casc-secrets/agent-ssh-key > /dev/null <<'KEYEOF'
${private_key_pem}
KEYEOF

# App secrets (.env_server) for the tiktok-env-server credential
echo '${env_server_b64}' | base64 -d | sudo tee /var/lib/jenkins/casc-secrets/env-server > /dev/null

sudo chmod 600 /var/lib/jenkins/casc-secrets/*
sudo chown -R jenkins:jenkins /var/lib/jenkins/casc-secrets

# JCasC YAML
# The YAML contains JCasC variable references like admin-password and agent-ssh-key.
# These are resolved by JCasC from the secrets directory at Jenkins startup.
sudo tee /var/lib/jenkins/jenkins.yaml > /dev/null <<'CASCEOF'
jenkins:
  systemMessage: "Jenkins configured automatically via JCasC"
  numExecutors: 0
  securityRealm:
    local:
      allowsSignup: false
      users:
        - id: "admin"
          password: "$${admin-password}"
  authorizationStrategy:
    loggedInUsersCanDoAnything:
      allowAnonymousRead: false
  nodes:
    - permanent:
        name: "agent-1"
        labelString: "tiktok"
        remoteFS: "/opt/jenkins-agent"
        numExecutors: 1
        mode: NORMAL
        launcher:
          ssh:
            host: "AGENT_IP_PLACEHOLDER"
            port: 22
            credentialsId: "agent-ssh-key"
            sshHostKeyVerificationStrategy:
              nonVerifyingKeyVerificationStrategy: {}
        retentionStrategy: "always"

jobs:
  - script: |
      pipelineJob('tiktok') {
        description('Build and deploy TikTok with compose.server.yaml')

        triggers {
          scm('* * * * *')
        }

        definition {
          cpsScm {
            scm {
              git {
                remote {
                  url('https://github.com/fraspess/Tiktok_Clone.git')
                }
                branches('*/develop')
              }
            }
            scriptPath('tools/Jenkins.jenkinsfile')
            lightweight(true)
          }
        }
      }

credentials:
  system:
    domainCredentials:
      - credentials:
          - basicSSHUserPrivateKey:
              scope: SYSTEM
              id: "agent-ssh-key"
              username: "jenkins"
              description: "SSH key for Jenkins agent"
              privateKeySource:
                directEntry:
                  privateKey: "$${agent-ssh-key}"
          - file:
              scope: GLOBAL
              id: "tiktok-env-server"
              fileName: ".env_server"
              description: "App secrets for compose.server.yaml"
              secretBytes: "$${readFileBase64:/var/lib/jenkins/casc-secrets/env-server}"
CASCEOF

# Insert the agent IP into jenkins.yaml
sudo sed -i "s/AGENT_IP_PLACEHOLDER/${agent_ip}/" /var/lib/jenkins/jenkins.yaml

sudo chown jenkins:jenkins /var/lib/jenkins/jenkins.yaml

# Configure Jenkins environment (skip wizard + JCasC + secrets)
sudo mkdir -p /etc/systemd/system/jenkins.service.d
sudo tee /etc/systemd/system/jenkins.service.d/override.conf <<EOF
[Service]
Environment="JENKINS_LISTEN_ADDRESS=0.0.0.0"
Environment="SECRETS=/var/lib/jenkins/casc-secrets"
Environment="CASC_JENKINS_CONFIG=/var/lib/jenkins/jenkins.yaml"
Environment="JAVA_OPTS=-Djenkins.install.runSetupWizard=false"
EOF

# Start Jenkins
sudo systemctl daemon-reload
sudo systemctl enable jenkins
sudo systemctl start jenkins

echo "--- Jenkins Master installation complete ---"
echo "JCasC config: /var/lib/jenkins/jenkins.yaml"
echo "Agent node 'agent-1' (label: tiktok) configured to SSH to ${agent_ip}"

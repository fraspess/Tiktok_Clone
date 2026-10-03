#!/bin/bash
# Runs terraform apply for the Proxmox VMs with the values from tools/.env.
# Extra arguments go to terraform apply
set -euo pipefail

cd "$(dirname "$0")"

# TF_VAR_* values
set -a
source ../../.env
set +a

# The provider uploads the cloud-init snippets over SSH with the key from ssh-agent.
# ssh-add -l exits with 2 when there is no agent and 1 when it has no keys.
agent_status=0
ssh-add -l > /dev/null 2>&1 || agent_status=$?
if [ "$agent_status" -eq 2 ]; then
  eval "$(ssh-agent -s)" > /dev/null
  trap 'ssh-agent -k > /dev/null' EXIT
fi
if [ "$agent_status" -ne 0 ]; then
  ssh-add
fi

[ -d .terraform ] || terraform init -input=false
terraform apply "$@"

terraform {
  required_providers {
    proxmox = {
      source = "bpg/proxmox"
    }
    tls = {
      source = "hashicorp/tls"
    }
  }
}

// Provider
// Snippets (cloud-init user data) are uploaded to the node over SSH,
// with the key file if set, otherwise with the key loaded in ssh-agent
provider "proxmox" {
  endpoint  = var.proxmox_endpoint
  api_token = var.proxmox_api_token
  insecure  = var.proxmox_insecure

  ssh {
    username    = var.proxmox_ssh_user
    agent       = var.proxmox_ssh_key_file == ""
    private_key = var.proxmox_ssh_key_file == "" ? null : file(pathexpand(var.proxmox_ssh_key_file))
  }
}

// Certbot saves the certificate under the name nginx reads it from,
// so duckdns_subdomain can differ from the domain in the nginx config
locals {
  cert_name = regex("live/([^/]+)/fullchain\\.pem", file("${path.module}/../../../front/nginx.server.conf"))[0]
}

// Keys for  master to agent connection
resource "tls_private_key" "jenkins_agent" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

// Ubuntu 24.04 cloud image, the same OS as the AWS AMI.
// The .img is qcow2 inside, the file name tells Proxmox the format for import.
resource "proxmox_download_file" "ubuntu" {
  content_type = "import"
  datastore_id = var.files_datastore
  node_name    = var.node_name
  url          = var.ubuntu_image_url
  file_name    = "tiktok-noble-server-cloudimg-amd64.qcow2"
  // Don't download again when Ubuntu publishes a newer image
  overwrite = false
}


// Cloud-init user data, reuses the AWS install scripts
resource "proxmox_virtual_environment_file" "master_user_data" {
  content_type = "snippets"
  datastore_id = var.files_datastore
  node_name    = var.node_name

  source_raw {
    file_name = "tiktok-jenkins-master.yaml"
    data = templatefile("${path.module}/files/cloud-init.yaml.tftpl", {
      hostname       = "tiktok-jenkins-master"
      ssh_public_key = var.ssh_public_key
      script_b64 = base64encode(templatefile("${path.module}/../aws/files/install_jenkins_master.sh", {
        agent_ip        = var.agent_ip
        private_key_pem = tls_private_key.jenkins_agent.private_key_pem
        admin_password  = var.jenkins_admin_password
        env_server_b64  = filebase64("${path.module}/../../../.env_server")
      }))
    })
  }
}

resource "proxmox_virtual_environment_file" "agent_user_data" {
  content_type = "snippets"
  datastore_id = var.files_datastore
  node_name    = var.node_name

  source_raw {
    file_name = "tiktok-jenkins-agent.yaml"
    data = templatefile("${path.module}/files/cloud-init.yaml.tftpl", {
      hostname       = "tiktok-jenkins-agent"
      ssh_public_key = var.ssh_public_key
      script_b64 = base64encode(templatefile("${path.module}/../aws/files/install_jenkins_agent.sh", {
        public_key          = tls_private_key.jenkins_agent.public_key_openssh
        duckdns_subdomain   = var.duckdns_subdomain
        duckdns_token       = var.duckdns_token
        cert_name           = local.cert_name
        letsencrypt_email   = var.letsencrypt_email
        letsencrypt_staging = var.letsencrypt_staging
        duckdns_ip          = var.lan_only ? var.agent_ip : ""
      }))
    })
  }
}


// Jenkins Master
resource "proxmox_virtual_environment_vm" "jenkins_master" {
  name      = "tiktok-jenkins-master"
  node_name = var.node_name
  tags      = ["tiktok"]
  on_boot   = true

  agent {
    enabled = true
  }

  cpu {
    cores = var.master_cores
    type  = "x86-64-v2-AES"
  }

  memory {
    dedicated = var.master_memory
  }

  disk {
    datastore_id = var.vm_datastore
    import_from  = proxmox_download_file.ubuntu.id
    interface    = "virtio0"
    iothread     = true
    discard      = "on"
    size         = 15
  }

  network_device {
    bridge = var.network_bridge
  }

  operating_system {
    type = "l26"
  }

  // Ubuntu cloud images expect a serial console
  serial_device {}

  initialization {
    datastore_id = var.vm_datastore

    ip_config {
      ipv4 {
        address = "${var.master_ip}/${var.subnet_prefix}"
        gateway = var.gateway
      }
    }

    dns {
      servers = var.dns_servers
    }

    user_data_file_id = proxmox_virtual_environment_file.master_user_data.id
  }
}

// Jenkins Agent
resource "proxmox_virtual_environment_vm" "jenkins_agent" {
  name      = "tiktok-jenkins-agent"
  node_name = var.node_name
  tags      = ["tiktok"]
  on_boot   = true

  agent {
    enabled = true
  }

  cpu {
    cores = var.agent_cores
    type  = "x86-64-v2-AES"
  }

  memory {
    dedicated = var.agent_memory
  }

  // Docker images, build cache and DB backups need more space than the master
  disk {
    datastore_id = var.vm_datastore
    import_from  = proxmox_download_file.ubuntu.id
    interface    = "virtio0"
    iothread     = true
    discard      = "on"
    size         = 30
  }

  network_device {
    bridge = var.network_bridge
  }

  operating_system {
    type = "l26"
  }

  serial_device {}

  initialization {
    datastore_id = var.vm_datastore

    ip_config {
      ipv4 {
        address = "${var.agent_ip}/${var.subnet_prefix}"
        gateway = var.gateway
      }
    }

    dns {
      servers = var.dns_servers
    }

    user_data_file_id = proxmox_virtual_environment_file.agent_user_data.id
  }
}


//  Outputs
output "jenkins_url" {
  value       = "http://${var.master_ip}:8080"
  description = "Jenkins UI on the LAN"
}

output "jenkins_master_ip" {
  value       = var.master_ip
  description = "LAN IP - Jenkins Master"
}

output "jenkins_agent_ip" {
  value       = var.agent_ip
  description = "LAN IP - Jenkins Agent, forward ports 80 and 443 to it unless lan_only is set"
}

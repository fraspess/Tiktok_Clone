terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
    }
    tls = {
      source = "hashicorp/tls"
    }
  }
}

// Provider
provider "aws" {
  region     = "us-east-1"
  access_key = var.aws_access_key
  secret_key = var.aws_secret_key
}

// Keys for  master to agent connection
resource "tls_private_key" "jenkins_agent" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

// Look up the default VPC
data "aws_vpc" "default" {
  default = true
}

// Look up a subnet in the target AZ
data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }

  filter {
    name   = "availability-zone"
    values = [var.a-zone]
  }
}


// Jenkins Master
resource "aws_instance" "jenkins_master" {
  subnet_id              = data.aws_subnets.default.ids[0]
  ami                    = var.ami-id
  instance_type          = var.master_instance_type
  key_name               = var.key_name
  vpc_security_group_ids = [aws_security_group.jenkins_master_sg.id]

  root_block_device {
    volume_size = 15
    volume_type = "gp3"
    tags = {
      "name" = "root disk"
    }
  }

  tags = {
    Name = "TikTok-Jenkins-Master"
  }

  user_data = templatefile("files/install_jenkins_master.sh", {
    agent_ip        = aws_instance.jenkins_agent.private_ip
    private_key_pem = tls_private_key.jenkins_agent.private_key_pem
    admin_password  = var.jenkins_admin_password
    env_server_b64  = filebase64("${path.module}/../../../.env_server")
  })
}

// Jenkins Agent
resource "aws_instance" "jenkins_agent" {
  subnet_id              = data.aws_subnets.default.ids[0]
  ami                    = var.ami-id
  instance_type          = var.agent_instance_type
  key_name               = var.key_name
  vpc_security_group_ids = [aws_security_group.jenkins_agent_sg.id]

  // Docker images, build cache and DB backups need more space than the master
  root_block_device {
    volume_size = 30
    volume_type = "gp3"
    tags = {
      "name" = "root disk"
    }
  }

  tags = {
    Name = "TikTok-Jenkins-Agent"
  }

  # Pass the SSH public key so the master can connect to this agent,
  # and the DuckDNS/Let's Encrypt settings for the HTTPS certificate
  user_data = templatefile("files/install_jenkins_agent.sh", {
    public_key          = tls_private_key.jenkins_agent.public_key_openssh
    duckdns_subdomain   = var.duckdns_subdomain
    duckdns_token       = var.duckdns_token
    letsencrypt_email   = var.letsencrypt_email
    letsencrypt_staging = var.letsencrypt_staging
  })
}


//  Security Group - Master
resource "aws_security_group" "jenkins_master_sg" {
  name        = "tiktok-jenkins-master-sg"
  description = "SG Jenkins Master, web UI from specific IP, agent traffic on 8080"
  vpc_id      = data.aws_vpc.default.id
}

resource "aws_security_group_rule" "master_ssh" {
  type              = "ingress"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip]
  security_group_id = aws_security_group.jenkins_master_sg.id
  description       = "SSH from specific IP"
}

resource "aws_security_group_rule" "master_http_my_ip" {
  type              = "ingress"
  from_port         = 8080
  to_port           = 8080
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip]
  security_group_id = aws_security_group.jenkins_master_sg.id
  description       = "UI SG"
}

resource "aws_security_group_rule" "master_http_from_agent" {
  type                     = "ingress"
  from_port                = 8080
  to_port                  = 8080
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.jenkins_agent_sg.id
  security_group_id        = aws_security_group.jenkins_master_sg.id
  description              = "Jenkins agent - master on port 8080"
}

resource "aws_security_group_rule" "master_icmp" {
  type              = "ingress"
  from_port         = 8
  to_port           = 0
  protocol          = "icmp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_master_sg.id
  description       = "Allow ping"
}

resource "aws_security_group_rule" "master_egress" {
  type              = "egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_master_sg.id
}


//  Security Group - Agent
resource "aws_security_group" "jenkins_agent_sg" {
  name        = "tiktok-jenkins-agent-sg"
  description = "Agent SG, SSH from specific IP, traffic from master"
  vpc_id      = data.aws_vpc.default.id
}

resource "aws_security_group_rule" "agent_ssh" {
  type              = "ingress"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = [var.my_ip]
  security_group_id = aws_security_group.jenkins_agent_sg.id
  description       = "SSH from my IP"
}

resource "aws_security_group_rule" "agent_front" {
  type              = "ingress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_agent_sg.id
  description       = "Frontend HTTP - redirect to HTTPS and certificate challenges"
}

resource "aws_security_group_rule" "agent_front_https" {
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_agent_sg.id
  description       = "Frontend HTTPS"
}

resource "aws_security_group_rule" "agent_from_master" {
  type                     = "ingress"
  from_port                = 0
  to_port                  = 65535
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.jenkins_master_sg.id
  security_group_id        = aws_security_group.jenkins_agent_sg.id
  description              = "All TCP from Jenkins master"
}

resource "aws_security_group_rule" "agent_icmp" {
  type              = "ingress"
  from_port         = 8
  to_port           = 0
  protocol          = "icmp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_agent_sg.id
  description       = "Allow ping"
}

resource "aws_security_group_rule" "agent_egress" {
  type              = "egress"
  from_port         = 0
  to_port           = 0
  protocol          = "-1"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = aws_security_group.jenkins_agent_sg.id
}


//  Outputs
output "jenkins_master_public_ip" {
  value       = aws_instance.jenkins_master.public_ip
  description = "Public IP - Jenkins Master"
}

output "jenkins_agent_public_ip" {
  value       = aws_instance.jenkins_agent.public_ip
  description = "Public IP - Jenkins Agent"
}

output "jenkins_master_private_ip" {
  value       = aws_instance.jenkins_master.private_ip
  description = "Private IP - Jenkins Master"
}
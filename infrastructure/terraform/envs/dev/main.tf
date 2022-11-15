resource "random_password" "database_password" {
  length  = 24
  special = false
}

# Secrets

resource "aws_secretsmanager_secret" "db_password" {
  name                    = "/${var.application_name}/db_password"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "db_password" {
  secret_id     = aws_secretsmanager_secret.db_password.id
  secret_string = random_password.database_password.result
}

# DocumentDB

resource "aws_docdb_global_cluster" "reservation" {
  provider = aws.primary
  global_cluster_identifier = "reservation"
  engine                    = "docdb"
  engine_version            = "4.0.0"
}

resource "aws_docdb_cluster" "primary" {
  skip_final_snapshot       = true
  provider                  = aws.primary
  engine                    = aws_docdb_global_cluster.reservation.engine
  engine_version            = aws_docdb_global_cluster.reservation.engine_version
  cluster_identifier        = "reservation-primary-cluster"
  master_username           = var.db_username
  master_password           = random_password.database_password.result
  global_cluster_identifier = aws_docdb_global_cluster.reservation.id
  db_subnet_group_name      = aws_docdb_subnet_group.west.name
  vpc_security_group_ids    = [aws_security_group.rds_west.id]
}

resource "aws_docdb_cluster_instance" "primary" {
  provider           = aws.primary
  engine             = aws_docdb_global_cluster.reservation.engine
  identifier         = "reservation-primary-cluster-instance"
  cluster_identifier = aws_docdb_cluster.primary.id
  instance_class     = "db.r5.large"
}

resource "aws_docdb_cluster" "secondary" {
  skip_final_snapshot       = true
  provider                  = aws.secondary
  engine                    = aws_docdb_global_cluster.reservation.engine
  engine_version            = aws_docdb_global_cluster.reservation.engine_version
  cluster_identifier        = "reservation-secondary-cluster"
  global_cluster_identifier = aws_docdb_global_cluster.reservation.id
  db_subnet_group_name      = aws_docdb_subnet_group.east.name
  vpc_security_group_ids    = [aws_security_group.rds_east.id]
}

resource "aws_docdb_cluster_instance" "secondary" {
  provider           = aws.secondary
  engine             = aws_docdb_global_cluster.reservation.engine
  identifier         = "reservation-secondary-cluster-instance"
  cluster_identifier = aws_docdb_cluster.secondary.id
  instance_class     = "db.r5.large"

  depends_on = [
    aws_docdb_cluster_instance.primary
  ]
}

# Remote Access EC2 Instance for SSH tunnel

data "aws_ami" "remote_access" {
  most_recent = true

  filter {
    name   = "name"
    values = ["ubuntu/images/hvm-ssd/ubuntu-xenial-16.04-amd64-server-*"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }

  owners = ["099720109477"]
}

resource "aws_instance" "remote_access" {
  provider               = aws.primary
  ami                    = data.aws_ami.remote_access.id
  vpc_security_group_ids = [aws_security_group.remote_access_west.id]
  subnet_id              = aws_subnet.public_a_west.id
  instance_type          = "t2.micro"
  key_name               = aws_key_pair.remote_access.key_name

  #  provisioner "remote-exec" {
  #    inline = [
  #      "sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv 2930ADAE8CAF5059EE73BB4B58712A2291FA4AD5",
  #      "echo 'deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu xenial/mongodb-org/3.6 multiverse' | sudo tee /etc/apt/sources.list.d/mongodb-org-3.6.list",
  #      "sudo apt-get update",
  #      "sudo apt-get install -y mongodb-org-shell"
  #    ]
  #  }
  #
  #  connection {
  #    type = "ssh"
  #    user = "ubuntu"
  #    private_key = "${tls_private_key.remote_access.private_key_pem}"
  #  }
}

resource "tls_private_key" "remote_access" {
  algorithm = "RSA"
  rsa_bits  = 4096
}

resource "aws_key_pair" "remote_access" {
  key_name   = "tf-${var.application_name}-ec2"
  public_key = tls_private_key.remote_access.public_key_openssh
}

#resource "local_file" "remote_access_private_key" {
#  content  = tls_private_key.remote_access.private_key_pem
#  filename = aws_key_pair.remote_access.key_name
#  provisioner "local-exec" {
#    command = "chmod 400 ${aws_key_pair.remote_access.key_name}"
#  }
#}

resource "aws_iam_role" "iam_for_lambda" {
  name = "iam_for_lambda"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

resource "aws_iam_role_policy" "services" {
  name = "iam_for_lambda_services"
  role = aws_iam_role.iam_for_lambda.id

  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "s3:*",
        "ec2:*"
      ],
      "Effect": "Allow",
      "Resource": "*"
    }
  ]
}
EOF
}

resource "aws_iam_policy_attachment" "lambda_access_policy" {
  name = "lambda-access-policy-attachment"
  roles = [aws_iam_role.iam_for_lambda.name]
  policy_arn = "arn:aws:iam::aws:policy/AWSLambda_FullAccess"
}

resource "aws_iam_role_policy_attachment" "AWSLambdaVPCAccessExecutionRole" {
  role       = aws_iam_role.iam_for_lambda.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
}

resource "aws_iam_role_policy_attachment" "AWSLambdaBasicExecutionRole" {
  role       = aws_iam_role.iam_for_lambda.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy" "policy_secretsmanager" {
  name = "policy_secretsmanager"
  role = aws_iam_role.iam_for_lambda.id

  policy = <<-EOF
  {
    "Version": "2012-10-17",
    "Statement": [
      {
        "Action": [
          "secretsmanager:GetSecretValue"
        ],
        "Effect": "Allow",
        "Resource": [
          "${aws_secretsmanager_secret.db_password.arn}"
        ]
      }
    ]
  }
  EOF
}

# data "aws_region" "current" {}
# 
# locals {
#   architecture_to_arns_mapping = {
#     "x86_64" = local.collector_layer_arns_amd64
#     "arm64"  = local.collector_layer_arns_arm64
#   }
# }

resource "aws_lambda_function" "reservation_service" {
  provider              = aws.primary
  filename              = "lambda_function_payload.zip"
  function_name         = "reservation_service"
  role                  = aws_iam_role.iam_for_lambda.arn
  handler               = "Scheduling.Reservation.API"
  memory_size           = var.memory_allocation
  timeout               = var.api_timeout

  vpc_config {
    security_group_ids = [aws_security_group.rds_west.id]
    subnet_ids         = [aws_subnet.private_a_west.id]
  }

  source_code_hash = filebase64sha256("lambda_function_payload.zip")

  runtime = "dotnet6"

  environment {
    variables = {
      ASPNETCORE_ENVIRONMENT = var.aspnetcore_env,

      ReservationDatabase__Username = var.db_username,
      ReservationDatabase__ConnectionString = "mongodb://${urlencode(var.db_username)}:${urlencode(random_password.database_password.result)}@${aws_docdb_cluster_instance.primary.endpoint}:${aws_docdb_cluster_instance.primary.port}/?tls=true",
      ReservationDatabase__DatabaseName = "scheduling",
      ReservationDatabase__CollectionName = "reservations",
      ReservationDatabase__UseCloudConnection = "true",

      DT_TENANT = "ivw36740",
      DT_CLUSTER_ID = "1787758731",
      DT_CONNECTION_BASE_URL = "https://ivw36740.live.dynatrace.com",
      DT_CONNECTION_AUTH_TOKEN = var.dynatrace_key,

      AWS__DatabaseSecretArn = aws_secretsmanager_secret.db_password.arn,
      AWS__SecretManagerEndpoint = "https://${element(aws_vpc_endpoint.private_secretsmanager.dns_entry, 0).dns_name}",
      Logging__Console__FormatterName = "simple"
    }
  }

  depends_on = [
    aws_docdb_cluster.primary
  ]
}

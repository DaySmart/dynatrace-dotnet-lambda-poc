provider "aws" {
  region = "us-west-2"
}

provider "aws" {
  alias  = "primary"
  region = "us-west-2"
}

provider "aws" {
  alias  = "secondary"
  region = "us-east-1"
}

resource "aws_vpc" "scheduling-reservation" {
  provider              = aws.primary
  cidr_block            = "10.0.0.0/16"
  enable_dns_hostnames  = true
  enable_dns_support    = true

  tags = {
    Name = var.application_name
  }
}

resource "aws_subnet" "public_a_west" {
  provider          = aws.primary
  vpc_id            = aws_vpc.scheduling-reservation.id
  cidr_block        = "10.0.1.0/25"
  availability_zone = "us-west-2c"

  tags = {
    "Name" = "public | us-west-2c"
  }
}

resource "aws_subnet" "private_a_west" {
  provider          = aws.primary
  vpc_id            = aws_vpc.scheduling-reservation.id
  cidr_block        = "10.0.2.0/25"
  availability_zone = "us-west-2d"

  tags = {
    "Name" = "private | us-west-2d"
  }
}

resource "aws_docdb_subnet_group" "west" {
  provider   = aws.primary
  name       = "${var.application_name}-docdb"
  subnet_ids = [aws_subnet.public_a_west.id, aws_subnet.private_a_west.id]

  tags = {
    Name = "${var.application_name}-docdb"
  }
}

resource "aws_vpc" "scheduling-reservation-east" {
  provider             = aws.secondary
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true

  tags = {
    Name = var.application_name
  }
}

resource "aws_subnet" "public_a_east" {
  provider          = aws.secondary
  vpc_id            = aws_vpc.scheduling-reservation-east.id
  cidr_block        = "10.0.1.128/25"
  availability_zone = "us-east-1d"

  tags = {
    "Name" = "public | us-east-1d"
  }
}

resource "aws_subnet" "private_a_east" {
  provider          = aws.secondary
  vpc_id            = aws_vpc.scheduling-reservation-east.id
  cidr_block        = "10.0.2.128/25"
  availability_zone = "us-east-1e"

  tags = {
    "Name" = "private | us-east-1e"
  }
}

resource "aws_docdb_subnet_group" "east" {
  provider   = aws.secondary
  name       = "${var.application_name}-docdb"
  subnet_ids = [aws_subnet.public_a_east.id, aws_subnet.private_a_east.id]

  tags = {
    Name = "${var.application_name}-docdb"
  }
}

resource "aws_security_group" "remote_access_west" {
  provider = aws.primary
  name   = "${var.application_name}-remote-access-security"
  vpc_id = aws_vpc.scheduling-reservation.id

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.application_name}-remote-access-security"
  }
}

resource "aws_security_group" "rds_west" {
  provider = aws.primary
  name   = "${var.application_name}-docdb-security"
  vpc_id = aws_vpc.scheduling-reservation.id

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.application_name}-docdb-security"
  }
}

resource "aws_security_group" "rds_east" {
  provider = aws.secondary
  name   = "${var.application_name}-docdb-security"
  vpc_id = aws_vpc.scheduling-reservation-east.id

  ingress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "${var.application_name}-docdb-security"
  }
}

resource "aws_eip" "remote_access" {
  provider = aws.primary
  vpc      = true
  instance = aws_instance.remote_access.id

  tags = {
    Name = "remote_access"
  }
}

resource "aws_eip" "lb" {
  provider = aws.primary
  vpc      = true

  tags = {
    Name = "nat"
  }
}

resource "aws_route_table" "public_west" {
  provider = aws.primary
  vpc_id = aws_vpc.scheduling-reservation.id
  tags   = {
    "Name" = "public_access"
  }
}

resource "aws_route_table" "private_west" {
  provider = aws.primary
  vpc_id = aws_vpc.scheduling-reservation.id
  tags   = {
    "Name" = "private_access"
  }
}

resource "aws_route_table_association" "public_west_subnet" {
  provider = aws.primary
  subnet_id      = aws_subnet.public_a_west.id
  route_table_id = aws_route_table.public_west.id
}

resource "aws_route_table_association" "private_west_subnet" {
  provider = aws.primary
  subnet_id      = aws_subnet.private_a_west.id
  route_table_id = aws_route_table.private_west.id
}

#resource "aws_route_table" "public_east" {
#  provider = aws.secondary
#  vpc_id = aws_vpc.scheduling-reservation-east.id
#  tags   = {
#    "Name" = "public_access"
#  }
#}
#
#resource "aws_route_table" "private_east" {
#  provider = aws.secondary
#  vpc_id = aws_vpc.scheduling-reservation-east.id
#  tags   = {
#    "Name" = "private_access"
#  }
#}
#
#resource "aws_route_table_association" "public_east_subnet" {
#  provider = aws.secondary
#  subnet_id      = aws_subnet.public_a_east.id
#  route_table_id = aws_route_table.public_east.id
#}
#
#resource "aws_route_table_association" "private_east_subnet" {
#  provider = aws.secondary
#  subnet_id      = aws_subnet.private_a_east.id
#  route_table_id = aws_route_table.private_east.id
#}

resource "aws_internet_gateway" "igw" {
  provider = aws.primary
  vpc_id = aws_vpc.scheduling-reservation.id
}

resource "aws_nat_gateway" "ngw" {
  provider = aws.primary
  subnet_id     = aws_subnet.public_a_west.id
  allocation_id = aws_eip.lb.id

  depends_on = [aws_internet_gateway.igw]
}

resource "aws_route" "public_igw" {
  provider = aws.primary
  route_table_id         = aws_route_table.public_west.id
  destination_cidr_block = "0.0.0.0/0"
  gateway_id             = aws_internet_gateway.igw.id
}

resource "aws_route" "private_ngw" {
  provider = aws.primary
  route_table_id         = aws_route_table.private_west.id
  destination_cidr_block = "0.0.0.0/0"
  nat_gateway_id         = aws_nat_gateway.ngw.id
}

# Lambda API Gateway

resource "aws_apigatewayv2_api" "lambda" {
  name          = "lambda_gw"
  protocol_type = "HTTP"
}

resource "aws_apigatewayv2_stage" "lambda" {
  api_id = aws_apigatewayv2_api.lambda.id

  name        = "reservation"
  auto_deploy = true

  access_log_settings {
    destination_arn = aws_cloudwatch_log_group.api_gw.arn

    format = jsonencode({
      requestId               = "$context.requestId"
      sourceIp                = "$context.identity.sourceIp"
      requestTime             = "$context.requestTime"
      protocol                = "$context.protocol"
      httpMethod              = "$context.httpMethod"
      resourcePath            = "$context.resourcePath"
      routeKey                = "$context.routeKey"
      status                  = "$context.status"
      responseLength          = "$context.responseLength"
      integrationErrorMessage = "$context.integrationErrorMessage"
    }
    )
  }
}

resource "aws_apigatewayv2_domain_name" "lambda" {
  domain_name = var.dns_name

  domain_name_configuration {
    certificate_arn = var.certificate_arn
    endpoint_type   = "REGIONAL"
    security_policy = "TLS_1_2"
  }
}

resource "aws_apigatewayv2_api_mapping" "lambda" {
  api_id      = aws_apigatewayv2_api.lambda.id
  domain_name = aws_apigatewayv2_domain_name.lambda.id
  stage       = aws_apigatewayv2_stage.lambda.id
}

//route53 alias A record to api
resource "aws_route53_record" "lambda" {
  zone_id = var.aws_zone_id
  name = aws_apigatewayv2_domain_name.lambda.domain_name
  type = "A"

  alias {
    name = aws_apigatewayv2_domain_name.lambda.domain_name_configuration.0.target_domain_name
    zone_id = aws_apigatewayv2_domain_name.lambda.domain_name_configuration.0.hosted_zone_id
    evaluate_target_health = false
  }
}

resource "aws_apigatewayv2_integration" "reservation_service" {
  api_id = aws_apigatewayv2_api.lambda.id

  integration_uri    = aws_lambda_function.reservation_service.invoke_arn
  integration_type   = "AWS_PROXY"
  integration_method = "POST"
}

resource "aws_apigatewayv2_route" "reservation_service" {
  api_id = aws_apigatewayv2_api.lambda.id

  route_key = "ANY /{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.reservation_service.id}"
}

resource "aws_cloudwatch_log_group" "api_gw" {
  name = "/aws/api_gw/${aws_apigatewayv2_api.lambda.name}"

  retention_in_days = 30
}

resource "aws_lambda_permission" "api_gw" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.reservation_service.function_name
  principal     = "apigateway.amazonaws.com"

  source_arn = "${aws_apigatewayv2_api.lambda.execution_arn}/*/*"
}

resource "aws_vpc_endpoint" "private_secretsmanager" {
  provider = aws.primary
  vpc_id = aws_vpc.scheduling-reservation.id
  service_name = "com.amazonaws.us-west-2.secretsmanager"
  vpc_endpoint_type = "Interface"
  private_dns_enabled = true
  security_group_ids = [aws_security_group.rds_west.id]
  subnet_ids = [aws_subnet.private_a_west.id]
  policy = <<POLICY
    {
    "Statement": [
        {
        "Action": "*",
        "Effect": "Allow",
        "Resource": "*",
        "Principal": "*"
        }
    ]
    }
    POLICY
}

#resource "aws_vpc_endpoint_route_table_association" "private-secretsmanager" {
#  provider = aws.primary
#  vpc_endpoint_id = aws_vpc_endpoint.private-secretsmanager.id
#  route_table_id  = aws_route_table.private_west.id
#}

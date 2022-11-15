output "rds_hostname" {
  description = "RDS instance hostname"
  value       = aws_docdb_cluster_instance.primary.endpoint
}

output "rds_port" {
  description = "RDS instance port"
  value       = aws_docdb_cluster_instance.primary.port
}

output "rds_username" {
  description = "RDS instance root username"
  value       = aws_docdb_cluster.primary.master_username
}

output "remote_access_dns" {
  value = "${aws_instance.remote_access.public_dns}"
}

output "remote_access_ip" {
  value = "${aws_instance.remote_access.public_ip}"
}

output "elastic_ip" {
  value = "${aws_eip.lb.public_ip}"
}

output "elastic_dns" {
  value = "${aws_eip.lb.public_dns}"
}

output "function_name" {
  value = aws_lambda_function.reservation_service.function_name
}

output "base_url" {
  value = aws_apigatewayv2_stage.lambda.invoke_url
}

output "vpc_private_endpoint" {
  value = aws_vpc_endpoint.private_secretsmanager.dns_entry
}

output "vpc_private_endpoint_element" {
  value = element(aws_vpc_endpoint.private_secretsmanager.dns_entry, 0).dns_name
}

output "service_endpoint" {
  value = aws_apigatewayv2_domain_name.lambda.domain_name
}

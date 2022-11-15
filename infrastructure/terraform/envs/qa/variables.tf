variable "application_name" {
  type    = string
  default = "scheduling-reservation"
}

variable "region" {
  default     = "us-west-2"
  description = "AWS region"
}

variable "db_name" {
  type    = string
  default = "reservation"
}

variable "db_username" {
  type    = string
  default = "scheduling_service"
}

variable "aspnetcore_env" {
  type    = string
  default = "Development"
}

variable "dns_name" {
  type    = string
  default = "dev.reservation.daysmart.com"
}

variable "aws_zone_id" {
  description = "Route53 Zone ID -- this is a dependency that cannot be automated because of access control restrictions from devops. It requires account delegation via SSO to an authorized 'Frankenstack' account for DNS/TLS Cert generation and must be run manually by an authorized user with active credentials. I would love for automations to be able to acquire DNS/TLS Certs relative to their env."
  type        = string
  default     = "Z07701642EZ0L3312RART"
}

variable "certificate_arn" {
  description = "Cert ARN for app -- this is a dependency that cannot be automated because of access control restrictions from devops. It requires account delegation via SSO to an authorized 'Frankenstack' account for DNS/TLS Cert generation and must be run manually by an authorized user with active credentials. I would love for automations to be able to acquire DNS/TLS Certs relative to their env."
  type        = string
  default     = "arn:aws:acm:us-west-2:631541896113:certificate/cf99c8b8-c689-449d-8d38-25a25d8bc14e"
}

variable "memory_allocation" {
  type    = number
  default = 512
}

variable "api_timeout" {
  type    = number
  default = 60
}

variable "dynatrace_key" {
  type      = string
  sensitive = true
}

#provider "aws" {
#  region  = var.region
#  profile = "default"
#}

terraform {
  required_version = ">= 1.0"

  backend "s3" {
    # This value must be globally unique across AWS.
#    bucket  = "ds-scheduling-reservation-terraform"
    # the bucket value should be supplied at runtime via the parameter to terraform init ie:
    # terraform init -backend-config="bucket=${TFSTATE_BUCKET}"
    # see: https://stackoverflow.com/a/63051134
    key     = "terraform.tfstate"
    region  = "us-west-2"
    profile = "default"
    encrypt = true
    dynamodb_table = "ds-scheduling-reservation-terraform-state"
  }

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

# Project Infrastructure

This project has the infrastructure defined using terraform targeting the AWS cloud.

## Current Status

### Stages
There are four AWS accounts pertaining to the four stages
- ds-scheduling-dev
- ds-scheduling-qa
- ds-scheduling-stage
- ds-scheduling-prod

as of now, infrastructure is only deployed to the dev and qa accounts.

### CI/CD
Github Actions is used for the CI/CD pipline. Values from the DNS and TLS certs that were deployed with Frankenstack are hardcoded into environment variables for each stage to use.

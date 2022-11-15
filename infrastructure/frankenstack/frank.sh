#!/usr/bin/env bash

echo Configuring AWS...
#aws configure set aws_access_key_id ${AWS_ACCESS_KEY}
#aws configure set aws_secret_access_key ${AWS_ACCESS_SECRET}
aws configure set default.region us-west-2
echo AWS Configured

aws sts get-caller-identity

echo Executing Frank...
echo frank deploy $1
frank deploy $1

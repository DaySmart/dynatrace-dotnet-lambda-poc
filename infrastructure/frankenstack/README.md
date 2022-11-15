This repo includes a containerized implementation to run frankenstack.

> **NOTE**: For most commands the frank tool requires the **frankenstack** aws sso account credentials and not the 
> local user or the relative aws environment account. A common indicator of using the wrong account is when 
> frankenstack throws a `Parameter is NULL` error.

To build the container image:
1. Navigate to the project `infrastructure/frankenstack` directory
2. execute `docker build -t ds/frank .`a. The 
   1. The container image should now be in your local registry available as `ds/frank`

The easiest way to issue a command to the frank container is by including mappings for your
aws credentials and passing the env var `AWS_PROFILE` that corresponds to the account you'd like to use.

ex: `-e AWS_PROFILE=frank -v $HOME/.aws/:/root/.aws/:ro`

Using volume mapping you can specify a template file to pass to frank. Here is an example of a full
command:

```
docker run --rm -it -e AWS_PROFILE=frank \
    -v \$HOME/.aws/:/root/.aws/:ro \
    -v $(pwd)/frank.sh:/usr/src/app/frank.sh:ro \
    -v $(pwd)/envs/dev/:/usr/src/app/dev:ro \
    --entrypoint=/bin/bash \
    ds/frank frank.sh dev/template.cert.yml
```

This command will:
- use your local aws credentials file accessing the profile `frank`.
- map the local `frank.sh` file to the container's working directory
- map the environment folder specified to the container's working directory
- set the entrypoint for the container since we want to run an esoteric command
- run the container executing the `frank.sh` file utilizing the mapped template file specified.
